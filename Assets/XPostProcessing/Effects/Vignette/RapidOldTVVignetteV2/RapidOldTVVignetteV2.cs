using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Vignette + "老式TV渐晕V2 (Rapid Old TV Vignette V2)")]
    public class RapidOldTVVignetteV2 : VolumeSettingBase
    {
        public override bool IsActive() => vignetteSize.value > 0;
        public VignetteTypeParameter vignetteType = new VignetteTypeParameter(VignetteType.ClassicMode);
        public FloatParameter vignetteSize = new ClampedFloatParameter(0, 0, 5000);
        public FloatParameter sizeOffset = new ClampedFloatParameter(0.2f, 0f, 1f);
        public ColorParameter vignetteColor = new ColorParameter(new Color(0.1f, 0.8f, 1.0f), true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.Vignette + 20)]
    public class RapidOldTVVignetteV2Renderer : VolumeRendererBase<RapidOldTVVignetteV2>
    {
        public override string ProfilerTag => "Vignette-RapidOldTVVignetteV2";
        protected override string ShaderName => "Hidden/XPostProcessing/Vignette/RapidOldTVVignetteV2";

        static class ShaderIDs
        {
            internal static readonly int VignetteSize = Shader.PropertyToID("_VignetteSize");
            internal static readonly int SizeOffset = Shader.PropertyToID("_SizeOffset");
            internal static readonly int VignetteColor = Shader.PropertyToID("_VignetteColor");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.VignetteSize, m_Settings.vignetteSize.value);
            m_BlitMaterial.SetFloat(ShaderIDs.SizeOffset, m_Settings.sizeOffset.value);
            if (m_Settings.vignetteType.value == VignetteType.ColorMode)
            {
                m_BlitMaterial.SetColor(ShaderIDs.VignetteColor, m_Settings.vignetteColor.value);
            }
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.vignetteType.value);
        }

    }
}