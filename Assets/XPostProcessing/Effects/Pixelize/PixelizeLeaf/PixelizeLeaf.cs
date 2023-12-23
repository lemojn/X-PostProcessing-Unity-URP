using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Pixelate + "叶子像素化 (Pixelize Leaf)")]
    public class PixelizeLeaf : VolumeSettingBase
    {
        public override bool IsActive() => pixelSize.value > 0;
        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.0f, 1.0f);
        public BoolParameter useAutoScreenRatio = new BoolParameter(false);
        public FloatParameter pixelRatio = new ClampedFloatParameter(1f, 0.2f, 5.0f);
        public FloatParameter pixelScaleX = new ClampedFloatParameter(1f, 0.2f, 5.0f);
        public FloatParameter pixelScaleY = new ClampedFloatParameter(1f, 0.2f, 5.0f);
    }

    [VolumeRendererPriority(VolumePriority.Pixelate + 50)]
    public class PixelizeLeafRenderer : VolumeRendererBase<PixelizeLeaf>
    {
        public override string ProfilerTag => "Pixelate-PixelizeLeaf";
        protected override string ShaderName => "Hidden/XPostProcessing/Pixelate/PixelizeLeaf";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            float size = (1.01f - m_Settings.pixelSize.value) * 10f;
            float ratio = m_Settings.pixelRatio.value;
            if (m_Settings.useAutoScreenRatio.value)
            {
                ratio = (float)(Screen.width / (float)Screen.height);
                if (ratio == 0)
                    ratio = 1;
            }
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(size, ratio, m_Settings.pixelScaleX.value * 20, m_Settings.pixelScaleY.value * 20));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}