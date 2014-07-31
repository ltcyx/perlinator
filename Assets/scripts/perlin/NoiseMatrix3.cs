using UnityEngine;
using System.Collections;

public class NoiseMatrix3 {

	private double[,,] values;

	public NoiseMatrix3(int size, int seed) {
		System.Random rand = new System.Random(seed);
		int sizex, sizey, sizez;
		sizex = sizey = sizez = size;

		values = new double[sizex,sizey,sizez];
		for (int i=0; i<values.GetLength(0); ++i) {
			for (int j=0; j<values.GetLength(1); ++j) {
				for (int k=0; k<values.GetLength(2); ++k) {
					values[i,j,k] = rand.NextDouble();
				}
			}
		}
	}

	private int wrapValue(int value, int max) {
		value = value % max;
		while(value<0) {
			value = max - value;
		}
		return value;
	}

	public double getValue(int x, int y, int z) {
		return values[wrapValue(x, values.GetLength(0)), wrapValue(y, values.GetLength(1)), wrapValue(z, values.GetLength(2))];
	}

	public int sizex {
		get {
			return values.GetLength(0);
		}
	}

	public int sizey {
		get {
			return values.GetLength(1);
		}
	}

	public int sizez {
		get {
			return values.GetLength(2);
		}
	}

}
