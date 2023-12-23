using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ImageProcessing + "锐化V2 (Sharpen V2)")]
    public class SharpenV2 : VolumeSettingBase
    {
        public override bool IsActive() => Sharpness.value > 0;
        public FloatParameter Sharpness = new ClampedFloatParameter(0f, 0f, 5f);
    }

    [VolumeRendererPriority(VolumePriority.ImageProcessing + 20)]
    public class SharpenV2Renderer : VolumeRendererBase<SharpenV2>
    {
        public override string ProfilerTag => "ImageProcessing-SharpenV2";
        protected override string ShaderName => "Hidden/XPostProcessing/ImageProcessing/SharpenV2";

        static class ShaderIDs
        {
            internal static readonly int Sharpness = Shader.PropertyToID("_Sharpness");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.Sharpness, m_Settings.Sharpness.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}