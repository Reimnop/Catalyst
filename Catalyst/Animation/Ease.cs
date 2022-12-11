using System;
using System.Collections.Generic;
using UnityEngine;

namespace Catalyst.Animation;

public enum Easing
{
	Linear,
	Instant,
	InSine,
	OutSine,
	InOutSine,
	InElastic,
	OutElastic,
	InOutElastic,
	InBack,
	OutBack,
	InOutBack,
	InBounce,
	OutBounce,
	InOutBounce,
	InQuad,
	OutQuad,
	InOutQuad,
	InCirc,
	OutCirc,
	InOutCirc,
	InExpo,
	OutExpo,
	InOutExpo
}

/// <summary>
/// Static class with useful easer functions that can be used by Tweens.
/// </summary>
public static class Ease
{
	public static readonly Func<float, float>[] EaseLookup =
	{
		Linear,
		Instant,
		SineIn,
		SineOut,
		SineInOut,
		ElasticIn,
		ElasticOut,
		ElasticInOut,
		BackIn,
		BackOut,
		BackInOut,
		BounceIn,
		BounceOut,
		BounceInOut,
		QuadIn,
		QuadOut,
		QuadInOut,
		CircIn,
		CircOut,
		CircInOut,
		ExpoIn,
		ExpoOut,
		ExpoInOut
	};
	
	public static readonly Dictionary<string, Easing> EaseStringLookup = new Dictionary<string, Easing>()
	{
		{ "Linear", Easing.Linear },
		{ "Instant", Easing.Instant },
		{ "InSine", Easing.InSine },
		{ "OutSine", Easing.OutSine },
		{ "InOutSine", Easing.InOutSine },
		{ "InElastic", Easing.InElastic },
		{ "OutElastic", Easing.OutElastic },
		{ "InOutElastic", Easing.InOutElastic },
		{ "InBack", Easing.InBack },
		{ "OutBack", Easing.OutBack },
		{ "InOutBack", Easing.InOutBack },
		{ "InBounce", Easing.InBounce },
		{ "OutBounce", Easing.OutBounce },
		{ "InOutBounce", Easing.InOutBounce },
		{ "InQuad", Easing.InQuad },
		{ "OutQuad", Easing.OutQuad },
		{ "InOutQuad", Easing.InOutQuad },
		{ "InCirc", Easing.InCirc },
		{ "OutCirc", Easing.OutCirc },
		{ "InOutCirc", Easing.InOutCirc },
		{ "InExpo", Easing.InExpo },
		{ "OutExpo", Easing.OutExpo },
		{ "InOutExpo", Easing.InOutExpo }
	};

	private const float PI = 3.14159265359f;
	private const float PI2 = PI / 2;
	private const float B1 = 1 / 2.75f;
	private const float B2 = 2 / 2.75f;
	private const float B3 = 1.5f / 2.75f;
	private const float B4 = 2.5f / 2.75f;
	private const float B5 = 2.25f / 2.75f;
	private const float B6 = 2.625f / 2.75f;

	/// <summary>
	/// Ease a value to its target and then back. Use this to wrap another easing function.
	/// </summary>
	public static Func<float, float> ToAndFro(Func<float, float> easer)
	{
		return t => ToAndFro(easer(t));
	}

	/// <summary>
	/// Ease a value to its target and then back.
	/// </summary>
	public static float ToAndFro(float t)
	{
		return t < 0.5f ? t * 2 : 1 + ((t - 0.5f) / 0.5f) * -1;
	}

	/// <summary>
	/// Linear.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float Linear(float t)
		=> t;

	/// <summary>
	/// Instant.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float Instant(float t)
	{
		if (t == 1.0f) return 1.0f;
		return 0.0f;
	}

	#region Sine

	/// <summary>
	/// Sine in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float SineIn(float t)
	{
		if (t == 1) return 1;
		return -Mathf.Cos(PI2 * t) + 1;
	}

	/// <summary>
	/// Sine out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float SineOut(float t)
	{
		return Mathf.Sin(PI2 * t);
	}

	/// <summary>
	/// Sine in and out
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float SineInOut(float t)
	{
		return -Mathf.Cos(PI * t) / 2 + 0.5f;
	}

	#endregion

	#region Elastic

	/// <summary>
	/// Elastic in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float ElasticIn(float t)
	{
		return (Mathf.Sin(13 * PI2 * t) * Mathf.Pow(2, 10 * (t - 1)));
	}

	/// <summary>
	/// Elastic out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float ElasticOut(float t)
	{
		if (t == 1) return 1;
		return (Mathf.Sin(-13 * PI2 * (t + 1)) * Mathf.Pow(2, -10 * t) + 1);
	}

