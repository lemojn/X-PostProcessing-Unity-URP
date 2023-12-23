using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.EdgeDetection + "Roberts")]
    public class Roberts : VolumeSettingBase
    {
        public override bool IsActive() => edgeWidth.value > 0;
        public ClampedFloatParameter edgeWidth = new(0f, 0f, 5f);
        public ColorParameter edgeColor = new(new Color(0.0f, 0.0f, 0.0f, 1), true, true, true);
        public ClampedFloatParameter backgroundFade = new(0f, 0f, 1f);
        public ColorParameter backgroundColor = new(new Color(1.0f, 1.0f, 1.0f, 1), true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.EdgeDetection + 20)]
    public sealed class RobertsRenderer : VolumeRendererBase<Roberts>
    {
        public override string ProfilerTag => "EdgeDetection-Roberts";
        protected override string ShaderName => "Hidden/XPostProcessing/EdgeDetection/Roberts";

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