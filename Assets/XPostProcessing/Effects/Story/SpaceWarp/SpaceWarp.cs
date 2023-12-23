using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Story + "空间扭曲鱼眼 (Space Warp)")]
    public class SpaceWarp : VolumeSettingBase
    {
        public override bool IsActive() => intensity.value > 0;
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0, 0, 1);
    }

    [VolumeRendererPriority(VolumePriority.Story + 10)]
    public class SpaceWarpRendererBase : VolumeRendererBase<SpaceWarp>
    {
        public override string ProfilerTag => "Story-SpaceWarp";
        protected override string ShaderName => "Hidden/XPostProcessing/Story/SpaceWarp";

        static class ShaderIDs
        {
            internal static readonly int SpaceWarpLength = Shader.PropertyToID("_SpaceWarpLength");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.SpaceWarpLength, 1 - m_Settings.intensity.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}