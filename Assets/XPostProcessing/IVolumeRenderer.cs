using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    /// <summary>
    /// VolumeRenderer接口.
    /// </summary>
    public interface IVolumeRenderer
    {
        string ProfilerTag { get; }
        bool IsActive(ref RenderingData renderingData);
        void Init();
        void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData);
        void Dispose();
    }
}
