using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Glitch + "数字条纹故障 (Digital Stripe Glitch)")]
    public class GlitchDigitalStripe : VolumeSettingBase
    {
        public override bool IsActive() => intensity.value > 0;
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedIntParameter frequncy = new ClampedIntParameter(3, 1, 10);
        public ClampedFloatParameter stripeLength = new ClampedFloatParameter(0.89f, 0f, 0.99f);
        public ClampedIntParameter noiseTextureWidth = new ClampedIntParameter(20, 8, 256);
        public ClampedIntParameter noiseTextureHeight = new ClampedIntParameter(20, 8, 256);
        public BoolParameter needStripColorAdjust = new BoolParameter(false);
        public ColorParameter StripColorAdjustColor = new ColorParameter(new Color(0.1f, 0.1f, 0.1f), true, true, true);
        public ClampedFloatParameter StripColorAdjustIndensity = new ClampedFloatParameter(2, 0, 10);
    }

    [VolumeRendererPriority(VolumePriority.Glitch + 20)]
    public class GlitchDigitalStripeRenderer : VolumeRendererBase<GlitchDigitalStripe>
    {
        public override string ProfilerTag => "Glitch-GlitchDigitalStripe";
        protected override string ShaderName => "Hidden/XPostProcessing/Glitch/DigitalStripe";

        Texture2D m_NoiseTex;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_NoiseTex != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(m_NoiseTex);
                }
                else
                {
                    Object.DestroyImmediate(m_NoiseTex);
                }
                m_NoiseTex = null;
            }
        }

        static class ShaderIDs
        {
            internal static readonly int Indensity = Shader.PropertyToID("_Indensity");
            internal static readonly int NoiseTex = Shader.PropertyToID("_NoiseTex");
            internal static readonly int StripColorAdjustColor = Shader.PropertyToID("_StripColorAdjustColor");
            internal static readonly int StripColorAdjustIndensity = Shader.PropertyToID("_StripColorAdjustIndensity");
        }

        private void UpdateNoiseTexture(int frame, int noiseTextureWidth, int noiseTextureHeight, float stripLength)
        {
            int frameCount = Time.frameCount;
            if (frameCount % frame != 0)
            {
                return;
            }

            if (m_NoiseTex != null && (m_NoiseTex.width != noiseTextureWidth || m_NoiseTex.height != noiseTextureHeight))
            {
                m_NoiseTex.Reinitialize(noiseTextureWidth, noiseTextureHeight);
            }
            if (m_NoiseTex == null)
            {
                m_NoiseTex = new Texture2D(noiseTextureWidth, noiseTextureHeight, TextureFormat.ARGB32, false)
                {
                    name = "NoiseTex",
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Point
                };
            }

            Color32 color = VolumeUtility.RandomColor();
            for (int y = 0; y < m_NoiseTex.height; y++)
            {
                for (int x = 0; x < m_NoiseTex.width; x++)
                {
                    //随机值若大于给定strip随机阈值，重新随机颜色
                    if (Random.value > stripLength)
                        color = VolumeUtility.RandomColor();
                    //设置贴图像素值
                    m_NoiseTex.SetPixel(x, y, color);
                }
            }
            m_NoiseTex.Apply();
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            UpdateNoiseTexture(m_Settings.frequncy.value, m_Settings.noiseTextureWidth.value, m_Settings.noiseTextureHeight.value, m_Settings.stripeLength.value);
            m_BlitMaterial.SetFloat(ShaderIDs.Indensity, m_Settings.intensity.value);
            if (m_NoiseTex != null)
            {
                m_BlitMaterial.SetTexture(ShaderIDs.NoiseTex, m_NoiseTex);
            }
            if (m_Settings.needStripColorAdjust == true)
            {
                m_BlitMaterial.EnableKeyword("NEED_TRASH_FRAME");
                m_BlitMaterial.SetColor(ShaderIDs.StripColorAdjustColor, m_Settings.StripColorAdjustColor.value);
                m_BlitMaterial.SetFloat(ShaderIDs.StripColorAdjustIndensity, m_Settings.StripColorAdjustIndensity.value);
            }
            else
            {
                m_BlitMaterial.DisableKeyword("NEED_TRASH_FRAME");
            }
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}