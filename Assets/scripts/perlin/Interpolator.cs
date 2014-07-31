using UnityEngine;
using System.Collections;

public interface Interpolator{
	double interpolate(double v0, double v1, double interpolation);
}
