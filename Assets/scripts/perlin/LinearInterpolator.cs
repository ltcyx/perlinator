using UnityEngine;
using System.Collections;

public class LinearInterpolator : Interpolator {

	public double interpolate(double v0, double v1, double interpolation) {
		return (v1 - v0) * interpolation + v0;
	}

}
