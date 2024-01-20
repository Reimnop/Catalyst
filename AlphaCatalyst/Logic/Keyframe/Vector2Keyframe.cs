using Catalyst.Engine.Core.Animation.Keyframe;
using UnityEngine;

namespace Catalyst.Logic.Keyframe;

public struct Vector2Keyframe : IKeyframe<Vector2>
{
    public float Time { get; set; }
    public Engine.Core.Animation.EaseFunction Ease { get; set; }
    public Vector2 Value { get; set; }
    
    public Vector2Keyframe(float time, Vector2 value, Engine.Core.Animation.EaseFunction ease)
    {
        Time = time;
        Value = value;
        Ease = ease;
    }

    public Vector2 Interpolate(IKeyframe<Vector2> other, float time)
    {
        var otherCasted = (Vector2Keyframe) other;
        return Vector2.LerpUnclamped(Value, otherCasted.Value, otherCasted.Ease(time));
    }
}