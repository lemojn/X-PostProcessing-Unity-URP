using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.EdgeDetection + "Scharr Neon V2")]
    public class ScharrNeonV2 : VolumeSettingBase
    {
        public override bool IsActive() => EdgeWidth.value > 0;
        public FloatParameter EdgeWidth = new ClampedFloatParameter(0f, 0.0f, 5.0f);
        public FloatParameter EdgeNeonFade = new ClampedFloatParameter(1f, 0.1f, 1.0f);
        public FloatParameter BackgroundFade = new ClampedFloatParameter(1f, 0f, 1f);
        public FloatParameter Brigtness = new ClampedFloatParameter(1f, 0.2f, 2.0f);
        public ColorParameter BackgroundColor = new ColorParameter(Color.black, true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.EdgeDetection + 70)]
    public class ScharrNeonV2Renderer : VolumeRendererBase<ScharrNeonV2>
    {
        public override string ProfilerTag => "EdgeDetection-ScharrNeonV2";
        protected override string ShaderName => "Hidden/XPostProcessing/EdgeDetection/ScharrNeonV2";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(m_Settings.EdgeWidth.value, m_Settings.EdgeNeonFade.value, m_Settings.Brigtness.value, m_Settings.BackgroundFade.value));
            m_BlitMaterial.SetColor(ShaderIDs.BackgroundColor, m_Settings.BackgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}