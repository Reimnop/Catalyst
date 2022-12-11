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

        int index = Search(time);
        Keyframe<T> first = keyframes[index];
        Keyframe<T> second = keyframes[index + 1];

        Func<float, float> easeFunc = Ease.EaseLookup[(int) second.Easing];
        
        float t = FastMathUtils.InverseLerp(first.Time, second.Time, time);
        return interpolator.Interpolate(first.Value, second.Value, easeFunc(t));
    }
    
    // Binary search for the keyframe pair that contains the given time
    private int Search(float time)
    {
        int low = 0;
        int high = keyframes.Length - 1;

        while (low <= high)
        {
            int mid = (low + high) / 2;
            float midTime = keyframes[mid].Time;

            if (time < midTime)
            {
                high = mid - 1;
            }
            else if (time > midTime)
            {
                low = mid + 1;
            }
            else
            {
                return mid;
            }
        }

        return low - 1;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TResult ResultFromSingleKeyframe(Keyframe<T> keyframe)
    {
        return interpolator.Interpolate(keyframe.Value, keyframe.Value, 0.0f);
    }
}