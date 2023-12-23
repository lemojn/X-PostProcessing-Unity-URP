using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ImageProcessing + "锐化V3 (Sharpen V3)")]
    public class SharpenV3 : VolumeSettingBase
    {
        public override bool IsActive() => Sharpness.value > 0;
        public FloatParameter Sharpness = new ClampedFloatParameter(0f, 0f, 5f);
    }

    [VolumeRendererPriority(VolumePriority.ImageProcessing + 30)]
    public class SharpenV3Renderer : VolumeRendererBase<SharpenV3>
    {
        public override string ProfilerTag => "ImageProcessing-SharpenV3";
        protected override string ShaderName => "Hidden/XPostProcessing/ImageProcessing/SharpenV3";

        static class ShaderIDs
        {
            internal static readonly int CentralFactor = Shader.PropertyToID("_CentralFactor");
            internal static readonly int SideFactor = Shader.PropertyToID("_SideFactor");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.CentralFactor, 1.0f + (3.2f * m_Settings.Sharpness.value));
            m_BlitMaterial.SetFloat(ShaderIDs.SideFactor, 0.8f * m_Settings.Sharpness.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}