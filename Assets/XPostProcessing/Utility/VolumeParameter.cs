using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    public enum Direction
    {
        Horizontal = 0,
        Vertical = 1,
    }

    public enum DirectionEX
    {
        Horizontal = 0,
        Vertical = 1,
        Horizontal_Vertical = 2,
    }

    public enum IntervalType
    {
        Infinite,
        Periodic,
        Random
    }

    public enum VignetteType
    {
        ClassicMode = 0,
        ColorMode = 1,
    }

    public enum IrisBlurQualityLevel
    {
        High_Quality = 0,
        Normal_Quality = 1,
    }

    public enum RadialBlurQuality
    {
        RadialBlur_4Tap_Fatest = 0,
        RadialBlur_6Tap = 1,
        RadialBlur_8Tap_Balance = 2,
        RadialBlur_10Tap = 3,
        RadialBlur_12Tap = 4,
        RadialBlur_20Tap_Quality = 5,
        RadialBlur_30Tap_Extreme = 6,
    }

    public enum TiltShiftBlurQualityLevel
    {
        High_Quality = 0,
        Normal_Quality = 1,
    }

    public enum BloomQuailtyType
    {
        Low,
        High,
    }

    public enum TonemappingType
    {
        None,
        GranTurismo,
        ACES,
    }

    [Serializable]
    public sealed class DirectionParameter : VolumeParameter<Direction> { public DirectionParameter(Direction value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class DirectionEXParameter : VolumeParameter<DirectionEX> { public DirectionEXParameter(DirectionEX value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class IntervalTypeParameter : VolumeParameter<IntervalType> { public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class VignetteTypeParameter : VolumeParameter<VignetteType> { public VignetteTypeParameter(VignetteType value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class IrisBlurQualityLevelParameter : VolumeParameter<IrisBlurQualityLevel> { public IrisBlurQualityLevelParameter(IrisBlurQualityLevel value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class RadialBlurQualityParameter : VolumeParameter<RadialBlurQuality> { public RadialBlurQualityParameter(RadialBlurQuality value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class TiltShiftBlurQualityLevelParameter : VolumeParameter<TiltShiftBlurQualityLevel> { public TiltShiftBlurQualityLevelParameter(TiltShiftBlurQualityLevel value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class BloomQuailtyParameter : VolumeParameter<BloomQuailtyType> { public BloomQuailtyParameter(BloomQuailtyType value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class TonemappingTypeParameter : VolumeParameter<TonemappingType> { public TonemappingTypeParameter(TonemappingType value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class GradientParameter : VolumeParameter<Gradient> { public GradientParameter(Gradient value, bool overrideState = false) : base(value, overrideState) { } }

    [Serializable]
    public sealed class TransformParameter : VolumeParameter<Transform> { public TransformParameter(Transform value, bool overrideState = false) : base(value, overrideState) { } }

}