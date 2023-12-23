using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "对比度V2 (Contrast V2)")]
    public class ContrastV2 : VolumeSettingBase
    {
        public override bool IsActive() => contrast.value != 0;
        public FloatParameter contrast = new ClampedFloatParameter(0, -1, 5);
        public FloatParameter ContrastFactorR = new ClampedFloatParameter(0, -1, 1);
        public FloatParameter ContrastFactorG = new ClampedFloatParameter(0, -1, 1);
        public FloatParameter ContrastFactorB = new ClampedFloatParameter(0, -1, 1);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 40)]
    public class ContrastV2Renderer : VolumeRendererBase<ContrastV2>
    {
        public override string ProfilerTag => "ColorAdjustment-ContrastV2";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/ContrastV2";

        static class ShaderIDs
        {
            internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Contrast, new Vector4(m_Settings.ContrastFactorR.value, m_Settings.ContrastFactorG.value,
            m_Settings.ContrastFactorB.value, m_Settings.contrast.value + 1));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}