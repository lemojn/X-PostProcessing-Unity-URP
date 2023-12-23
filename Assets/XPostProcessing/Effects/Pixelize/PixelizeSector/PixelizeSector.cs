using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Pixelate + "扇形像素化 (Pixelize Sector)")]
    public class PixelizeSector : VolumeSettingBase
    {
        public override bool IsActive() => pixelSize.value > 0;
        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.0f, 1.0f);
        public FloatParameter circleRadius = new ClampedFloatParameter(0.8f, 0.01f, 1.0f);
        public FloatParameter pixelIntervalX = new ClampedFloatParameter(1f, 0.2f, 5.0f);
        public FloatParameter pixelIntervalY = new ClampedFloatParameter(1f, 0.2f, 5.0f);
        public ColorParameter BackgroundColor = new ColorParameter(Color.black, true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.Pixelate + 80)]
    public class PixelizeSectorRenderer : VolumeRendererBase<PixelizeSector>
    {
        public override string ProfilerTag => "Pixelate-PixelizeSector";
        protected override string ShaderName => "Hidden/XPostProcessing/Pixelate/PixelizeSector";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Params2 = Shader.PropertyToID("_Params2");
            internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            float size = (1.01f - m_Settings.pixelSize.value) * 300f;
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(size, Screen.width * 2 / Screen.height * size / Mathf.Sqrt(3f), m_Settings.circleRadius.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params2, new Vector2(m_Settings.pixelIntervalX.value, m_Settings.pixelIntervalY.value));
            m_BlitMaterial.SetColor(ShaderIDs.BackgroundColor, m_Settings.BackgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}