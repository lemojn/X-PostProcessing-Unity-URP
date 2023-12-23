using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Pixelate + "菱形像素化 (Pixelize Diamond)")]
    public class PixelizeDiamond : VolumeSettingBase
    {
        public override bool IsActive() => pixelSize.value > 0;
        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.0f, 1.0f);
    }

    [VolumeRendererPriority(VolumePriority.Pixelate + 20)]
    public class PixelizeDiamondRenderer : VolumeRendererBase<PixelizeDiamond>
    {
        public override string ProfilerTag => "Pixelate-PixelizeDiamond";
        protected override string ShaderName => "Hidden/XPostProcessing/Pixelate/PixelizeDiamond";

        static class ShaderIDs
        {
            internal static readonly int PixelSize = Shader.PropertyToID("_PixelSize");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.PixelSize, m_Settings.pixelSize.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}