using UnityEngine;
using System.Collections;

public class PerlinNoise {

	private SmoothNoiseMatrix3 matrix;
	private int octaves;
	private double persistence;
	private double lacunarity;

	public PerlinNoise(SmoothNoiseMatrix3 matrix, int octaves) {
		init(matrix, octaves, 1.0/2.0, 2.0);
	}

	public PerlinNoise(SmoothNoiseMatrix3 matrix, int octaves, double persistence, double lacunarity) {
		init(matrix, octaves, persistence, lacunarity);
	}

	private void init(SmoothNoiseMatrix3 matrix, int octaves, double persistence, double lacunarity) {
		this.matrix = matrix;
		this.octaves = octaves;
		this.persistence = persistence;
		this.lacunarity = lacunarity;
	}

	public double getValue(double x, double y, double z) {
		double value = 0;
		double accumulatedAmplitudes = 0;
		double frequency = 1;
		double amplitude = 1;
		for (int i=0; i<octaves; ++i) {
			accumulatedAmplitudes += amplitude;
			value += matrix.getValue(x*frequency, y*frequency, z*frequency) * amplitude;
			frequency *= lacunarity;
			amplitude *= persistence;
		}
		return value/accumulatedAmplitudes;
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
