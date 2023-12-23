using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "RGB颜色分离V2 (RGB Split V2)")]
    public class GlitchRGBSplitV2 : VolumeSettingBase
    {
        public override bool IsActive() => Amount.value > 0;
        public DirectionEXParameter SplitDirection = new DirectionEXParameter(DirectionEX.Horizontal);
        public FloatParameter Amount = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter Amplitude = new ClampedFloatParameter(3f, 1f, 6f);
        public FloatParameter Speed = new ClampedFloatParameter(1f, 0f, 2f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 90)]
    public sealed class GlitchRGBSplitV2Renderer : VolumeRendererBase<GlitchRGBSplitV2>
    {
        public override string ProfilerTag => "Glitch-GlitchRGBSplitV2";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/RGBSplitV2";

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

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(m_TimeX * m_Settings.Speed.value, m_Settings.Amount.value, m_Settings.Amplitude.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.SplitDirection.value);
        }

    }
}