using UnityEngine;
using System.Collections;

public class PerlinClientBehaviour : MonoBehaviour {

	private PerlinNoise perlin;
	private Texture2D tex;

	public double depth = 0;
	public double depthSpeed = 0;
	public int octaves = 1;
	public double textureScale = 1;

	public bool updateTexture = false;

	void Start () {
		perlin = new PerlinNoise(new SmoothNoiseMatrix3(new NoiseMatrix3(32,1), new CosineInterpolator()), 1, 0.5, 2);
		tex = new Texture2D(256, 256);
		renderer.sharedMaterial.mainTexture = tex;
		UpdateTexture();
	}

	private void UpdateTexture() {
		perlin.Octaves = octaves;
		double highestVal = 0;
		for (int i=0; i<tex.width; ++i) {
			double x = (double)i/(double)tex.width;
			for (int j=0; j<tex.height; ++j) {
				double y = (double)j/(double)tex.height;
				double val = perlin.getValue(x * textureScale, y * textureScale, depth);
				if (val > highestVal) highestVal = val;
				Color col = getColourForValue(val);
				tex.SetPixel(i, j, col);
			}
		}
		tex.Apply();
		renderer.sharedMaterial.mainTexture = tex;
		Debug.Log("highest value: "+highestVal);
	}

	private Color getColourForValue(double val) {
		return new Color((float)val, (float)val, (float)val);
	}

	void Update () {
		if (textureScale < 0.00001) textureScale = 0.00001;
		if (updateTexture) {
			UpdateTexture();
			updateTexture = false;
		}
		if (depthSpeed!=0) {
			depth += depthSpeed * Time.deltaTime;
			UpdateTexture();
		}
	}


}
