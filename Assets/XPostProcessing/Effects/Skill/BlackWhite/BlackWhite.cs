using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Skill + "黑白闪 (Black White)")]
    public class BlackWhite : VolumeSettingBase
    {
        public override bool IsActive() => Enable.value;
        public BoolParameter Enable = new BoolParameter(false);
        public Vector2Parameter Center = new Vector2Parameter(new Vector2(0.5f, 0.5f));
        public ColorParameter TintColor = new ColorParameter(Color.white);
        public ClampedFloatParameter Threshold = new ClampedFloatParameter(0.51f, 0.51f, 0.99f);
        public TextureParameter NoiseTex = new TextureParameter(null);
        public ClampedFloatParameter TillingX = new ClampedFloatParameter(0.1f, 0, 20);
        public ClampedFloatParameter TillingY = new ClampedFloatParameter(5, 0, 20);
        public ClampedFloatParameter Speed = new ClampedFloatParameter(0.1f, -10, 10);
        public TextureParameter DissolveTex = new TextureParameter(null);
        public ClampedFloatParameter Change = new ClampedFloatParameter(0, 0, 1);
    }

    [VolumeRendererPriority(VolumePriority.Skill + 10)]
    public class BlackWhiteRenderer : VolumeRendererBase<BlackWhite>
    {
        public override string ProfilerTag => "Skill-BlackWhite";
        protected override string ShaderName => "Hidden/XPostProcessing/Skill/BlackWhite";

        static class ShaderIDs
        {
            public static readonly int NoiseTex = Shader.PropertyToID("_NoiseTex");
            public static readonly int DissolveTex = Shader.PropertyToID("_DissolveTex");
            public static readonly int Color = Shader.PropertyToID("_Color");
            public static readonly int Params1 = Shader.PropertyToID("_Params1");
            public static readonly int Params2 = Shader.PropertyToID("_Params2");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            m_BlitMaterial.SetTexture(ShaderIDs.NoiseTex, m_Settings.NoiseTex.value);
            m_BlitMaterial.SetTexture(ShaderIDs.DissolveTex, m_Settings.DissolveTex.value);
            m_BlitMaterial.SetColor(ShaderIDs.Color, m_Settings.TintColor.value);
            m_BlitMaterial.SetVector(ShaderIDs.Params1, new Vector4(m_Settings.Threshold.value, m_Settings.Center.value.x, m_Settings.Center.value.y, 0));
            m_BlitMaterial.SetVector(ShaderIDs.Params2, new Vector4(m_Settings.TillingX.value, m_Settings.TillingY.value, m_Settings.Speed.value, m_Settings.Change.value));
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 0);
        }

    }
}
