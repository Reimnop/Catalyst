using System.Collections.Generic;
using Catalyst.Engine.Core.Animation;
using Catalyst.Engine.Core.Animation.Keyframe;
using UnityEngine;

namespace Catalyst.Logic.Keyframe;

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
        var otherCasted = (Vector2Keyframe) other;
        return Vector2.Lerp(Value, otherCasted.Value, Ease(time));
    }
}