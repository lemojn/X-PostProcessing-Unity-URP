using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "亮度 (Brightness)")]
    public class Brightness : VolumeSettingBase
    {
        public override bool IsActive() => Indensity.value != 0;
        public FloatParameter Indensity = new ClampedFloatParameter(0, -0.9f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 20)]
    public class BrightnessRenderer : VolumeRendererBase<Brightness>
    {
        public override string ProfilerTag => "ColorAdjustment-Brightness";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/Brightness";

        static class ShaderIDs
        {
            internal static readonly int Indensity = Shader.PropertyToID("_Brightness");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.Indensity, m_Settings.Indensity.value + 1f);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}