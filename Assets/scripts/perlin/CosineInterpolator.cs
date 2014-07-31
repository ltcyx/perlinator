using UnityEngine;
using System.Collections;

public class CosineInterpolator : Interpolator {
	
	public double interpolate(double v0, double v1, double interpolation) {
		double ft = interpolation * System.Math.PI;
		double f = (1 - System.Math.Cos(ft)) * 0.5;
		return v0*(1-f) + v1*f;
	}
	
}
