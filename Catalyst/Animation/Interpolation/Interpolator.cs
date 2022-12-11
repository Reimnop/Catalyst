namespace Catalyst.Animation.Interpolation;

public abstract class Interpolator<T, TResult>
{
    public abstract TResult Interpolate(T first, T second, float factor);
}