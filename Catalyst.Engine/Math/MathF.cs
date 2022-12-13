using System.Runtime.CompilerServices;

namespace Catalyst.Engine.Math;

/// <summary>
/// Common math functions.
/// </summary>
public static class MathF
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Pow(float x, float y) => (float) System.Math.Pow(x, y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin(float x) => (float) System.Math.Sin(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos(float x) => (float) System.Math.Cos(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tan(float x) => (float) System.Math.Tan(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asin(float x) => (float) System.Math.Asin(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acos(float x) => (float) System.Math.Acos(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan(float x) => (float) System.Math.Atan(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan2(float y, float x) => (float) System.Math.Atan2(y, x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(float x) => (float) System.Math.Sqrt(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Abs(float x) => (float) System.Math.Abs(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(float x, float y) => System.Math.Min(x, y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(float x, float y) => System.Math.Max(x, y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float x, float min, float max) => Min(Max(x, min), max);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(float x) => (float) System.Math.Round(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Floor(float x) => (float) System.Math.Floor(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Ceil(float x) => (float) System.Math.Ceiling(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float x, float y, float t) => x + (y - x) * t;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InverseLerp(float x, float y, float t) => (t - x) / (y - x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SmoothStep(float x, float y, float t) => Lerp(x, y, t * t * (3.0f - 2.0f * t));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InverseSmoothStep(float x, float y, float t) => InverseLerp(x, y, t * t * (3.0f - 2.0f * t));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToRadians(float degrees) => degrees * (float) System.Math.PI / 180.0f;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToDegrees(float radians) => radians * 180.0f / (float) System.Math.PI;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToNormalized(float value, float min, float max) => (value - min) / (max - min);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToUnnormalized(float value, float min, float max) => value * (max - min) + min;
}