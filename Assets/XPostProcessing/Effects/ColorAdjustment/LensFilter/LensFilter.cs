using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "镜头滤光 (Lens Filter)")]
    public class LensFilter : VolumeSettingBase
    {
        public override bool IsActive() => Indensity.value > 0;
        public ClampedFloatParameter Indensity = new(0, 0, 1);
        public ColorParameter LensColor = new(new Color(1, 1, 0.1f, 1), true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 70)]
    public class LensFilterRenderer : VolumeRendererBase<LensFilter>
    {
        public override string ProfilerTag => "ColorAdjustment-LensFilter";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/LensFilter";

        static class ShaderIDs
        {
            internal static readonly int LensColor = Shader.PropertyToID("_LensColor");
            internal static readonly int Indensity = Shader.PropertyToID("_Indensity");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.Indensity, m_Settings.Indensity.value);
            m_BlitMaterial.SetColor(ShaderIDs.LensColor, m_Settings.LensColor.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}