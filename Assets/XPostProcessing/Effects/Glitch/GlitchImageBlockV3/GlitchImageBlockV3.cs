using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "错位图块故障V3 (Image Block Glitch V3)")]
    public class GlitchImageBlockV3 : VolumeSettingBase
    {
        public override bool IsActive() => Speed.value > 0;
        public FloatParameter Speed = new ClampedFloatParameter(0f, 0f, 50f);
        public FloatParameter BlockSize = new ClampedFloatParameter(8f, 0f, 50f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 50)]
    public class GlitchImageBlockV3Renderer : VolumeRendererBase<GlitchImageBlockV3>
    {
        public override string ProfilerTag => "Glitch-GlitchImageBlockV3";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/ImageBlockV3";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.Speed.value, m_Settings.BlockSize.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}