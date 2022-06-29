using System;

namespace Catalyst.Animation;

/// <summary>
/// The keyframe struct.
/// </summary>
/// <typeparam name="T">The keyframe value type.</typeparam>
public struct Keyframe<T> : IComparable<Keyframe<T>>
{
    public float Time { get; set; }
    public T Value { get; set; }
    public Easing Easing { get; set; }

    public Keyframe(float time, T value, Easing easing)
    {
        Time = time;
        Value = value;
        Easing = easing;
    }

    public int CompareTo(Keyframe<T> other)
    {
        return Time.CompareTo(other.Time);
    }
}