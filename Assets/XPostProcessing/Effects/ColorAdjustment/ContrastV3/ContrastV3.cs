using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "对比度V3 (Contrast V3)")]
    public class ContrastV3 : VolumeSettingBase
    {
        public override bool IsActive() => contrast.value > 0;
        public ClampedFloatParameter contrast = new(0, 0, 5);
        public ColorParameter contrastFactor = new(Color.black, false, false, true);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 50)]
    public class ContrastV3Renderer : VolumeRendererBase<ContrastV3>
    {
        public override string ProfilerTag => "ColorAdjustment-ContrastV3";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/ContrastV3";

        static class ShaderIDs
        {
            internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Contrast, new Vector4(m_Settings.contrastFactor.value.r, m_Settings.contrastFactor.value.g,
            m_Settings.contrastFactor.value.b, m_Settings.contrast.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}