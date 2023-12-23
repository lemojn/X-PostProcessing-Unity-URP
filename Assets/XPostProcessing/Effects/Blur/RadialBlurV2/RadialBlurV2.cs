using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "径向模糊V2 (Radial Blur V2)")]
    public class RadialBlurV2 : VolumeSettingBase
    {
        public override bool IsActive() => BlurRadius.value != 0;
        public RadialBlurQualityParameter QualityLevel = new(RadialBlurQuality.RadialBlur_8Tap_Balance);
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter RadialCenterX = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter RadialCenterY = new ClampedFloatParameter(0.5f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 140)]
    public class RadialBlurV2Renderer : VolumeRendererBase<RadialBlurV2>
    {
        public override string ProfilerTag => "Blur-RadialBlurV2";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/RadialBlurV2";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(m_Settings.BlurRadius.value * 0.02f, m_Settings.RadialCenterX.value, m_Settings.RadialCenterY.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, (int)m_Settings.QualityLevel.value);
        }

    }
}