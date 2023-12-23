using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "漂白 (Bleach Bypass)")]
    public class BleachBypass : VolumeSettingBase
    {
        public override bool IsActive() => Indensity.value > 0;
        public FloatParameter Indensity = new ClampedFloatParameter(0, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 10)]
    public class BleachBypassRenderer : VolumeRendererBase<BleachBypass>
    {
        public override string ProfilerTag => "ColorAdjustment-BleachBypass";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/BleachBypass";

        static class ShaderIDs
        {
            internal static readonly int Indensity = Shader.PropertyToID("_Indensity");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.Indensity, m_Settings.Indensity.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}