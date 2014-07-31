using UnityEngine;
using System.Collections;

public class PerlinNoise {

	private SmoothNoiseMatrix3 matrix;
	private int octaves;

	public PerlinNoise(SmoothNoiseMatrix3 matrix, int octaves) {
		this.matrix = matrix;
		this.octaves = octaves;
	}

	public double getValue(double x, double y, double z) {
		double value = 0;
		double accumulatedScales = 0;
		for (int i=0; i<octaves; ++i) {
			double scale = 1 << i;
			accumulatedScales += 1/scale;
			value += matrix.getValue(x*scale, y*scale, z*scale) / scale;
		}
		return value/accumulatedScales;
	}

	public int Octaves {
		get {
			return octaves;
		}
		set {
			octaves = value;
		}
	}
}
