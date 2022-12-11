using System.Runtime.CompilerServices;

namespace Catalyst.Util;

public static class FastMathUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float first, float second, float factor)
    {
        return (second - first) * factor + first;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InverseLerp(float first, float second, float value)
    {
        return (value - first) / (second - first);
    }
}