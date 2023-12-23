using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "屏幕灰化 (Screen Binarization)")]
    public class ScreenBinarization : VolumeSettingBase
    {
        public override bool IsActive() => intensity.value > 0;
        public ClampedFloatParameter intensity = new(0, 0, 1);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 140)]
    public class ScreenBinarizationRenderer : VolumeRendererBase<ScreenBinarization>
    {
        public override string ProfilerTag => "ColorAdjustment-ScreenBinarization";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/ScreenBinarization";

        static class ShaderIDs
        {
            public static readonly int BinarizationAmount = Shader.PropertyToID("_BinarizationAmount");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.BinarizationAmount, m_Settings.intensity.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}