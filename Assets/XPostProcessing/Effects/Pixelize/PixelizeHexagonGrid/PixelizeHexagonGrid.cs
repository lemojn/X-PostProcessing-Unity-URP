using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Pixelate + "六边格像素化 (Pixelize Hexagon Grid)")]
    public class PixelizeHexagonGrid : VolumeSettingBase
    {
        public override bool IsActive() => pixelSize.value > 0;
        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.0f, 1.0f);
        public FloatParameter gridWidth = new ClampedFloatParameter(1.0f, 0.01f, 5.0f);
    }

    [VolumeRendererPriority(VolumePriority.Pixelate + 40)]
    public class PixelizeHexagonGridRenderer : VolumeRendererBase<PixelizeHexagonGrid>
    {
        public override string ProfilerTag => "Pixelate-PixelizeHexagonGrid";
        protected override string ShaderName => "Hidden/XPostProcessing/Pixelate/PixelizeHexagonGrid";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.pixelSize.value, m_Settings.gridWidth.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}