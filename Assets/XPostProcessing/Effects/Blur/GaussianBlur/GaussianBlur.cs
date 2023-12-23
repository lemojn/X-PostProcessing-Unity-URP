using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "高斯模糊 (Gaussian Blur)")]
    public class GaussianBlur : VolumeSettingBase
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 5f);
        public IntParameter Iteration = new ClampedIntParameter(6, 1, 15);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1f, 1f, 8f);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 10)]
    public class GaussianBlurRenderer : VolumeRendererBase<GaussianBlur>
    {
        public override string ProfilerTag => "Blur-GaussianBlur";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/GaussianBlur";

        private RTHandle m_BufferRT1;
        private RTHandle m_BufferRT2;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_BufferRT1 != null)
            {
                RTHandles.Release(m_BufferRT1);
                m_BufferRT1 = null;
            }
            if (m_BufferRT2 != null)
            {
                RTHandles.Release(m_BufferRT2);
                m_BufferRT2 = null;
            }
        }

        static class ShaderIDs
        {
            internal static readonly string BufferRT1 = "_BufferRT1";
            internal static readonly string BufferRT2 = "_BufferRT2";
            internal static readonly int BlurRadius = Shader.PropertyToID("_BlurOffset");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            var desc = GetDefaultColorRTDescriptor(ref renderingData, (int)(Screen.width / m_Settings.RTDownScaling.value), (int)(Screen.height / m_Settings.RTDownScaling.value));
            desc.colorFormat = RenderTextureFormat.ARGB32;
            desc.sRGB = true;
            RenderingUtils.ReAllocateIfNeeded(ref m_BufferRT1, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BufferRT1);
            RenderingUtils.ReAllocateIfNeeded(ref m_BufferRT2, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BufferRT2);

            Blitter.BlitCameraTexture(cmd, source, m_BufferRT1);
            for (int i = 0; i < m_Settings.Iteration.value; i++)
            {
                // horizontal blur
                m_BlitMaterial.SetVector(ShaderIDs.BlurRadius, new Vector4(m_Settings.BlurRadius.value / Screen.width, 0, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_BufferRT1, m_BufferRT2, m_BlitMaterial, 0);
                // vertical blur
                m_BlitMaterial.SetVector(ShaderIDs.BlurRadius, new Vector4(0, m_Settings.BlurRadius.value / Screen.height, 0, 0));
                Blitter.BlitCameraTexture(cmd, m_BufferRT2, m_BufferRT1, m_BlitMaterial, 0);
            }
            Blitter.BlitCameraTexture(cmd, m_BufferRT1, target);
        }

    }
}