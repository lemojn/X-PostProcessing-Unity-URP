using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "光圈模糊V2 (Iris Blur V2)")]
    public class IrisBlurV2 : VolumeSettingBase
    {
        public override bool IsActive() => BlurRadius.value > 0;
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 3f);
        public IntParameter Iteration = new ClampedIntParameter(60, 8, 128);
        public FloatParameter centerOffsetX = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter centerOffsetY = new ClampedFloatParameter(0f, -1f, 1f);
        public FloatParameter AreaSize = new ClampedFloatParameter(8f, 0f, 50f);
        public BoolParameter showPreview = new BoolParameter(false);
    }

    [VolumeRendererPriority(VolumePriority.Blur + 110)]
    public sealed class IrisBlurV2Renderer : VolumeRendererBase<IrisBlurV2>
    {
        public override string ProfilerTag => "Blur-IrisBlurV2";
        protected override string ShaderName => "Hidden/XPostProcessing/Blur/IrisBlurV2";

        private Vector4 m_GoldenRot;

        public override void Init()
        {
            base.Init();

            // Precompute rotations
            float c = Mathf.Cos(2.39996323f);
            float s = Mathf.Sin(2.39996323f);
            m_GoldenRot = new(c, s, -s, c);
        }

        static class ShaderIDs
        {
            internal static readonly int GoldenRot = Shader.PropertyToID("_GoldenRot");
            internal static readonly int Gradient = Shader.PropertyToID("_Gradient");
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.GoldenRot, m_GoldenRot);
            m_BlitMaterial.SetVector(ShaderIDs.Gradient, new Vector3(m_Settings.centerOffsetX.value, m_Settings.centerOffsetY.value, m_Settings.AreaSize.value * 0.1f));
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(m_Settings.Iteration.value, m_Settings.BlurRadius.value, 1f / Screen.width, 1f / Screen.height));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, m_Settings.showPreview.value ? 1 : 0);
        }

    }
}