using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "颜色替换 (Color Replace)")]
    public class ColorReplace : VolumeSettingBase
    {
        public override bool IsActive() => range.value > 0;
        public ClampedFloatParameter range = new(0, 0, 1);
        public ClampedFloatParameter fuzziness = new(0.5f, 0, 1);
        public ColorParameter fromColor = new(new Color(0.8f, 0, 0, 1), true, true, true);
        public ColorParameter toColor = new(new Color(0, 0.8f, 0, 1), true, true, true);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 120)]
    public class ColorReplaceRenderer : VolumeRendererBase<ColorReplace>
    {
        public override string ProfilerTag => "ColorAdjustment-ColorReplace";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/ColorReplace";

        static class ShaderIDs
        {
            internal static readonly int Range = Shader.PropertyToID("_Range");
            internal static readonly int Fuzziness = Shader.PropertyToID("_Fuzziness");
            internal static readonly int FromColor = Shader.PropertyToID("_FromColor");
            internal static readonly int ToColor = Shader.PropertyToID("_ToColor");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderIDs.Range, m_Settings.range.value);
            m_BlitMaterial.SetFloat(ShaderIDs.Fuzziness, m_Settings.fuzziness.value);
            m_BlitMaterial.SetColor(ShaderIDs.FromColor, m_Settings.fromColor.value);
            m_BlitMaterial.SetColor(ShaderIDs.ToColor, m_Settings.toColor.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}