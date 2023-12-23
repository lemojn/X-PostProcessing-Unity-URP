using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Pixelate + "LED像素化 (Pixelize Led)")]
    public class PixelizeLed : VolumeSettingBase
    {
        public override bool IsActive() => pixelSize.value > 0;
        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.0f, 1.0f);
        public FloatParameter ledRadius = new ClampedFloatParameter(1f, 0.01f, 1.0f);
        public ColorParameter BackgroundColor = new ColorParameter(Color.black, true, true, true);
        public BoolParameter useAutoScreenRatio = new BoolParameter(true);
        public FloatParameter pixelRatio = new ClampedFloatParameter(1f, 0.2f, 5.0f);
    }

    [VolumeRendererPriority(VolumePriority.Pixelate + 60)]
    public class PixelizeLedRenderer : VolumeRendererBase<PixelizeLed>
    {
        public override string ProfilerTag => "Pixelate-PixelizeLed";
        protected override string ShaderName => "Hidden/XPostProcessing/Pixelate/PixelizeLed";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            float size = (1.01f - m_Settings.pixelSize.value) * 300f;
            float ratio = m_Settings.pixelRatio.value;
            if (m_Settings.useAutoScreenRatio.value)
            {
                ratio = (float)(Screen.width / (float)Screen.height);
                if (ratio == 0)
                    ratio = 1;
            }
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(size, ratio, m_Settings.ledRadius.value));
            m_BlitMaterial.SetColor(ShaderIDs.BackgroundColor, m_Settings.BackgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}