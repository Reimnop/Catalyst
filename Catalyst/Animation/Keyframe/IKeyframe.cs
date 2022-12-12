using System;

namespace Catalyst.Animation.Keyframe;

/// <summary>
/// A keyframe.
/// </summary>
/// <typeparam name="T">Output type of the keyframe.</typeparam>
public interface IKeyframe<T>
{
    public float Time { get; set; }

    public T Interpolate(IKeyframe<T> other, float time);
}