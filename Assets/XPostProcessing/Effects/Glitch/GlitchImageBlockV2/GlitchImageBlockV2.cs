using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "错位图块故障V2 (Image Block Glitch V2)")]
    public class GlitchImageBlockV2 : VolumeSettingBase
    {
        public override bool IsActive() => Fade.value > 0;
        public FloatParameter Fade = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter Speed = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter Amount = new ClampedFloatParameter(1f, 0f, 10f);
        public FloatParameter BlockLayer1_U = new ClampedFloatParameter(2f, 0f, 50f);
        public FloatParameter BlockLayer1_V = new ClampedFloatParameter(16f, 0f, 50f);
        public FloatParameter BlockLayer1_Indensity = new ClampedFloatParameter(8f, 0f, 50f);
        public FloatParameter RGBSplitIndensity = new ClampedFloatParameter(2f, 0f, 50f);

        public BoolParameter BlockVisualizeDebug = new BoolParameter(false);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 40)]
    public class GlitchImageBlockV2Renderer : VolumeRendererBase<GlitchImageBlockV2>
    {
        public override string ProfilerTag => "Glitch-GlitchImageBlockV2";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/ImageBlockV2";

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

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(m_TimeX * m_Settings.Speed.value, m_Settings.Amount.value, m_Settings.Fade.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params2, new Vector4(m_Settings.BlockLayer1_U.value, m_Settings.BlockLayer1_V.value, m_Settings.BlockLayer1_Indensity.value, m_Settings.RGBSplitIndensity.value));
            if (m_Settings.BlockVisualizeDebug.value)
            {
                //debug
                Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 1);
            }
            else
            {
                Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
            }
        }

    }
}