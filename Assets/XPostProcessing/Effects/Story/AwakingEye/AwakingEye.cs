using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Story + "眨眼苏醒 (Awaking Eye)")]
    public class AwakingEye : VolumeSettingBase
    {
        public override bool IsActive() => enable.value;
        public BoolParameter enable = new BoolParameter(false);
        [Tooltip("眨眼幅度")]
        public ClampedFloatParameter openEyeValue = new ClampedFloatParameter(0, 0, 1);
        [Tooltip("眼睛长度")]
        public ClampedFloatParameter openEyeLength = new ClampedFloatParameter(0, 0, 1);
    }

    [VolumeRendererPriority(VolumePriority.Story + 20)]
    public class AwakingEyeRenderer : VolumeRendererBase<AwakingEye>
    {
        public override string ProfilerTag => "Story-AwakingEye";
        protected override string ShaderName => "Hidden/XPostProcessing/Story/AwakingEye";

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(m_Settings.openEyeValue.value, m_Settings.openEyeLength.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}