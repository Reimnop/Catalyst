using Catalyst.Animation.Interpolation;

namespace Catalyst.Logic;

public static class Interpolators
{
    public static Vector2Interpolator Vector2Interpolator { get; } = new Vector2Interpolator();
    public static FloatInterpolator FloatInterpolator { get; } = new FloatInterpolator();
    public static ThemeInterpolator ThemeInterpolator { get; } = new ThemeInterpolator();
}