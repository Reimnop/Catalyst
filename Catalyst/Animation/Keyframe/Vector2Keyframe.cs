using Catalyst.Util;
using UnityEngine;

namespace Catalyst.Animation.Keyframe;

/// <summary>
/// A keyframe that animates a Vector2 value.
/// </summary>
public struct Vector2Keyframe : IKeyframe<Vector2>
{
    public float Time { get; set; }
    public EaseFunction Ease { get; set; }
    public Vector2 Value { get; set; }
    
    public Vector2Keyframe(float time, Vector2 value, EaseFunction ease)
    {
        Time = time;
        Value = value;
        Ease = ease;
    }
    
    public Vector2 Interpolate(IKeyframe<Vector2> other, float time)
    {
        Vector2Keyframe second = (Vector2Keyframe) other;
        float t = second.Ease(time);
        return new Vector2(
            FastMathUtils.Lerp(Value.x, second.Value.x, t),
            FastMathUtils.Lerp(Value.y, second.Value.y, t));
    }
}