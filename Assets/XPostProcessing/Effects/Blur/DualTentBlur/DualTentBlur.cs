using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "双重Tent模糊 (Dual Tent Blur)")]
    public class DualTentBlur : VolumeSettingBase
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 15f);
        public IntParameter Iteration = new ClampedIntParameter(4, 1, 8);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(2f, 1f, 10f);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 70)]
    public class DualTentBlurRenderer : VolumeRendererBase<DualTentBlur>
    {
        public override string ProfilerTag => "Blur-DualTentBlur";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/DualTentBlur";

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
                    downName = "_BlurMipDown" + i,
                    upName = "_BlurMipUp" + i,
                    downRT = null,
                    upRT = null,
                };
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            for (int i = 0; i < k_MaxPyramidSize; i++)
            {
                if (m_Pyramid[i].downRT != null)
                {
                    RTHandles.Release(m_Pyramid[i].downRT);
                    m_Pyramid[i].downRT = null;
                }
                if (m_Pyramid[i].upRT != null)
                {
                    RTHandles.Release(m_Pyramid[i].upRT);
                    m_Pyramid[i].upRT = null;
                }
            }
        }

        struct Level
        {
            internal string downName;
            internal string upName;
            internal RTHandle downRT;
            internal RTHandle upRT;
        }

        static class ShaderIDs
        {
            internal static readonly int BlurOffset = Shader.PropertyToID("_BlurOffset");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            Vector4 BlurOffset = new(m_Settings.BlurRadius.value / Screen.width, m_Settings.BlurRadius.value / Screen.height, 0, 0);
            m_BlitMaterial.SetVector(ShaderIDs.BlurOffset, BlurOffset);

            var desc = GetDefaultColorRTDescriptor(ref renderingData, (int)(Screen.width / m_Settings.RTDownScaling.value), (int)(Screen.height / m_Settings.RTDownScaling.value));
            desc.colorFormat = RenderTextureFormat.ARGB32;
            desc.sRGB = true;
            // Downsample
            m_TempLastRT = source;
            for (int i = 0; i < m_Settings.Iteration.value; i++)
            {
                RenderingUtils.ReAllocateIfNeeded(ref m_Pyramid[i].downRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_Pyramid[i].downName);
                RenderingUtils.ReAllocateIfNeeded(ref m_Pyramid[i].upRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_Pyramid[i].upName);
                Blitter.BlitCameraTexture(cmd, m_TempLastRT, m_Pyramid[i].downRT, m_BlitMaterial, 0);
                m_TempLastRT = m_Pyramid[i].downRT;

                desc.width = Mathf.Max(desc.width / 2, 1);
                desc.height = Mathf.Max(desc.height / 2, 1);
            }
            // Upsample
            for (int i = m_Settings.Iteration.value - 2; i >= 0; i--)
            {
                Blitter.BlitCameraTexture(cmd, m_TempLastRT, m_Pyramid[i].upRT, m_BlitMaterial, 0);
                m_TempLastRT = m_Pyramid[i].upRT;
            }
            // Render blurred texture in blend pass
            Blitter.BlitCameraTexture(cmd, m_TempLastRT, target);
            m_TempLastRT = null;
        }

    }
}