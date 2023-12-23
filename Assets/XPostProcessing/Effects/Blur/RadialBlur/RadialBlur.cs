using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "径向模糊 (Radial Blur)")]
    public class RadialBlur : VolumeSettingBase
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 1f);
        public IntParameter Iteration = new ClampedIntParameter(10, 2, 30);
        public FloatParameter RadialCenterX = new ClampedFloatParameter(0.5f, 0f, 1f);
        public FloatParameter RadialCenterY = new ClampedFloatParameter(0.5f, 0f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 130)]
    public class RadialBlurRenderer : VolumeRendererBase<RadialBlur>
    {
        public override string ProfilerTag => "Blur-RadialBlur";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/RadialBlur";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(m_Settings.BlurRadius.value * 0.02f, m_Settings.Iteration.value, m_Settings.RadialCenterX.value, m_Settings.RadialCenterY.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}