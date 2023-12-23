using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    /// <summary>
    /// VolumeRenderer基类.
    /// </summary>
    /// <typeparam name="T">VolumeSetting类型.</typeparam>
    public abstract class VolumeRendererBase<T> : IVolumeRenderer where T : VolumeSettingBase
    {
        public abstract string ProfilerTag { get; }
        protected abstract string ShaderName { get; }

        protected T m_Settings;
        protected Material m_BlitMaterial;
        private bool m_IsEnable;

        public virtual void Init()
        {
            var shader = Shader.Find(ShaderName);
            if (shader == null)
            {
                Debug.LogError($"Cannot find the shader: {ShaderName}");
                return;
            }

            m_Settings = VolumeManager.instance.stack.GetComponent<T>();
            m_BlitMaterial = CoreUtils.CreateEngineMaterial(shader);
            SetEnable();
        }

        public bool IsActive(ref RenderingData renderingData)
        {
            if (m_Settings == null || m_BlitMaterial == null)
            {
                SetDisable();
                return false;
            }

            if (m_Settings.active && m_Settings.IsActive() && CheckActive(ref renderingData))
            {
                SetEnable();
                return true;
            }
            else
            {
                SetDisable();
                return false;
            }
        }

        private void SetEnable()
        {
            if (!m_IsEnable)
            {
                m_IsEnable = true;
                OnEnable();
            }
        }

        private void SetDisable()
        {
            if (m_IsEnable)
            {
                OnDisable();
                m_IsEnable = false;
            }
        }

        protected virtual bool CheckActive(ref RenderingData renderingData) => true;
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        public virtual void Dispose()
        {
            SetDisable();

            if (m_BlitMaterial != null)
            {
                CoreUtils.Destroy(m_BlitMaterial);
                m_BlitMaterial = null;
            }
            m_Settings = null;
        }

        public abstract void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData);

        protected RenderTextureDescriptor GetDefaultColorRTDescriptor(ref RenderingData renderingData, int width = 0, int height = 0)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            if (width > 0 && height > 0)
            {
                desc.width = width;
                desc.height = height;
            }
            desc.msaaSamples = 1;
            desc.depthBufferBits = 0;
            return desc;
        }

    }
}
