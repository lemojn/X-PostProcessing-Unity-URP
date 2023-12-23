using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "图块抖动故障 (Tile Jitter Glitch)")]
    public class GlitchTileJitter : VolumeSettingBase
    {
        public override bool IsActive() => frequency.value > 0;
        public DirectionParameter jitterDirection = new DirectionParameter(Direction.Horizontal);
        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 25f);
        public DirectionParameter splittingDirection = new DirectionParameter(Direction.Vertical);
        public FloatParameter splittingNumber = new ClampedFloatParameter(5f, 0f, 20f);
        public FloatParameter amount = new ClampedFloatParameter(10f, 0f, 100f);
        public FloatParameter speed = new ClampedFloatParameter(0.35f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 160)]
    public class GlitchTileJitterRenderer : VolumeRendererBase<GlitchTileJitter>
    {
        public override string ProfilerTag => "Glitch-GlitchTileJitter";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/TileJitter";

        private float m_RandomFrequency;

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
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

            if (m_Settings.jitterDirection.value == Direction.Horizontal)
            {
                m_BlitMaterial.EnableKeyword("JITTER_DIRECTION_HORIZONTAL");
            }
            else
            {
                m_BlitMaterial.DisableKeyword("JITTER_DIRECTION_HORIZONTAL");
            }

            float frequency = m_Settings.intervalType.value == IntervalType.Random ? m_RandomFrequency : m_Settings.frequency.value;
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(m_Settings.splittingNumber.value, m_Settings.amount.value, m_Settings.speed.value * 100f, frequency));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.splittingDirection.value);
        }

    }
}