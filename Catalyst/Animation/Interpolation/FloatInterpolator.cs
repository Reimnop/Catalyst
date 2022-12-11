using Catalyst.Util;

namespace Catalyst.Animation.Interpolation;

public class FloatInterpolator : Interpolator<float, float>
{
    public override float Interpolate(float first, float second, float factor)
    {
        return FastMathUtils.Lerp(first, second, factor);
    }
}