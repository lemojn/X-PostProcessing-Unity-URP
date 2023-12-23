using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "方向模糊 (Directional Blur)")]
    public class DirectionalBlur : VolumeSettingBase
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 1f);
        public IntParameter Iteration = new ClampedIntParameter(10, 2, 30);
        public FloatParameter Angle = new ClampedFloatParameter(0.5f, 0f, 6f);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1f, 1f, 10f);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 150)]
    public class DirectionalBlurRenderer : VolumeRendererBase<DirectionalBlur>
    {
        public override string ProfilerTag => "Blur-DirectionalBlur";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/DirectionalBlur";

        private RTHandle m_BufferRT1;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_BufferRT1 != null)
            {
                RTHandles.Release(m_BufferRT1);
                m_BufferRT1 = null;
            }
        }

        static class ShaderIDs
        {
            internal static readonly string BufferRT1 = "_BufferRT1";
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            float sinVal = Mathf.Sin(m_Settings.Angle.value) * m_Settings.BlurRadius.value * 0.05f / m_Settings.Iteration.value;
            float cosVal = Mathf.Cos(m_Settings.Angle.value) * m_Settings.BlurRadius.value * 0.05f / m_Settings.Iteration.value;
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector3(m_Settings.Iteration.value, sinVal, cosVal));

            if (m_Settings.RTDownScaling.value > 1)
            {
                var desc = GetDefaultColorRTDescriptor(ref renderingData, (int)(Screen.width / m_Settings.RTDownScaling.value), (int)(Screen.height / m_Settings.RTDownScaling.value));
                desc.colorFormat = RenderTextureFormat.ARGB32;
                desc.sRGB = true;
                RenderingUtils.ReAllocateIfNeeded(ref m_BufferRT1, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BufferRT1);
                Blitter.BlitCameraTexture(cmd, source, m_BufferRT1);
                Blitter.BlitCameraTexture(cmd, m_BufferRT1, target, m_BlitMaterial, 0);
            }
            else
            {
                Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
            }
        }

    }
}