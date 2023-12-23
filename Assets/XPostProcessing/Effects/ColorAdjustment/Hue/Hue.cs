using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "色相偏移 (Hue)")]
    public class Hue : VolumeSettingBase
    {
        public override bool IsActive() => HueDegree.value != 0;
        public FloatParameter HueDegree = new ClampedFloatParameter(0f, -180f, 180f);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 60)]
    public class HueRenderer : VolumeRendererBase<Hue>
    {
        public override string ProfilerTag => "ColorAdjustment-Hue";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/Hue";

        static class ShaderIDs
        {
            internal static readonly int HueDegree = Shader.PropertyToID("_HueDegree");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.HueDegree, m_Settings.HueDegree.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}