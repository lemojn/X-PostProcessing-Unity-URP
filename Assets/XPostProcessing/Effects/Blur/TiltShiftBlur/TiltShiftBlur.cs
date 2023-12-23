using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "移轴模糊 (Tilt Shift Blur)")]
    public class TiltShiftBlur : VolumeSettingBase
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public TiltShiftBlurQualityLevelParameter QualityLevel = new(TiltShiftBlurQualityLevel.High_Quality);
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter AreaSize = new ClampedFloatParameter(0.5f, 0f, 1f);
        public IntParameter Iteration = new ClampedIntParameter(2, 1, 8);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1f, 1f, 2f);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 90)]
    public class TiltShiftBlurRenderer : VolumeRendererBase<TiltShiftBlur>
    {
        public override string ProfilerTag => "Blur-TiltShiftBlur";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/TiltShiftBlur";

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
            internal static readonly int BlurredTex = Shader.PropertyToID("_BlurredTex");
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            var desc = GetDefaultColorRTDescriptor(ref renderingData, (int)(Screen.width / m_Settings.RTDownScaling.value), (int)(Screen.height / m_Settings.RTDownScaling.value));
            desc.colorFormat = RenderTextureFormat.ARGB32;
            desc.sRGB = true;
            RenderingUtils.ReAllocateIfNeeded(ref m_BufferRT1, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BufferRT1);

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.AreaSize.value, m_Settings.BlurRadius.value));
            Blitter.BlitCameraTexture(cmd, source, m_BufferRT1, m_BlitMaterial, (int)m_Settings.QualityLevel.value);
            m_BlitMaterial.SetTexture(ShaderIDs.BlurredTex, m_BufferRT1);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 2);
        }

    }
}