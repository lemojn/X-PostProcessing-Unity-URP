using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Environment + "云投影 (Cloud Shadow)")]
    public class CloudShadow : VolumeSettingBase
    {
        public override bool IsActive() => shadowStrength.value > 0;
        [Tooltip("阴影强度")]
        public ClampedFloatParameter shadowStrength = new ClampedFloatParameter(0, 0, 2);
        [Tooltip("阴影照射方式")]
        public BoolParameter shadowForLightDirection = new BoolParameter(false);
        public TextureParameter cloudTexture = new TextureParameter(null);
        public MinFloatParameter cloudScale = new MinFloatParameter(1, 0);
        public ColorParameter shadowColor = new ColorParameter(Color.black, false, false, true);
        [Tooltip("云采样最小值")]
        public ClampedFloatParameter shadowCutoffMin = new ClampedFloatParameter(0.5f, 0, 1);
        [Tooltip("云采样最大值")]
        public ClampedFloatParameter shadowCutoffMax = new ClampedFloatParameter(0.6f, 0, 1);
        [Tooltip("最大生效距离,阴影强度线性减弱")]
        public FloatParameter maxDistance = new FloatParameter(100);
        [Tooltip("云的移动方向和速度")]
        public Vector2Parameter windSpeedDirection = new Vector2Parameter(Vector2.one);
        [Tooltip("云层高度")]
        public MinIntParameter CloudHeight = new MinIntParameter(1000, 0);
    }

    [VolumeRendererPriority(VolumePriority.Environment + 10)]
    public class CloudShadowRenderer : VolumeRendererBase<CloudShadow>
    {
        public override string ProfilerTag => "Environment-CloudShadow";
        protected override string ShaderName => "Hidden/XPostProcessing/Environment/CloudShadow";

        static class ShaderIDs
        {
            internal static readonly int CloudShadowMode = Shader.PropertyToID("_CloudShadowMode");
            internal static readonly int CloudTex = Shader.PropertyToID("_CloudTex");
            internal static readonly int CloudShadowColor = Shader.PropertyToID("_CloudShadowColor");
            internal static readonly int CloudTiling = Shader.PropertyToID("_CloudTiling");
            internal static readonly int WindFactor = Shader.PropertyToID("_WindFactor");
            internal static readonly int CloudHeight = Shader.PropertyToID("_CloudHeight");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetInt(ShaderIDs.CloudShadowMode, m_Settings.shadowForLightDirection.value ? 1 : 0);
            m_BlitMaterial.SetTexture(ShaderIDs.CloudTex, m_Settings.cloudTexture.value);
            m_BlitMaterial.SetColor(ShaderIDs.CloudShadowColor, m_Settings.shadowColor.value);

            // cloud.
            m_BlitMaterial.SetVector(ShaderIDs.CloudTiling, new Vector4(m_Settings.cloudScale.value, m_Settings.cloudScale.value,
            m_Settings.shadowStrength.value, m_Settings.maxDistance.value));
            // wind.
            m_BlitMaterial.SetVector(ShaderIDs.WindFactor, new Vector4(m_Settings.windSpeedDirection.value.x, m_Settings.windSpeedDirection.value.y,
            m_Settings.shadowCutoffMin.value, m_Settings.shadowCutoffMax.value));

            m_BlitMaterial.SetInt(ShaderIDs.CloudHeight, m_Settings.CloudHeight.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}