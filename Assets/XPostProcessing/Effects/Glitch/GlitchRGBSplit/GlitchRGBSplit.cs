using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "RGB颜色分离 (RGB Split)")]
    public class GlitchRGBSplit : VolumeSettingBase
    {
        public override bool IsActive() => Fading.value > 0;
        public DirectionEXParameter SplitDirection = new DirectionEXParameter(DirectionEX.Horizontal);
        public FloatParameter Fading = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter Amount = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter Speed = new ClampedFloatParameter(0f, 0f, 10f);
        public ClampedFloatParameter CenterFading = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter AmountR = new ClampedFloatParameter(1f, 0f, 5f);
        public ClampedFloatParameter AmountB = new ClampedFloatParameter(1f, 0f, 5f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 80)]
    public sealed class GlitchRGBSplitRenderer : VolumeRendererBase<GlitchRGBSplit>
    {
        public override string ProfilerTag => "Glitch-GlitchRGBSplit";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/RGBSplit";

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

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(m_Settings.Fading.value, m_Settings.Amount.value, m_Settings.Speed.value, m_Settings.CenterFading.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params2, new Vector3(m_TimeX, m_Settings.AmountR.value, m_Settings.AmountB.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.SplitDirection.value);
        }

    }
}