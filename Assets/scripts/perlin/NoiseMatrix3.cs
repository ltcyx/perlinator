using UnityEngine;
using System.Collections;
using System;

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
			value += max;
		}
		return value;
	}

	public double getValue(int x, int y, int z) {
        int wx = wrapValue(x, values.GetLength(0));
        int wy = wrapValue(y, values.GetLength(1));
        int wz = wrapValue(z, values.GetLength(2));
        try
        {
            return values[wx, wy, wz];
        } catch (Exception e)
        {
            Debug.LogError(string.Format("Exception reading value for [{0}, {1}, {2}] size: ({3}, {4}, {5}): {6}", wx, wy, wz, values.GetLength(0), values.GetLength(1), values.GetLength(2), e));
            throw e;
        }
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
