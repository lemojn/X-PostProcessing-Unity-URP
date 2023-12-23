using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "屏幕跳跃故障 (Screen Jump Glitch)")]
    public class GlitchScreenJump : VolumeSettingBase
    {
        public override bool IsActive() => ScreenJumpIndensity.value > 0;
        public DirectionParameter ScreenJumpDirection = new DirectionParameter(Direction.Vertical);
        public FloatParameter ScreenJumpIndensity = new ClampedFloatParameter(0f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 140)]
    public class GlitchScreenJumpRenderer : VolumeRendererBase<GlitchScreenJump>
    {
        public override string ProfilerTag => "Glitch-GlitchScreenJump";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/ScreenJump";

        private float m_ScreenJumpTime;

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_ScreenJumpTime += Time.deltaTime * m_Settings.ScreenJumpIndensity.value * 9.8f;
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.ScreenJumpIndensity.value, m_ScreenJumpTime));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.ScreenJumpDirection.value);
        }

    }
}