using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.EdgeDetection + "Scharr")]
    public class Scharr : VolumeSettingBase
    {
        public override bool IsActive() => edgeWidth.value > 0;
        public FloatParameter edgeWidth = new ClampedFloatParameter(0, 0f, 5.0f);
        public ColorParameter edgeColor = new ColorParameter(Color.black, true, true, true);
        public FloatParameter backgroundFade = new ClampedFloatParameter(1, 0f, 1.0f);
        public ColorParameter backgroundColor = new ColorParameter(Color.white, true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.EdgeDetection + 50)]
    public class ScharrRenderer : VolumeRendererBase<Scharr>
    {
        public override string ProfilerTag => "EdgeDetection-Scharr";
        protected override string ShaderName => "Hidden/XPostProcessing/EdgeDetection/Scharr";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
            internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.edgeWidth.value, m_Settings.backgroundFade.value));
            m_BlitMaterial.SetColor(ShaderIDs.EdgeColor, m_Settings.edgeColor.value);
            m_BlitMaterial.SetColor(ShaderIDs.BackgroundColor, m_Settings.backgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}