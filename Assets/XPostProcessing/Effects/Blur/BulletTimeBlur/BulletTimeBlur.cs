using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Blur + "子弹时间模糊 (Bullet Time Blur)")]
    public class BulletTimeBlur : VolumeSettingBase
    {
        public override bool IsActive() => bulletBlurControl.value > 0;

        [Tooltip("特效控制值")]
        public ClampedFloatParameter bulletBlurControl = new(0, 0, 1);
        public ClampedFloatParameter bulletUnblurRadius = new(0.2f, 0, 1);
        public MinFloatParameter bulletBlurLength = new(0.55f, 0.1f);
        public Vector2Parameter bulletBlurCenterPoint = new(new Vector2(0.5f, 0.5f));
    }

    [VolumeRendererPriority(VolumePriority.Blur + 160)]
    public class BulletTimeBlurRenderer : VolumeRendererBase<BulletTimeBlur>
    {
        public override string ProfilerTag => "Blur-BulletTimeBlur";
        protected override string ShaderName => "Hidden/XPostProcessing/BulletTimeBlur";

        static class ShaderIDs
        {
            public static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(m_Settings.bulletBlurControl.value * m_Settings.bulletBlurLength.value, m_Settings.bulletUnblurRadius.value,
            m_Settings.bulletBlurCenterPoint.value.x, m_Settings.bulletBlurCenterPoint.value.y));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}