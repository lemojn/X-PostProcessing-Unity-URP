using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "扫描线抖动故障 (Scane Line Jitter Glitch)")]
    public class GlitchScanLineJitter : VolumeSettingBase
    {
        public override bool IsActive() => frequency.value > 0;
        public DirectionParameter JitterDirection = new DirectionParameter(Direction.Horizontal);
        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 25f);
        public FloatParameter JitterIndensity = new ClampedFloatParameter(0.1f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 130)]
    public class GlitchScanLineJitterRenderer : VolumeRendererBase<GlitchScanLineJitter>
    {
        public override string ProfilerTag => "Glitch-GlitchScanLineJitter";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/ScanLineJitter";

        private float m_RandomFrequency;

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int JitterIndensity = Shader.PropertyToID("_ScanLineJitter");
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

            float displacement = 0.005f + Mathf.Pow(m_Settings.JitterIndensity.value, 3) * 0.1f;
            float threshold = Mathf.Clamp01(1.0f - m_Settings.JitterIndensity.value * 1.2f);
            float frequency = m_Settings.intervalType.value == IntervalType.Random ? m_RandomFrequency : m_Settings.frequency.value;
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(displacement, threshold, frequency));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.JitterDirection.value);
        }

    }
}