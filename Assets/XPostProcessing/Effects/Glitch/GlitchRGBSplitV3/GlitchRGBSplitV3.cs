using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "RGB颜色分离V3 (RGB Split V3)")]
    public class GlitchRGBSplitV3 : VolumeSettingBase
    {
        public override bool IsActive() => Frequency.value > 0;
        public DirectionEXParameter SplitDirection = new DirectionEXParameter(DirectionEX.Horizontal);
        public IntervalTypeParameter intervalType = new IntervalTypeParameter(IntervalType.Random);
        public FloatParameter Frequency = new ClampedFloatParameter(0f, 0.1f, 25f);
        public FloatParameter Amount = new ClampedFloatParameter(30f, 0f, 200f);
        public FloatParameter Speed = new ClampedFloatParameter(20f, 0f, 20f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 100)]
    public sealed class GlitchRGBSplitV3Renderer : VolumeRendererBase<GlitchRGBSplitV3>
    {
        public override string ProfilerTag => "Glitch-GlitchRGBSplitV3";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/RGBSplitV3";

        private float m_RandomFrequency;
        private int m_FrameCount = 0;

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        private void UpdateFrequency()
        {
            if (m_Settings.intervalType.value == IntervalType.Random)
            {
                if (m_FrameCount > m_Settings.Frequency.value)
                {
                    m_FrameCount = 0;
                    m_RandomFrequency = UnityEngine.Random.Range(0, m_Settings.Frequency.value);
                }
                m_FrameCount++;
            }

            if (m_Settings.intervalType.value == IntervalType.Infinite)
            {
                m_BlitMaterial.EnableKeyword("USING_Frequency_INFINITE");
            }
            else
            {
                m_BlitMaterial.DisableKeyword("USING_Frequency_INFINITE");
            }
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            UpdateFrequency();
            float frequency = m_Settings.intervalType.value == IntervalType.Random ? m_RandomFrequency : m_Settings.Frequency.value;
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(frequency, m_Settings.Amount.value, m_Settings.Speed.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.SplitDirection.value);
        }

    }
}