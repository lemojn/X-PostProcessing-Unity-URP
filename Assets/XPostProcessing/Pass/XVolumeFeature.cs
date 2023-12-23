using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [DisallowMultipleRendererFeature("X Post Processing")]
    public class XVolumeFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        [SerializeField]
        private Settings m_Settings;
        private VolumePassBase m_VolumePass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.postProcessEnabled)
                renderer.EnqueuePass(m_VolumePass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            base.SetupRenderPasses(renderer, renderingData);

            m_VolumePass.renderPassEvent = m_Settings.renderPassEvent;
            m_VolumePass.Setup(renderer.cameraColorTargetHandle);
        }

        public override void Create()
        {
            m_VolumePass ??= new XVolumePass();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            m_VolumePass?.Dispose();
            m_VolumePass = null;
        }

    }
}
