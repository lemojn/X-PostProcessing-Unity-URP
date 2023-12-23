using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Environment + "上帝之光 (Light Shaft) Ps:效果不太行")]
    public class LightShaft : VolumeSettingBase
    {
        public override bool IsActive() => BloomIntensity.value > 0;
        public ClampedFloatParameter BloomIntensity = new ClampedFloatParameter(0f, 0f, 5f);
        public ClampedFloatParameter Threshold = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter OcclusionDepthRange = new ClampedFloatParameter(0.1f, 0f, 1f);
        public ColorParameter BloomTint = new ColorParameter(Color.white);
        //降采样 默认一次
        public ClampedIntParameter DownSample = new ClampedIntParameter(1, 0, 5);
        public ClampedFloatParameter Attenuation = new ClampedFloatParameter(1, 0, 10);
        public TransformParameter virtualLight = new TransformParameter(null);
    }

    [VolumeRendererPriority(VolumePriority.Environment + 20)]
    public class LightShaftRenderer : VolumeRendererBase<LightShaft>
    {
        public override string ProfilerTag => "Environment-LightShift";
        protected override string ShaderName => "Hidden/XPostProcessing/Environment/LightShift";

        private RTHandle m_BlurRT1;
        private RTHandle m_BlurRT2;
        private RTHandle m_LightShaftRT;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_BlurRT1 != null)
            {
                RTHandles.Release(m_BlurRT1);
                m_BlurRT1 = null;
            }
            if (m_BlurRT2 != null)
            {
                RTHandles.Release(m_BlurRT2);
                m_BlurRT2 = null;
            }
            if (m_LightShaftRT != null)
            {
                RTHandles.Release(m_LightShaftRT);
                m_LightShaftRT = null;
            }
        }

        static class ShaderIDs
        {
            internal static readonly string BlurRT1 = "_BlurRT1";
            internal static readonly string BlurRT2 = "_BlurRT2";
            internal static readonly string LightShaftRT = "_LightShaftRT";
            internal static readonly int BloomTintAndThreshold = Shader.PropertyToID("_BloomTintAndThreshold");
            internal static readonly int LightShaftParameters = Shader.PropertyToID("_LightShaftParameters");
            internal static readonly int SampleDistance = Shader.PropertyToID("_SampleDistance");
            internal static readonly int BluredTexture = Shader.PropertyToID("_BluredTexture");
            internal static readonly int Attenuation = Shader.PropertyToID("_Attenuation");
        }

        protected override bool CheckActive(ref RenderingData renderingData)
        {
            //必须要有主光源
            int mainLightIndex = renderingData.lightData.mainLightIndex;
            if (mainLightIndex == -1)
                return false;

            //其次如果太背离光源，也不执行
            var mainLight = renderingData.lightData.visibleLights[mainLightIndex].light;
            var cameraDir = renderingData.cameraData.camera.transform.forward;
            var mainLightDir = -mainLight.transform.forward;
            if (Vector3.Dot(cameraDir, mainLightDir) < 0.4f)
                return false;

            return true;
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            var desc = GetDefaultColorRTDescriptor(ref renderingData, Screen.width >> m_Settings.DownSample.value, Screen.height >> m_Settings.DownSample.value);
            //获取主光源
            int mainLightIndex = renderingData.lightData.mainLightIndex;
            var mainLight = renderingData.lightData.visibleLights[mainLightIndex].light;
            var camera = renderingData.cameraData.camera;
            //将光源坐标转化为屏幕坐标
            Vector3 sunPos;
            if (m_Settings.virtualLight.value != null)
            {
                Vector3 direction = -m_Settings.virtualLight.value.localToWorldMatrix.GetColumn(2);
                sunPos = camera.transform.position + direction * camera.farClipPlane;
            }
            else
            {
                sunPos = mainLight.transform.position + mainLight.transform.forward * camera.farClipPlane;
            }
            Vector3 sunScreenPos = camera.WorldToScreenPoint(sunPos);
            //第一步 降采样提取屏幕高亮区域 这一步和Bloom一致 是不是可以两个效果合并成一个
            RenderingUtils.ReAllocateIfNeeded(ref m_LightShaftRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.LightShaftRT);
            m_BlitMaterial.SetVector(ShaderIDs.BloomTintAndThreshold, new Vector4(mainLight.color.r, mainLight.color.g, mainLight.color.b, m_Settings.Threshold.value));
            m_BlitMaterial.SetVector(ShaderIDs.LightShaftParameters, new Vector4(m_Settings.OcclusionDepthRange.value, m_Settings.BloomIntensity.value, sunScreenPos.x / camera.pixelWidth, sunScreenPos.y / camera.pixelHeight));
            Blitter.BlitCameraTexture(cmd, source, m_LightShaftRT, m_BlitMaterial, 0);
            //第二步 使用径向模糊
            int sampleDistance = 25;
            RenderingUtils.ReAllocateIfNeeded(ref m_BlurRT1, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BlurRT1);
            RenderingUtils.ReAllocateIfNeeded(ref m_BlurRT2, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ShaderIDs.BlurRT2);
            //1
            m_BlitMaterial.SetInt(ShaderIDs.SampleDistance, sampleDistance);
            Blitter.BlitCameraTexture(cmd, m_LightShaftRT, m_BlurRT1, m_BlitMaterial, 1);
            //2
            m_BlitMaterial.SetInt(ShaderIDs.SampleDistance, sampleDistance * 2);
            Blitter.BlitCameraTexture(cmd, m_BlurRT1, m_BlurRT2, m_BlitMaterial, 1);
            //最终Blit回去
            m_BlitMaterial.SetTexture(ShaderIDs.BluredTexture, m_BlurRT2);
            m_BlitMaterial.SetFloat(ShaderIDs.Attenuation, m_Settings.Attenuation.value);
            Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 2);
        }

    }
}