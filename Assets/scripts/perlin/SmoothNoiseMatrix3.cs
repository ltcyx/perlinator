using UnityEngine;
using System.Collections;
using System;

public class SmoothNoiseMatrix3 {

	private class InterpolationValue {
		public int x0;
		public int x1;
		public double d;

		public InterpolationValue(int x0, int x1, double d) {
			this.x0 = x0;
			this.x1 = x1;
			this.d = d;
		}

		private static double roundToZero(double val) {
			return val > 0 ? Math.Floor(val) : Math.Ceiling(val);
		}

		public static InterpolationValue NewFromValue(double val) {
			int ival;
			double dval;
			if (val >= 0) {
				ival = (int)Math.Floor(val);
				dval = val - ival;
			} else {
				ival = (int)Math.Ceiling(val);
				dval = 1 - (val - ival);
				//we want this to be the value on the left
				--ival;
			}
			return new InterpolationValue(ival, ival+1, dval);
		}
	}

	private NoiseMatrix3 matrix;
	private Interpolator interpolator;

	public SmoothNoiseMatrix3(NoiseMatrix3 matrix, Interpolator interpolator) {
		this.matrix = matrix;
		this.interpolator = interpolator;
	}

	public double getValue(double x, double y, double z) {
		return getRawValue(x*matrix.sizex, y*matrix.sizey, z*matrix.sizez);
	}

	public double getRawValue(double x, double y, double z) {

		InterpolationValue ix = InterpolationValue.NewFromValue(x);
		InterpolationValue iy = InterpolationValue.NewFromValue(y);
		InterpolationValue iz = InterpolationValue.NewFromValue(z);

		//0,0,0 is point towards 'screen' and on the bottom left
		//x being horizontal, y vertical and z depth

		double p000 = matrix.getValue(ix.x0, iy.x0, iz.x0);
		double p001 = matrix.getValue(ix.x0, iy.x0, iz.x1);
		double p010 = matrix.getValue(ix.x0, iy.x1, iz.x0);
		double p011 = matrix.getValue(ix.x0, iy.x1, iz.x1);
		double p100 = matrix.getValue(ix.x1, iy.x0, iz.x0);
		double p101 = matrix.getValue(ix.x1, iy.x0, iz.x1);
		double p110 = matrix.getValue(ix.x1, iy.x1, iz.x0);
		double p111 = matrix.getValue(ix.x1, iy.x1, iz.x1);

		double p00 = interpolator.interpolate(p000, p001, iz.d);
		double p01 = interpolator.interpolate(p010, p011, iz.d);
		double p10 = interpolator.interpolate(p100, p101, iz.d);
		double p11 = interpolator.interpolate(p110, p111, iz.d);

		double p0 = interpolator.interpolate(p00, p01, iy.d);
		double p1 = interpolator.interpolate(p10, p11, iy.d);

		double p = interpolator.interpolate(p0, p1, ix.d);

		return p;
	}

}
