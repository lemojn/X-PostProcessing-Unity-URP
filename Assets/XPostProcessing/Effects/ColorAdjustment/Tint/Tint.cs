using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "色调 (Tint)")]
    public class Tint : VolumeSettingBase
    {
        public override bool IsActive() => indensity.value > 0;
        public ClampedFloatParameter indensity = new(0, 0, 1);
        public ColorParameter colorTint = new(new Color(0.9f, 1, 0, 1), true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 100)]
    public class TintRenderer : VolumeRendererBase<Tint>
    {
        public override string ProfilerTag => "ColorAdjustment-Tint";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/Tint";

        static class ShaderIDs
        {
            internal static readonly int indensity = Shader.PropertyToID("_Indensity");
            internal static readonly int ColorTint = Shader.PropertyToID("_ColorTint");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.indensity, m_Settings.indensity.value);
            m_BlitMaterial.SetVector(ShaderIDs.ColorTint, m_Settings.colorTint.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}