using Catalyst.Engine.Core.Animation;
using Catalyst.Engine.Core.Animation.Keyframe;
using Catalyst.Engine.Math;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace Catalyst.Logic;

/// <summary>
/// A keyframe that animates a theme color value.
/// </summary>
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
        List<Color> theme = GameManager.inst.LiveTheme.objectColors;
        ColorKeyframe second = (ColorKeyframe) other;
        
        float t = second.Ease(time);
        return new Color(
            MathF.Lerp(theme[Value].r, theme[second.Value].r, t),
            MathF.Lerp(theme[Value].g, theme[second.Value].g, t),
            MathF.Lerp(theme[Value].b, theme[second.Value].b, t),
            MathF.Lerp(theme[Value].a, theme[second.Value].a, t));
    }
}