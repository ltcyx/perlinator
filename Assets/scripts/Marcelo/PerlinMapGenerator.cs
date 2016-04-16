using UnityEngine;
using System.Collections;

public class PerlinMapGenerator : MonoBehaviour 
{
	public int sizeX;
	public int sizeY;
	public int octaves;
	public bool randomSeed;
	public int seed;
	public bool generate = false;

	public Gradient color;

	private bool seedHasBeSetted = false;

	public GameObject sphere;

	// Use this for initialization
	void Start () 
	{
		if(!randomSeed)
		{
			PerlinTexture.SetSeed(seed);
			seedHasBeSetted = true;
		}

		PerlinTexture.SetColorGradient(color);

		this.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = PerlinTexture.CreatePerlinNoise(sizeX, sizeY, octaves);
		sphere.GetComponent<Renderer>().material.mainTexture = this.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture;
		sphere.GetComponent<Renderer>().material.SetTexture("_BumpMap",sphere.GetComponent<Renderer>().material.mainTexture);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(generate)
		{
			generate = false;

			if(randomSeed)
			{
				if(seedHasBeSetted)
				{
					PerlinTexture.SetSeed();
				}
			}
			else
			{
				PerlinTexture.SetSeed(seed);
				seedHasBeSetted = true;
			}

			PerlinTexture.SetColorGradient(color);
			
			this.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = PerlinTexture.CreatePerlinNoise(sizeX, sizeY, octaves);
			sphere.GetComponent<Renderer>().material.mainTexture = this.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture;
			sphere.GetComponent<Renderer>().material.SetTexture("_NormalMap",sphere.GetComponent<Renderer>().material.mainTexture);
		}
	}

}
