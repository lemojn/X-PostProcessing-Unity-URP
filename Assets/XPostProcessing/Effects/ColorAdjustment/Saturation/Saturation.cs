using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "饱和度 (Saturation)")]
    public class Saturation : VolumeSettingBase
    {
        public override bool IsActive() => saturation.value != 0;
        public ClampedFloatParameter saturation = new(0, -1, 1);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 80)]
    public class SaturationRenderer : VolumeRendererBase<Saturation>
    {
        public override string ProfilerTag => "ColorAdjustment-Saturation";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/Saturation";

        static class ShaderIDs
        {
            internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.Saturation, m_Settings.saturation.value + 1);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}