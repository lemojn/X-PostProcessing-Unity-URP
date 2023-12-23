using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "错位线条故障 (Line Block Glitch)")]
    public class GlitchLineBlock : VolumeSettingBase
    {
        public override bool IsActive() => frequency.value > 0;
        public DirectionParameter blockDirection = new DirectionParameter(Direction.Horizontal);
        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter frequency = new ClampedFloatParameter(0f, 0f, 25f);
        public FloatParameter Amount = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter LinesWidth = new ClampedFloatParameter(1f, 0.1f, 10f);
        public FloatParameter Speed = new ClampedFloatParameter(0.8f, 0f, 1f);
        public FloatParameter Offset = new ClampedFloatParameter(1f, 0f, 13f);
        public FloatParameter Alpha = new ClampedFloatParameter(1f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 70)]
    public class GlitchLineBlockRenderer : VolumeRendererBase<GlitchLineBlock>
    {
        public override string ProfilerTag => "Glitch-GlitchLineBlock";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/LineBlock";

        private float m_TimeX = 1.0f;
        private float m_RandomFrequency;
        private int m_FrameCount = 0;

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Params2 = Shader.PropertyToID("_Params2");
        }

        private void UpdateFrequency()
        {
            if (m_Settings.intervalType.value == IntervalType.Random)
            {
                if (m_FrameCount > m_Settings.frequency.value)
                {
                    m_FrameCount = 0;
                    m_RandomFrequency = UnityEngine.Random.Range(0, m_Settings.frequency.value);
                }
                m_FrameCount++;
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

            m_TimeX += Time.deltaTime;
            if (m_TimeX > 100)
            {
                m_TimeX = 0;
            }

            float frequency = m_Settings.intervalType.value == IntervalType.Random ? m_RandomFrequency : m_Settings.frequency.value;
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(frequency, m_TimeX * m_Settings.Speed.value * 0.2f, m_Settings.Amount.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params2, new Vector3(m_Settings.Offset.value, 1 / m_Settings.LinesWidth.value, m_Settings.Alpha.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.blockDirection.value);
        }

    }
}