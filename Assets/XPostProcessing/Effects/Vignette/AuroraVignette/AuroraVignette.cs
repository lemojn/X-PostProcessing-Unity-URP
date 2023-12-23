using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Vignette + "极光渐晕 (Aurora Vignette)")]
    public class AuroraVignette : VolumeSettingBase
    {
        public override bool IsActive() => vignetteArea.value > 0;
        public FloatParameter vignetteArea = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter vignetteSmothness = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter vignetteFading = new ClampedFloatParameter(1f, 0f, 1f);
        public FloatParameter colorChange = new ClampedFloatParameter(0.1f, 0.1f, 1f);
        public FloatParameter colorFactorR = new ClampedFloatParameter(1f, 0f, 2f);
        public FloatParameter colorFactorG = new ClampedFloatParameter(1f, 0f, 2f);
        public FloatParameter colorFactorB = new ClampedFloatParameter(1f, 0f, 2f);
        public FloatParameter flowSpeed = new ClampedFloatParameter(1f, -2f, 2f);
    }

    [VolumeRendererPriority(VolumePriority.Vignette + 50)]
    public class AuroraVignetteRenderer : VolumeRendererBase<AuroraVignette>
    {
        public override string ProfilerTag => "Vignette-AuroraVignette";
        protected override string ShaderName => "Hidden/XPostProcessing/Vignette/AuroraVignette";

        private float m_TimeX = 1.0f;

        static class ShaderIDs
        {
            internal static readonly int vignetteArea = Shader.PropertyToID("_VignetteArea");
            internal static readonly int vignetteSmothness = Shader.PropertyToID("_VignetteSmothness");
            internal static readonly int colorChange = Shader.PropertyToID("_ColorChange");
            internal static readonly int colorFactor = Shader.PropertyToID("_ColorFactor");
            internal static readonly int TimeX = Shader.PropertyToID("_TimeX");
            internal static readonly int vignetteFading = Shader.PropertyToID("_Fading");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_TimeX += Time.deltaTime;
            if (m_TimeX > 100)
            {
                m_TimeX = 0;
            }

            m_BlitMaterial.SetFloat(ShaderIDs.vignetteArea, m_Settings.vignetteArea.value);
            m_BlitMaterial.SetFloat(ShaderIDs.vignetteSmothness, m_Settings.vignetteSmothness.value);
            m_BlitMaterial.SetFloat(ShaderIDs.colorChange, m_Settings.colorChange.value * 10f);
            m_BlitMaterial.SetVector(ShaderIDs.colorFactor, new Vector3(m_Settings.colorFactorR.value, m_Settings.colorFactorG.value, m_Settings.colorFactorB.value));
            m_BlitMaterial.SetFloat(ShaderIDs.TimeX, m_TimeX * m_Settings.flowSpeed.value);
            m_BlitMaterial.SetFloat(ShaderIDs.vignetteFading, m_Settings.vignetteFading.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}