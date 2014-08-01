using UnityEngine;
using System.Collections;

public class PerlinTexture
{
	private static Texture2D CreateNoiseTexture(int width,int height)
	{
		Texture2D noise = new Texture2D(width,height);

		for (int i = 0; i < width; i++) 
		{
			for (int j = 0; j < height; j++) 
			{
				float randomValue = Random.value;
				noise.SetPixel(i,j, new Color(randomValue,randomValue,randomValue,1f));
			}
		}
        return noise;
	}

	private static Texture2D CreateOctaveTexture(Texture2D baseNoise, int octave)
	{
		int width = baseNoise.width;
		int height = baseNoise.height;

		Texture2D noise = new Texture2D(width,height);

		int period = Mathf.RoundToInt(Mathf.Pow(2f,(float)(octave)));
		float frequency = 1f/ (float)(period);

		for (int i = 0; i < width; i++) 
		{
			int i0 = (i/period)*period;
			int i1 = (i0 + period) % width;
			float horizontalBlend = (i - i0) * frequency;

			for (int j = 0; j < height; j++) 
			{
				int j0 = (j/period)*period;
				int j1 = (j0 + period) % height;
				float verticalBlend = (j - j0) * frequency;

				Color top = ColorInterpolate(baseNoise.GetPixel(i0,j0), baseNoise.GetPixel(i1,j0), horizontalBlend);
				Color bottom = ColorInterpolate(baseNoise.GetPixel(i0,j1), baseNoise.GetPixel(i1,j1), horizontalBlend);

				noise.SetPixel(i,j, ColorInterpolate(top, bottom, verticalBlend));
			}
		}

		return noise;
	}

	public static Texture2D CreatePerlinNoise(Texture2D baseNoise, int octaves)
	{
		int width = baseNoise.width;
		int height = baseNoise.height;

		Texture2D noise = new Texture2D(width,height);

		float[][] noiseMatrix = new float[width][];
		for (int i = 0; i < width; i++) 
		{
			noiseMatrix[i] = new float[height];
		}

		float persistance = 0.5f;

		Texture2D[] noiseOctaves = new Texture2D[octaves];
		for (int i = 0; i < octaves; i++) 
		{
			noiseOctaves[i] = CreateOctaveTexture(baseNoise, octaves - 1 - i);
		}

		float amplitude = 1f;
		float totalAmplitude = 0f;

		for (int o = 0; o < octaves; o++) 
		{
			amplitude *= persistance;
			totalAmplitude += amplitude;

			for (int i = 0; i < width; i++) 
			{
				for (int j = 0; j < height; j++) 
				{
					noiseMatrix[i][j] += noiseOctaves[o].GetPixel(i,j).r * amplitude;
				}

			}
		}

		for (int i = 0; i < width; i++) 
		{
			for (int j = 0; j < height; j++) 
			{
				float finalHeight = noiseMatrix[i][j]/totalAmplitude;
				Color color = colorGradient.Evaluate(finalHeight);
				noise.SetPixel(i,j, color);
			}
		}

		noise.Apply();
		return noise;
	}

	public static Texture2D CreatePerlinNoise(int width, int height, int octaves)
	{
		Texture2D noise = CreateNoiseTexture(width, height);
		return CreatePerlinNoise(noise, octaves);
	}

	private static float Interpolate(float value0, float value1, float t)
	{
		//float functionValue = (1f - Mathf.Cos(t * Mathf.PI))/2f;
		//float functionValue = Mathf.Pow(t,2f);
		float functionValue = t;
		return value0*(1f - functionValue) + value1*functionValue;
	}

	private static Color ColorInterpolate(Color value0, Color value1, float t)
	{
		return new Color(Interpolate(value0.r,value1.r,t),
		                 Interpolate(value0.g,value1.g,t),
		                 Interpolate(value0.b,value1.b,t),
		                 Interpolate(value0.a,value1.a,t));
	}

	private static Gradient colorGradient;

	public static void SetColorGradient(Gradient color)
	{
		colorGradient = color;
	}

	public static void SetSeed(int seed)
	{
		Random.seed = seed;
	}

	public static void SetSeed()
	{
		Random.seed = Mathf.RoundToInt(Time.time);
	}
}
