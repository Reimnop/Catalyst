using Catalyst.Engine.Math;

namespace Catalyst.Engine.Core.Animation.Keyframe;

/// <summary>
/// A keyframe that animates a float value.
/// </summary>
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
        FloatKeyframe second = (FloatKeyframe) other;
        return MathF.Lerp(Value, second.Value, second.Ease(time));
    }
}