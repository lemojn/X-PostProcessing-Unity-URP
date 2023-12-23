using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "粒状模糊 (Grainy Blur)")]
    public class GrainyBlur : VolumeSettingBase
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 50f);
        public IntParameter Iteration = new ClampedIntParameter(4, 1, 8);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1f, 1f, 10f);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 120)]
    public class GrainyBlurRenderer : VolumeRendererBase<GrainyBlur>
    {
        public override string ProfilerTag => "Blur-GrainyBlur";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/GrainyBlur";

        private RTHandle m_BufferRT1;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_BufferRT1 != null)
            {
                RTHandles.Release(m_BufferRT1);
                m_BufferRT1 = null;
            }
        }

        static class ShaderIDs
        {
            internal static readonly string BufferRT1 = "_BufferRT1";
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.BlurRadius.value / Screen.height, m_Settings.Iteration.value));

            if (m_Settings.RTDownScaling.value > 1)
            {
                var desc = GetDefaultColorRTDescriptor(ref renderingData, (int)(Screen.width / m_Settings.RTDownScaling.value), (int)(Screen.height / m_Settings.RTDownScaling.value));
                desc.colorFormat = RenderTextureFormat.ARGB32;
                desc.sRGB = true;
                RenderingUtils.ReAllocateIfNeeded(ref m_BufferRT1, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BufferRT1);
                Blitter.BlitCameraTexture(cmd, source, m_BufferRT1);
                Blitter.BlitCameraTexture(cmd, m_BufferRT1, target, m_BlitMaterial, 0);
            }
            else
            {
                Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
            }
        }

    }
}