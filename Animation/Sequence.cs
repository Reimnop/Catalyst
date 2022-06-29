using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Catalyst.Animation.Interpolation;
using Catalyst.Util;

namespace Catalyst.Animation;

/// <summary>
/// Sequence class. Stores, manages and interpolates between keyframes.
/// </summary>
public class Sequence<T, TResult>
{
    private readonly Keyframe<T>[] keyframes;
    private readonly Interpolator<T, TResult> interpolator;

    private float lastTime = 0.0f;
    private int index = 0;

    public Sequence(IEnumerable<Keyframe<T>> keyframes, Interpolator<T, TResult> interpolator)
    {
        this.interpolator = interpolator;
        this.keyframes = keyframes.ToArray();
        Array.Sort(this.keyframes);
    }

    public Sequence<T, TResult> Copy()
    {
        return new Sequence<T, TResult>(keyframes, interpolator);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Interpolate(float time)
    {
        if (keyframes.Length == 0)
        {
            throw new NoKeyframeException("Cannot interpolate in an empty sequence!");
        }

        if (keyframes.Length == 1)
        {
            return ResultFromSingleKeyframe(keyframes[0]);
        }

        if (time < keyframes[0].Time)
        {
            return ResultFromSingleKeyframe(keyframes[0]);
        }
        
        if (time >= keyframes[keyframes.Length - 1].Time)
        {
            return ResultFromSingleKeyframe(keyframes[keyframes.Length - 1]);
        }

        int step = time >= lastTime ? 1 : -1;
        lastTime = time;

        while (!(time >= keyframes[index].Time && time < keyframes[index + 1].Time))
        {
            index += step;
        }

        Keyframe<T> first = keyframes[index];
        Keyframe<T> second = keyframes[index + 1];

        Func<float, float> easeFunc = Ease.EaseLookup[(int) second.Easing];
        
        float t = FastMathUtils.InverseLerp(first.Time, second.Time, time);
        return interpolator.Interpolate(first.Value, second.Value, easeFunc(t));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TResult ResultFromSingleKeyframe(Keyframe<T> keyframe)
    {
        return interpolator.Interpolate(keyframe.Value, keyframe.Value, 0.0f);
    }
}