﻿using System.Collections.Generic;
using Catalyst.Engine.Core.Animation;
using Catalyst.Engine.Core.Animation.Keyframe;
using UnityEngine;

namespace Catalyst.Logic.Keyframe;

public struct FloatKeyframe : IKeyframe<float>
{
    public float Time { get; set; }
    public EaseFunction Ease { get; set; }
    public float Value { get; set; }
    
    public FloatKeyframe(float time, float value, EaseFunction ease)
    {
        Time = time;
        Value = value;
        Ease = ease;
    }

    public float Interpolate(IKeyframe<float> other, float time)
    {
        var otherCasted = (FloatKeyframe) other;
        return Mathf.Lerp(Value, otherCasted.Value, Ease(time));
    }
}