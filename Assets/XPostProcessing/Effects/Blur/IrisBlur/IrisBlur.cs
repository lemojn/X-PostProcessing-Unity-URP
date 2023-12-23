using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "光圈模糊 (Iris Blur)")]
    public class IrisBlur : VolumeSettingBase
    {
        public override bool IsActive() => AreaSize.value > 0;
        public IrisBlurQualityLevelParameter QualityLevel = new(IrisBlurQualityLevel.Normal_Quality);
        public FloatParameter AreaSize = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter BlurRadius = new ClampedFloatParameter(1.0f, 0f, 1f);
        public IntParameter Iteration = new ClampedIntParameter(2, 1, 8);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1.0f, 1.0f, 2.0f);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 100)]
    public sealed class IrisBlurRenderer : VolumeRendererBase<IrisBlur>
    {
        public override string ProfilerTag => "Blur-IrisBlur";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/IrisBlur";

        private RTHandle m_BufferRT1;
        private RTHandle m_BufferRT2;
        private RTHandle m_TempRT1;
        private RTHandle m_TempRT2;
        private RTHandle m_TempFinalBlurRT;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_BufferRT1 != null)
            {
                RTHandles.Release(m_BufferRT1);
                m_BufferRT1 = null;
            }
            if (m_BufferRT2 != null)
            {
                RTHandles.Release(m_BufferRT2);
                m_BufferRT2 = null;
            }
        }

        static class ShaderIDs
        {
            internal static readonly string BufferRT1 = "_BufferRT1";
            internal static readonly string BufferRT2 = "_BufferRT2";
            internal static readonly int BlurredTex = Shader.PropertyToID("_BlurredTex");
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            var desc = GetDefaultColorRTDescriptor(ref renderingData, (int)(Screen.width / m_Settings.RTDownScaling.value), (int)(Screen.height / m_Settings.RTDownScaling.value));
            desc.colorFormat = RenderTextureFormat.ARGB32;
            desc.sRGB = true;

            if (m_Settings.Iteration == 1)
            {
                HandleOneBlitBlur(cmd, source, target, ref desc);
            }
            else
            {
                HandleMultipleIterationBlur(cmd, source, target, ref desc, m_Settings.Iteration.value);
            }
        }

        void HandleOneBlitBlur(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderTextureDescriptor desc)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(m_Settings.AreaSize.value, m_Settings.BlurRadius.value));

            RenderingUtils.ReAllocateIfNeeded(ref m_BufferRT1, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BufferRT1);
            Blitter.BlitCameraTexture(cmd, source, m_BufferRT1, m_BlitMaterial, (int)m_Settings.QualityLevel.value);
            m_BlitMaterial.SetTexture(ShaderIDs.BlurredTex, m_BufferRT1);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 2);
        }

        void HandleMultipleIterationBlur(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderTextureDescriptor desc, int iteration)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.AreaSize.value, m_Settings.BlurRadius.value));

            RenderingUtils.ReAllocateIfNeeded(ref m_BufferRT1, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BufferRT1);
            RenderingUtils.ReAllocateIfNeeded(ref m_BufferRT2, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BufferRT2);

            m_TempRT1 = source;
            m_TempRT2 = m_BufferRT1;
            m_TempFinalBlurRT = m_BufferRT1;
            for (int i = 0; i < iteration; i++)
            {
                Blitter.BlitCameraTexture(cmd, m_TempRT1, m_TempRT2, m_BlitMaterial, (int)m_Settings.QualityLevel.value);
                m_TempFinalBlurRT = m_TempRT2;
                m_TempRT1 = m_TempRT2;
                m_TempRT2 = (m_TempRT2 == m_BufferRT1) ? m_BufferRT2 : m_BufferRT1;
            }
            m_BlitMaterial.SetTexture(ShaderIDs.BlurredTex, m_TempFinalBlurRT);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 2);

            m_TempRT1 = null;
            m_TempRT2 = null;
            m_TempFinalBlurRT = null;
        }

    }
}