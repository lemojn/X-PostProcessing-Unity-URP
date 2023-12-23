using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "错位图块故障V4 (Image Block Glitch V4)")]
    public class GlitchImageBlockV4 : VolumeSettingBase
    {
        public override bool IsActive() => Speed.value > 0;
        public FloatParameter Speed = new ClampedFloatParameter(0f, 0f, 50f);
        public FloatParameter BlockSize = new ClampedFloatParameter(8f, 0f, 50f);
        public FloatParameter MaxRGBSplitX = new ClampedFloatParameter(1f, 0f, 25f);
        public FloatParameter MaxRGBSplitY = new ClampedFloatParameter(1f, 0f, 25f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 60)]
    public class GlitchImageBlockV4Renderer : VolumeRendererBase<GlitchImageBlockV4>
    {
        public override string ProfilerTag => "Glitch-GlitchImageBlockV4";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/ImageBlockV4";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");

        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(m_Settings.Speed.value, m_Settings.BlockSize.value, m_Settings.MaxRGBSplitX.value, m_Settings.MaxRGBSplitY.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}