using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "白平衡 (White Balance)")]
    public class WhiteBalance : VolumeSettingBase
    {
        public override bool IsActive() => temperature.value != 0;
        public ClampedFloatParameter temperature = new(0f, -1f, 1f);
        public ClampedFloatParameter tint = new(0f, -1f, 1f);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 110)]
    public class WhiteBalanceRenderer : VolumeRendererBase<WhiteBalance>
    {
        public override string ProfilerTag => "ColorAdjustment-WhiteBalance";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/WhiteBalance";

        static class ShaderIDs
        {
            internal static readonly int Temperature = Shader.PropertyToID("_Temperature");
            internal static readonly int Tint = Shader.PropertyToID("_Tint");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.Temperature, m_Settings.temperature.value);
            m_BlitMaterial.SetFloat(ShaderIDs.Tint, m_Settings.tint.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}