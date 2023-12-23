using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Vignette + "快速渐晕 (Rapid Vignette)")]
    public class RapidVignette : VolumeSettingBase
    {
        public override bool IsActive() => vignetteIndensity.value > 0;
        public VignetteTypeParameter vignetteType = new VignetteTypeParameter(VignetteType.ClassicMode);
        public FloatParameter vignetteIndensity = new ClampedFloatParameter(0f, 0f, 5f);
        public Vector2Parameter vignetteCenter = new Vector2Parameter(new Vector2(0.5f, 0.5f));
        public ColorParameter vignetteColor = new ColorParameter(new Color(0.1f, 0.8f, 1.0f), true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.Vignette + 30)]
    public class RapidVignetteRenderer : VolumeRendererBase<RapidVignette>
    {
        public override string ProfilerTag => "Vignette-RapidVignette";
        protected override string ShaderName => "Hidden/XPostProcessing/Vignette/RapidVignette";

        static class ShaderIDs
        {
            internal static readonly int VignetteIndensity = Shader.PropertyToID("_VignetteIndensity");
            internal static readonly int VignetteCenter = Shader.PropertyToID("_VignetteCenter");
            internal static readonly int VignetteColor = Shader.PropertyToID("_VignetteColor");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.VignetteIndensity, m_Settings.vignetteIndensity.value);
            m_BlitMaterial.SetVector(ShaderIDs.VignetteCenter, m_Settings.vignetteCenter.value);
            if (m_Settings.vignetteType.value == VignetteType.ColorMode)
            {
                m_BlitMaterial.SetVector(ShaderIDs.VignetteColor, m_Settings.vignetteColor.value);
            }
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.vignetteType.value);
        }

    }
}