using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Pixelate + "六边形像素化 (Pixelize Hexagon)")]
    public class PixelizeHexagon : VolumeSettingBase
    {
        public override bool IsActive() => pixelSize.value > 0;
        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.0f, 1.0f);
        public BoolParameter useAutoScreenRatio = new BoolParameter(false);
        public FloatParameter pixelRatio = new ClampedFloatParameter(1f, 0.2f, 5.0f);
        public FloatParameter pixelScaleX = new ClampedFloatParameter(1f, 0.2f, 5.0f);
        public FloatParameter pixelScaleY = new ClampedFloatParameter(1f, 0.2f, 5.0f);
    }

    [VolumeRendererPriority(VolumePriority.Pixelate + 30)]
    public class PixelizeHexagonRenderer : VolumeRendererBase<PixelizeHexagon>
    {
        public override string ProfilerTag => "Pixelate-PixelizeHexagon";
        protected override string ShaderName => "Hidden/XPostProcessing/Pixelate/PixelizeHexagon";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            float size = m_Settings.pixelSize.value * 0.2f;
            float ratio = m_Settings.pixelRatio.value;
            if (m_Settings.useAutoScreenRatio.value)
            {
                ratio = (float)(Screen.width / (float)Screen.height);
                if (ratio == 0)
                    ratio = 1;
            }
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(size, ratio, m_Settings.pixelScaleX.value, m_Settings.pixelScaleY.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}