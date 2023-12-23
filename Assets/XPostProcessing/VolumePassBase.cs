using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    /// <summary>
    /// VolumePass基类.
    /// </summary>
    public abstract class VolumePassBase : ScriptableRenderPass
    {
        enum VolumeRendererState
        {
            WaitingToAdd,
            WaitingToRemove,
            ImmediatelyRemove,
            InExecution,
        }

        class VolumeRendererMark
        {
            internal VolumeRendererState state;
            internal IVolumeRenderer renderer;
        }

        protected abstract string PostProcessingTag { get; }
        private readonly ProfilingSampler m_PostProcessingProfiling;
        private readonly List<IVolumeRenderer> m_VolumeRenderers;
        private readonly Dictionary<Type, VolumeRendererMark> m_VolumeRendererMarkDict;
        private bool m_IsCheckMark;
        private RTHandle m_SourceRT;
        private RTHandle m_TempRT;
        private static int m_RTIndex = 0;
        private readonly string m_RTName = "_TempRT";

        public VolumePassBase()
        {
            //可能会重复，所以要用来自增长.
            m_RTName += m_RTIndex;
            m_RTIndex += 1;
            m_PostProcessingProfiling = new ProfilingSampler(PostProcessingTag);
            m_VolumeRenderers = new List<IVolumeRenderer>();
            m_VolumeRendererMarkDict = new Dictionary<Type, VolumeRendererMark>();
            OnInit();
        }

        protected abstract void OnInit();

        /// <summary>
        /// 添加程序集里的所有符合条件的后处理效果.
        /// </summary>
        /// <param name="assembly">程序集</param>
        public void AddEffects(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (!type.IsDefined(typeof(VolumeRendererPriority)))
                    continue;
                if (!typeof(IVolumeRenderer).IsAssignableFrom(type))
                {
                    Debug.LogError($"Type is not IVolumeRenderer: {type.FullName}");
                    continue;
                }
                if (m_VolumeRendererMarkDict.TryGetValue(type, out var mark))
                {
                    if (mark.state == VolumeRendererState.WaitingToRemove)
                    {
                        mark.state = VolumeRendererState.WaitingToAdd;
                        m_IsCheckMark = true;
                    }
                }
                else
                {
                    mark = new VolumeRendererMark() { state = VolumeRendererState.WaitingToAdd, renderer = Activator.CreateInstance(type) as IVolumeRenderer };
                    m_VolumeRendererMarkDict.Add(type, mark);
                    m_IsCheckMark = true;
                }
            }
        }

        /// <summary>
        /// 添加后处理效果.
        /// </summary>
        /// <typeparam name="T">后处理效果的渲染器类型.</typeparam>
        public void AddEffect<T>() where T : IVolumeRenderer
        {
            var type = typeof(T);
            if (!type.IsDefined(typeof(VolumeRendererPriority)))
            {
                Debug.LogError($"Type donot have VolumeRendererPriority: {type.FullName}");
                return;
            }
            if (m_VolumeRendererMarkDict.TryGetValue(type, out var mark))
            {
                if (mark.state == VolumeRendererState.WaitingToRemove)
                {
                    mark.state = VolumeRendererState.WaitingToAdd;
                    m_IsCheckMark = true;
                }
            }
            else
            {
                mark = new VolumeRendererMark() { state = VolumeRendererState.WaitingToAdd, renderer = Activator.CreateInstance(type) as IVolumeRenderer };
                m_VolumeRendererMarkDict.Add(type, mark);
                m_IsCheckMark = true;
            }
        }

        /// <summary>
        /// 删除后处理效果.
        /// </summary>
        /// <typeparam name="T">后处理效果的渲染器类型.</typeparam>
        public void RemoveEffect<T>() where T : IVolumeRenderer
        {
            var type = typeof(T);
            if (m_VolumeRendererMarkDict.TryGetValue(type, out var mark))
            {
                if (mark.state == VolumeRendererState.WaitingToAdd)
                {
                    mark.state = VolumeRendererState.ImmediatelyRemove;
                    m_VolumeRendererMarkDict.Remove(type);
                }
                else if (mark.state == VolumeRendererState.InExecution)
                {
                    mark.state = VolumeRendererState.WaitingToRemove;
                    m_IsCheckMark = true;
                }
            }
        }

        /// <summary>
        /// 重新设置所有后处理效果.
        /// </summary>
        public void ResetAllEffects()
        {
            foreach (var renderer in m_VolumeRenderers)
            {
                renderer.Dispose();
                renderer.Init();
            }
        }

        /// <summary>
        /// 配置VolumePass的一些设置.
        /// </summary>
        /// <param name="cameraColorTarget">摄像机的颜色渲染目标.</param>
        public void Setup(RTHandle cameraColorTarget)
        {
            m_SourceRT = cameraColorTarget;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);

            var desc = cameraTextureDescriptor;
            desc.msaaSamples = 1;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref m_TempRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_RTName);

            ConfigureTarget(m_SourceRT);
            ConfigureClear(ClearFlag.None, Color.white);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            cmd.Clear();

            using (new ProfilingScope(cmd, m_PostProcessingProfiling))
            {
                if (m_IsCheckMark)
                {
                    foreach (var mark in m_VolumeRendererMarkDict)
                    {
                        if (mark.Value.state == VolumeRendererState.WaitingToRemove)
                        {
                            mark.Value.state = VolumeRendererState.ImmediatelyRemove;
                            mark.Value.renderer.Dispose();
                            m_VolumeRenderers.Remove(mark.Value.renderer);
                            m_VolumeRendererMarkDict.Remove(mark.Key);
                        }
                        else if (mark.Value.state == VolumeRendererState.WaitingToAdd)
                        {
                            mark.Value.state = VolumeRendererState.InExecution;
                            mark.Value.renderer.Init();
                            m_VolumeRenderers.Add(mark.Value.renderer);
                        }
                    }
                    // 重新排序.
                    m_VolumeRenderers.Sort((a, b) =>
                    {
                        int aPriority = a.GetType().GetCustomAttribute<VolumeRendererPriority>().priority;
                        int bPriority = b.GetType().GetCustomAttribute<VolumeRendererPriority>().priority;
                        return aPriority <= bPriority ? -1 : 1;
                    });
                    m_IsCheckMark = false;
                }
                // 执行渲染器.
                int count = 0;
                foreach (var renderer in m_VolumeRenderers)
                {
                    if (!renderer.IsActive(ref renderingData))
                        continue;

                    cmd.BeginSample(renderer.ProfilerTag);
                    renderer.Render(cmd, m_SourceRT, m_TempRT, ref renderingData);
                    CoreUtils.Swap(ref m_SourceRT, ref m_TempRT);
                    count++;
                    cmd.EndSample(renderer.ProfilerTag);
                }
                if (count > 0 && count % 2 != 0)
                {
                    cmd.BeginSample("FinalBlit");
                    Blitter.BlitCameraTexture(cmd, m_SourceRT, m_TempRT);
                    CoreUtils.Swap(ref m_SourceRT, ref m_TempRT);
                    cmd.EndSample("FinalBlit");
                }
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            foreach (var renderer in m_VolumeRenderers)
            {
                renderer.Dispose();
            }
            m_VolumeRenderers.Clear();
            m_VolumeRendererMarkDict.Clear();
            RTHandles.Release(m_TempRT);
            m_TempRT = null;
            m_SourceRT = null;
        }

    }
}