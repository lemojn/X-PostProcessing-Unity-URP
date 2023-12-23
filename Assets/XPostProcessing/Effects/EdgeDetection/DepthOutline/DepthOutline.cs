using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.EdgeDetection + "Depth Outline")]
    public class DepthOutline : VolumeSettingBase
    {
        public override bool IsActive() => edgeWidth.value > 0;
        public ClampedFloatParameter edgeWidth = new(0, 0, 5);
        public ClampedFloatParameter threshold = new(0.1f, 0, 10);
    }

    [VolumeRendererPriority(VolumePriority.EdgeDetection + 10)]
    public class DepthOutlineRenderer : VolumeRendererBase<DepthOutline>
    {
        public override string ProfilerTag => "EdgeDetection-DepthOutline";
        protected override string ShaderName => "Hidden/XPostProcessing/EdgeDetection/DepthOutline";

        static class ShaderIDs
        {
            public static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.edgeWidth.value, m_Settings.threshold.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}