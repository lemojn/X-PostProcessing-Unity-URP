using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.ColorAdjustment + "颜色替换V2 (Color Replace V2)")]
    public class ColorReplaceV2 : VolumeSettingBase
    {
        public override bool IsActive() => range.value > 0;
        public ClampedFloatParameter range = new(0, 0, 1);
        public ClampedFloatParameter fuzziness = new(0.5f, 0, 1);
        public ClampedFloatParameter gridentSpeed = new(0.5f, 0, 100);
        public GradientParameter fromGradientColor = new(null);
        public GradientParameter toGradientColor = new(null);
    }

    [VolumeRendererPriority(VolumePriority.ColorAdjustment + 130)]
    public class ColorReplaceV2Renderer : VolumeRendererBase<ColorReplaceV2>
    {
        public override string ProfilerTag => "ColorAdjustment-ColorReplaceV2";
        protected override string ShaderName => "Hidden/XPostProcessing/ColorAdjustment/ColorReplaceV2";

        private float m_TimeX = 1.0f;

        static class ShaderIDs
        {
            internal static readonly int FromColor = Shader.PropertyToID("_FromColor");
            internal static readonly int ToColor = Shader.PropertyToID("_ToColor");
            internal static readonly int Range = Shader.PropertyToID("_Range");
            internal static readonly int Fuzziness = Shader.PropertyToID("_Fuzziness");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_TimeX += Time.deltaTime * m_Settings.gridentSpeed.value;
            if (m_TimeX > 100)
            {
                m_TimeX = 0;
            }
            if (m_Settings.fromGradientColor.value != null)
            {
                m_BlitMaterial.SetColor(ShaderIDs.FromColor, m_Settings.fromGradientColor.value.Evaluate(m_TimeX * 0.01f));
            }
            if (m_Settings.toGradientColor.value != null)
            {
                m_BlitMaterial.SetColor(ShaderIDs.ToColor, m_Settings.toGradientColor.value.Evaluate(m_TimeX * 0.01f));
            }
            m_BlitMaterial.SetFloat(ShaderIDs.Range, m_Settings.range.value);
            m_BlitMaterial.SetFloat(ShaderIDs.Fuzziness, m_Settings.fuzziness.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}