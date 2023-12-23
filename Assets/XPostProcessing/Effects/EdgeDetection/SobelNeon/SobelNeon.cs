using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.EdgeDetection + "Sobel Neon")]
    public class SobelNeon : VolumeSettingBase
    {
        public override bool IsActive() => EdgeWidth.value > 0;
        public FloatParameter EdgeWidth = new ClampedFloatParameter(0f, 0.0f, 5.0f);
        public FloatParameter BackgroundFade = new ClampedFloatParameter(1f, 0f, 1f);
        public FloatParameter Brigtness = new ClampedFloatParameter(1f, 0.2f, 2.0f);
        public ColorParameter BackgroundColor = new ColorParameter(Color.black, true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.EdgeDetection + 90)]
    public class SobelNeonRenderer : VolumeRendererBase<SobelNeon>
    {
        public override string ProfilerTag => "EdgeDetection-SobelNeon";
        protected override string ShaderName => "Hidden/XPostProcessing/EdgeDetection/SobelNeon";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(m_Settings.EdgeWidth.value, m_Settings.Brigtness.value, m_Settings.BackgroundFade.value));
            m_BlitMaterial.SetColor(ShaderIDs.BackgroundColor, m_Settings.BackgroundColor.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}