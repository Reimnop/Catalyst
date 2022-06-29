using Catalyst.Util;
using UnityEngine;

namespace Catalyst.Animation.Interpolation;

public class Vector2Interpolator : Interpolator<Vector2, Vector2>
{
    public override Vector2 Interpolate(Vector2 first, Vector2 second, float factor)
    {
        return new Vector2(
            FastMathUtils.Lerp(first.x, second.x, factor),
            FastMathUtils.Lerp(first.y, second.y, factor));
    }
}