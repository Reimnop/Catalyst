using System.Runtime.CompilerServices;

namespace Catalyst.Engine.Math;

/// <summary>
/// Represents a X, Y pair.
/// </summary>
public struct Vector2
{
    public static Vector2 Zero => new Vector2(0.0f);
    
    public static Vector2 One => new Vector2(1.0f);
    
    public float X { get; set; }
    public float Y { get; set; }
    
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }
    
    public Vector2(float value)
    {
        X = value;
        Y = value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator -(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator *(Vector2 a, float b)
    {
        return new Vector2(a.X * b, a.Y * b);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator *(float a, Vector2 b)
    {
        return new Vector2(a * b.X, a * b.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator /(Vector2 a, float b)
    {
        return new Vector2(a.X / b, a.Y / b);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator /(float a, Vector2 b)
    {
        return new Vector2(a / b.X, a / b.Y);
    }
    
    public static Vector2 operator -(Vector2 a)
    {
        return new Vector2(-a.X, -a.Y);
    }
    
    public static bool operator ==(Vector2 a, Vector2 b)
    {
        return a.X == b.X && a.Y == b.Y;
    }
    
    public static bool operator !=(Vector2 a, Vector2 b)
    {
        return a.X != b.X || a.Y != b.Y;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is Vector2)
        {
            return this == (Vector2)obj;
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }
    
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Length()
    {
        return MathF.Sqrt(X * X + Y * Y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float LengthSquared()
    {
        return X * X + Y * Y;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Normalize()
    {
        float length = Length();
        return new Vector2(X / length, Y / length);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return a + (b - a) * t;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Min(Vector2 a, Vector2 b)
    {
        return new Vector2(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Max(Vector2 a, Vector2 b)
    {
        return new Vector2(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Clamp(Vector2 a, Vector2 min, Vector2 max)
    {
        return Min(Max(a, min), max);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Reflect(Vector2 a, Vector2 normal)
    {
        return a - 2.0f * Dot(a, normal) * normal;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Abs(Vector2 a)
    {
        return new Vector2(MathF.Abs(a.X), MathF.Abs(a.Y));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Floor(Vector2 a)
    {
        return new Vector2(MathF.Floor(a.X), MathF.Floor(a.Y));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Ceil(Vector2 a)
    {
        return new Vector2(MathF.Ceil(a.X), MathF.Ceil(a.Y));
    }
}