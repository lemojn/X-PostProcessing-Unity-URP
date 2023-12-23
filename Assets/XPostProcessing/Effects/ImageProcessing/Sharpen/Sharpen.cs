using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ImageProcessing + "锐化 (Sharpen)")]
    public class Sharpen : VolumeSettingBase
    {
        public override bool IsActive() => Strength.value > 0;
        public FloatParameter Strength = new ClampedFloatParameter(0f, 0f, 5f);
        public FloatParameter Threshold = new ClampedFloatParameter(0.1f, 0f, 1);
    }

    [VolumeRendererPriority(VolumePriority.ImageProcessing + 10)]
    public class SharpenRenderer : VolumeRendererBase<Sharpen>
    {
        public override string ProfilerTag => "ImageProcessing-Sharpen";
        protected override string ShaderName => "Hidden/XPostProcessing/ImageProcessing/Sharpen";

        static class ShaderIDs
        {
            internal static readonly int Strength = Shader.PropertyToID("_Strength");
            internal static readonly int Threshold = Shader.PropertyToID("_Threshold");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.Strength, m_Settings.Strength.value);
            m_BlitMaterial.SetFloat(ShaderIDs.Threshold, m_Settings.Threshold.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}