	/// <summary>
	/// Elastic in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float ElasticInOut(float t)
	{
		if (t < 0.5)
		{
			return (0.5f * Mathf.Sin(13 * PI2 * (2 * t)) * Mathf.Pow(2, 10 * ((2 * t) - 1)));
		}

		return (0.5f * (Mathf.Sin(-13 * PI2 * ((2 * t - 1) + 1)) * Mathf.Pow(2, -10 * (2 * t - 1)) + 2));
	}

	#endregion

	#region Back

	/// <summary>
	/// Back in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float BackIn(float t)
	{
		return (t * t * (2.70158f * t - 1.70158f));
	}

	/// <summary>
	/// Back out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float BackOut(float t)
	{
		return (1 - (--t) * (t) * (-2.70158f * t - 1.70158f));
	}

	/// <summary>
	/// Back in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float BackInOut(float t)
	{
		t *= 2;
		if (t < 1) return (t * t * (2.70158f * t - 1.70158f) / 2);
		t--;
		return ((1 - (--t) * (t) * (-2.70158f * t - 1.70158f)) / 2 + .5f);
	}

	#endregion

	#region Bounce

	/// <summary>
	/// Bounce in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float BounceIn(float t)
	{
		t = 1 - t;
		if (t < B1) return (1 - 7.5625f * t * t);
		if (t < B2) return (1 - (7.5625f * (t - B3) * (t - B3) + .75f));
		if (t < B4) return (1 - (7.5625f * (t - B5) * (t - B5) + .9375f));
		return (1 - (7.5625f * (t - B6) * (t - B6) + .984375f));
	}

	/// <summary>
	/// Bounce out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float BounceOut(float t)
	{
		if (t < B1) return (7.5625f * t * t);
		if (t < B2) return (7.5625f * (t - B3) * (t - B3) + .75f);
		if (t < B4) return (7.5625f * (t - B5) * (t - B5) + .9375f);
		return (7.5625f * (t - B6) * (t - B6) + .984375f);
	}

	/// <summary>
	/// Bounce in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float BounceInOut(float t)
	{
		if (t < .5)
		{
			t = 1 - t * 2;
			if (t < B1) return ((1 - 7.5625f * t * t) / 2);
			if (t < B2) return ((1 - (7.5625f * (t - B3) * (t - B3) + .75f)) / 2);
			if (t < B4) return ((1 - (7.5625f * (t - B5) * (t - B5) + .9375f)) / 2);
			return ((1 - (7.5625f * (t - B6) * (t - B6) + .984375f)) / 2);
		}

		t = t * 2 - 1;
		if (t < B1) return ((7.5625f * t * t) / 2 + .5f);
		if (t < B2) return ((7.5625f * (t - B3) * (t - B3) + .75f) / 2 + .5f);
		if (t < B4) return ((7.5625f * (t - B5) * (t - B5) + .9375f) / 2 + .5f);
		return ((7.5625f * (t - B6) * (t - B6) + .984375f) / 2 + .5f);
	}

	#endregion

	#region Quad

	/// <summary>
	/// Quadratic in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float QuadIn(float t)
	{
		return (t * t);
	}

	/// <summary>
	/// Quadratic out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float QuadOut(float t)
	{
		return (-t * (t - 2));
	}

	/// <summary>
	/// Quadratic in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float QuadInOut(float t)
	{
		return (t <= .5 ? t * t * 2 : 1 - (--t) * t * 2);
	}

	#endregion

	#region Circ

	/// <summary>
	/// Circle in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float CircIn(float t)
	{
		return (-(Mathf.Sqrt(1 - t * t) - 1));
	}

	/// <summary>
	/// Circle out
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float CircOut(float t)
	{
		return (Mathf.Sqrt(1 - (t - 1) * (t - 1)));
	}

	/// <summary>
	/// Circle in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float CircInOut(float t)
	{
		return (t <= .5 ? (Mathf.Sqrt(1 - t * t * 4) - 1) / -2 : (Mathf.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) / 2);
	}

	#endregion

	#region Expo

	/// <summary>
	/// Exponential in.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float ExpoIn(float t)
	{
		return (Mathf.Pow(2, 10 * (t - 1)));
	}

	/// <summary>
	/// Exponential out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float ExpoOut(float t)
	{
		if (t == 1) return 1;
		return (-Mathf.Pow(2, -10 * t) + 1);
	}

	/// <summary>
	/// Exponential in and out.
	/// </summary>
	/// <param name="t">Time elapsed.</param>
	/// <returns>Eased timescale.</returns>
	public static float ExpoInOut(float t)
	{
		if (t == 1) return 1;
		return (t < .5 ? Mathf.Pow(2, 10 * (t * 2 - 1)) / 2 : (-Mathf.Pow(2, -10 * (t * 2 - 1)) + 2) / 2);
	}

	#endregion
}