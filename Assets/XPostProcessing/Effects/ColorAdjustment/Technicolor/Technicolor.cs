using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "胶片 (Technicolor)")]
    public class Technicolor : VolumeSettingBase
    {
        public override bool IsActive() => indensity.value > 0;
        public FloatParameter indensity = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter exposure = new ClampedFloatParameter(4f, 0f, 8f);
        public FloatParameter colorBalanceR = new ClampedFloatParameter(0.2f, 0f, 1f);
        public FloatParameter colorBalanceG = new ClampedFloatParameter(0.2f, 0f, 1f);
        public FloatParameter colorBalanceB = new ClampedFloatParameter(0.2f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 90)]
    public class TechnicolorRenderer : VolumeRendererBase<Technicolor>
    {
        public override string ProfilerTag => "ColorAdjustment-Technicolor";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/Technicolor";

        static class ShaderIDs
        {
            internal static readonly int exposure = Shader.PropertyToID("_Exposure");
            internal static readonly int colorBalance = Shader.PropertyToID("_ColorBalance");
            internal static readonly int indensity = Shader.PropertyToID("_Indensity");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.exposure, 8f - m_Settings.exposure.value);
            m_BlitMaterial.SetVector(ShaderIDs.colorBalance, Vector3.one - new Vector3(m_Settings.colorBalanceR.value, m_Settings.colorBalanceG.value, m_Settings.colorBalanceB.value));
            m_BlitMaterial.SetFloat(ShaderIDs.indensity, m_Settings.indensity.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}