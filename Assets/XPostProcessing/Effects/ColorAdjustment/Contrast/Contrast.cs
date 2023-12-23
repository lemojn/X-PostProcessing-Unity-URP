using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "对比度 (Contrast)")]
    public class Contrast : VolumeSettingBase
    {
        public override bool IsActive() => contrast.value != 0;
        public FloatParameter contrast = new ClampedFloatParameter(0, -1, 2);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 30)]
    public class ContrastRenderer : VolumeRendererBase<Contrast>
    {
        public override string ProfilerTag => "ColorAdjustment-Contrast";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/Contrast";

        static class ShaderIDs
        {
            internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.Contrast, m_Settings.contrast.value + 1f);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}