using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    /// <summary>
    /// VolumeSetting基类.
    /// </summary>
    [Serializable]
    public abstract class VolumeSettingBase : VolumeComponent, IPostProcessComponent
    {
        public abstract bool IsActive();
        public virtual bool IsTileCompatible() => false;
    }
}