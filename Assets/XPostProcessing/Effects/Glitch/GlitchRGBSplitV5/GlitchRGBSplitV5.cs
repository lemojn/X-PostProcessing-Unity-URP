using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "RGB颜色分离V5 (RGB Split V5)")]
    public class GlitchRGBSplitV5 : VolumeSettingBase
    {
        public override bool IsActive() => Amplitude.value > 0;
        public FloatParameter Amplitude = new ClampedFloatParameter(0f, 0f, 5f);
        public FloatParameter Speed = new ClampedFloatParameter(0.1f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 120)]
    public sealed class GlitchRGBSplitV5Renderer : VolumeRendererBase<GlitchRGBSplitV5>
    {
        public override string ProfilerTag => "Glitch-GlitchRGBSplitV5";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/RGBSplitV5";

        private Texture2D m_NoiseTex;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_NoiseTex == null)
            {
                m_NoiseTex = Resources.Load("Glitch_Noise256") as Texture2D;
                m_NoiseTex.name = "Glitch_Noise256";
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_NoiseTex != null)
            {
                Resources.UnloadAsset(m_NoiseTex);
                m_NoiseTex = null;
            }
        }

        static class ShaderIDs
        {
            internal static readonly int NoiseTex = Shader.PropertyToID("_NoiseTex");
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.Amplitude.value, m_Settings.Speed.value));
            if (m_NoiseTex != null)
            {
                m_BlitMaterial.SetTexture(ShaderIDs.NoiseTex, m_NoiseTex);
            }
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}