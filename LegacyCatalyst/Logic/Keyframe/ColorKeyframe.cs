using System.Collections.Generic;
using Catalyst.Engine.Core.Animation;
using Catalyst.Engine.Core.Animation.Keyframe;
using UnityEngine;

namespace Catalyst.Logic.Keyframe;

public struct ColorKeyframe : IKeyframe<Color>
{
    public float Time { get; set; }
    public EaseFunction Ease { get; set; }
    public int Value { get; set; }
    
    public ColorKeyframe(float time, int value, EaseFunction ease)
    {
        Time = time;
        Value = value;
        Ease = ease;
    }

    public Color Interpolate(IKeyframe<Color> other, float time)
    {
        var theme = GameManager.inst.LiveTheme.objectColors;
        var second = (ColorKeyframe) other;
        var t = second.Ease(time);
        return new Color(
            Mathf.Lerp(theme[Value].r, theme[second.Value].r, t),
            Mathf.Lerp(theme[Value].g, theme[second.Value].g, t),
           Mathf.Lerp(theme[Value].b, theme[second.Value].b, t),
           Mathf.Lerp(theme[Value].a, theme[second.Value].a, t));
    }
}