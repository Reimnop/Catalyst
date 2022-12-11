using System.Collections.Generic;
using Catalyst.Util;
using UnityEngine;

namespace Catalyst.Animation.Interpolation;

public class ThemeInterpolator : Interpolator<int, Color>
{
    public override Color Interpolate(int first, int second, float factor)
    {
        List<Color> theme = GameManager.inst.LiveTheme.objectColors;
        return new Color(
            FastMathUtils.Lerp(theme[first].r, theme[second].r, factor),
            FastMathUtils.Lerp(theme[first].g, theme[second].g, factor),
            FastMathUtils.Lerp(theme[first].b, theme[second].b, factor),
            FastMathUtils.Lerp(theme[first].a, theme[second].a, factor));
    }
}