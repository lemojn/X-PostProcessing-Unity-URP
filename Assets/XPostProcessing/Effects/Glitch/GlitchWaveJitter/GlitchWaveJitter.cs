using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "波动抖动故障 (Wave Jitter Glitch)")]
    public class GlitchWaveJitter : VolumeSettingBase
    {
        public override bool IsActive() => frequency.value > 0;
        public DirectionParameter jitterDirection = new DirectionParameter(Direction.Horizontal);
        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 50f);
        public FloatParameter RGBSplit = new ClampedFloatParameter(20f, 0f, 50f);
        public FloatParameter speed = new ClampedFloatParameter(0.25f, 0f, 1f);
        public FloatParameter amount = new ClampedFloatParameter(1f, 0f, 2f);
        public BoolParameter customResolution = new BoolParameter(false);
        public Vector2Parameter resolution = new Vector2Parameter(new Vector2(640f, 480f));
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 170)]
    public sealed class GlitchWaveJitterRenderer : VolumeRendererBase<GlitchWaveJitter>
    {
        public override string ProfilerTag => "Glitch-GlitchWaveJitter";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/WaveJitter";

        private float m_RandomFrequency;

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Resolution = Shader.PropertyToID("_Resolution");
        }

        private void UpdateFrequency()
        {
            if (m_Settings.intervalType.value == IntervalType.Random)
            {
                m_RandomFrequency = UnityEngine.Random.Range(0, m_Settings.frequency.value);
            }
            if (m_Settings.intervalType.value == IntervalType.Infinite)
            {
                m_BlitMaterial.EnableKeyword("USING_FREQUENCY_INFINITE");
            }
            else
            {
                m_BlitMaterial.DisableKeyword("USING_FREQUENCY_INFINITE");
            }
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            UpdateFrequency();

            float frequency = m_Settings.intervalType.value == IntervalType.Random ? m_RandomFrequency : m_Settings.frequency.value;
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(frequency, m_Settings.RGBSplit.value, m_Settings.speed.value, m_Settings.amount.value));
            m_BlitMaterial.SetVector(ShaderIDs.Resolution, m_Settings.customResolution.value ? m_Settings.resolution.value : new Vector2(Screen.width, Screen.height));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.jitterDirection.value);
        }

    }
}