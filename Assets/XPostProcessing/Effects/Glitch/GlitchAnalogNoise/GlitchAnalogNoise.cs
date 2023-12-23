using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "模拟噪点故障 (Analog Noise Glitch)")]
    public class GlitchAnalogNoise : VolumeSettingBase
    {
        public override bool IsActive() => NoiseFading.value > 0;
        public FloatParameter NoiseFading = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter NoiseSpeed = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter LuminanceJitterThreshold = new ClampedFloatParameter(0.8f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 10)]
    public class GlitchAnalogNoiseRenderer : VolumeRendererBase<GlitchAnalogNoise>
    {
        public override string ProfilerTag => "Glitch-GlitchAnalogNoise";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/AnalogNoise";

        private float m_TimeX = 1.0f;

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_TimeX += Time.deltaTime;
            if (m_TimeX > 100)
            {
                m_TimeX = 0;
            }
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(m_Settings.NoiseSpeed.value, m_Settings.NoiseFading.value, m_Settings.LuminanceJitterThreshold.value, m_TimeX));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}