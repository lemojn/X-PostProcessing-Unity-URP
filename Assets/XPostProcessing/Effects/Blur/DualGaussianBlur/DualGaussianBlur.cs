using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "双重高斯模糊 (Dual Gaussian Blur)")]
    public class DualGaussianBlur : VolumeSettingBase
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 15f);
        public IntParameter Iteration = new ClampedIntParameter(4, 1, 8);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1f, 1f, 10f);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 50)]
    public class DualGaussianBlurRenderer : VolumeRendererBase<DualGaussianBlur>
    {
        public override string ProfilerTag => "Blur-DualGaussianBlur";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/DualGaussianBlur";

        private RTHandle m_TempLastRT;
        private Level[] m_Pyramid;
        private const int k_MaxPyramidSize = 16;

        public override void Init()
        {
            base.Init();

            m_Pyramid = new Level[k_MaxPyramidSize];
            for (int i = 0; i < k_MaxPyramidSize; i++)
            {
                m_Pyramid[i] = new Level
                {
                    vDownName = "_BlurMipDownV" + i,
                    hDownName = "_BlurMipDownH" + i,
                    vUpName = "_BlurMipUpV" + i,
                    hUpName = "_BlurMipUpH" + i,
                    vDownRT = null,
                    hDownRT = null,
                    vUpRT = null,
                    hUpRT = null,
                };
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            for (int i = 0; i < k_MaxPyramidSize; i++)
            {
                if (m_Pyramid[i].vDownRT != null)
                {
                    RTHandles.Release(m_Pyramid[i].vDownRT);
                    m_Pyramid[i].vDownRT = null;
                }
                if (m_Pyramid[i].hDownRT != null)
                {
                    RTHandles.Release(m_Pyramid[i].hDownRT);
                    m_Pyramid[i].hDownRT = null;
                }
                if (m_Pyramid[i].vUpRT != null)
                {
                    RTHandles.Release(m_Pyramid[i].vUpRT);
                    m_Pyramid[i].vUpRT = null;
                }
                if (m_Pyramid[i].hUpRT != null)
                {
                    RTHandles.Release(m_Pyramid[i].hUpRT);
                    m_Pyramid[i].hUpRT = null;
                }
            }
        }

        struct Level
        {
            internal string vDownName;
            internal string hDownName;
            internal string vUpName;
            internal string hUpName;
            internal RTHandle vDownRT;
            internal RTHandle hDownRT;
            internal RTHandle vUpRT;
            internal RTHandle hUpRT;
        }

        static class ShaderIDs
        {
            internal static readonly int BlurOffset = Shader.PropertyToID("_BlurOffset");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            var desc = GetDefaultColorRTDescriptor(ref renderingData, (int)(Screen.width / m_Settings.RTDownScaling.value), (int)(Screen.height / m_Settings.RTDownScaling.value));
            desc.colorFormat = RenderTextureFormat.ARGB32;
            desc.sRGB = true;
            // Downsample
            m_TempLastRT = source;
            for (int i = 0; i < m_Settings.Iteration.value; i++)
            {
                RenderingUtils.ReAllocateIfNeeded(ref m_Pyramid[i].vDownRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_Pyramid[i].vDownName);
                RenderingUtils.ReAllocateIfNeeded(ref m_Pyramid[i].hDownRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_Pyramid[i].hDownName);
                RenderingUtils.ReAllocateIfNeeded(ref m_Pyramid[i].vUpRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_Pyramid[i].vUpName);
                RenderingUtils.ReAllocateIfNeeded(ref m_Pyramid[i].hUpRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_Pyramid[i].hUpName);
                // horizontal blur
                m_BlitMaterial.SetVector(ShaderIDs.BlurOffset, new Vector4(m_Settings.BlurRadius.value / Screen.width, 0, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_TempLastRT, m_Pyramid[i].hDownRT, m_BlitMaterial, 0);
                // vertical blur
                m_BlitMaterial.SetVector(ShaderIDs.BlurOffset, new Vector4(0, m_Settings.BlurRadius.value / Screen.height, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_Pyramid[i].hDownRT, m_Pyramid[i].vDownRT, m_BlitMaterial, 0);
                m_TempLastRT = m_Pyramid[i].vDownRT;

                desc.width = Mathf.Max(desc.width / 2, 1);
                desc.height = Mathf.Max(desc.height / 2, 1);
            }
            // Upsample
            for (int i = m_Settings.Iteration.value - 2; i >= 0; i--)
            {
                // horizontal blur
                m_BlitMaterial.SetVector(ShaderIDs.BlurOffset, new Vector4(m_Settings.BlurRadius.value / Screen.width, 0, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_TempLastRT, m_Pyramid[i].hUpRT, m_BlitMaterial, 0);
                // vertical blur
                m_BlitMaterial.SetVector(ShaderIDs.BlurOffset, new Vector4(0, m_Settings.BlurRadius.value / Screen.height, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_Pyramid[i].hUpRT, m_Pyramid[i].vUpRT, m_BlitMaterial, 0);
                m_TempLastRT = m_Pyramid[i].vUpRT;
            }
            // Render blurred texture in blend pass
            Blitter.BlitCameraTexture(cmd, m_TempLastRT, target);
            m_TempLastRT = null;
        }

    }
}