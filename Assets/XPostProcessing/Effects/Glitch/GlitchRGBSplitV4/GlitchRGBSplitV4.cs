using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "RGB颜色分离V4 (RGB Split V4)")]
    public class GlitchRGBSplitV4 : VolumeSettingBase
    {
        public override bool IsActive() => indensity.value != 0;
        public DirectionEXParameter SplitDirection = new DirectionEXParameter(DirectionEX.Horizontal);
        public FloatParameter indensity = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter speed = new ClampedFloatParameter(10f, 0f, 100f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 110)]
    public sealed class GlitchRGBSplitV4Renderer : VolumeRendererBase<GlitchRGBSplitV4>
    {
        public override string ProfilerTag => "Glitch-GlitchRGBSplitV4";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/RGBSplitV4";

        private float m_TimeX = 1.0f;

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Params2 = Shader.PropertyToID("_Params2");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_TimeX += Time.deltaTime;
            if (m_TimeX > 100)
            {
                m_TimeX = 0;
            }

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.indensity.value * 0.1f, Mathf.Floor(m_TimeX * m_Settings.speed.value)));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.SplitDirection.value);
        }

    }
}