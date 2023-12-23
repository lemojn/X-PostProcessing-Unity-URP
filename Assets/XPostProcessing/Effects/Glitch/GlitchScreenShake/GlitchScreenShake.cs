using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "屏幕抖动故障 (Screen Shake Glitch)")]
    public class GlitchScreenShake : VolumeSettingBase
    {
        public override bool IsActive() => ScreenShakeIndensity.value > 0;
        public DirectionParameter ScreenShakeDirection = new DirectionParameter(Direction.Horizontal);
        public FloatParameter ScreenShakeIndensity = new ClampedFloatParameter(0f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 150)]
    public class GlitchScreenShakeRenderer : VolumeRendererBase<GlitchScreenShake>
    {
        public override string ProfilerTag => "Glitch-GlitchScreenShake";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/ScreenShake";

        static class ShaderIDs
        {
            internal static readonly int ScreenShakeIndensity = Shader.PropertyToID("_ScreenShake");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.ScreenShakeIndensity, m_Settings.ScreenShakeIndensity.value * 0.25f);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.ScreenShakeDirection.value);
        }

    }
}