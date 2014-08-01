using UnityEngine;
using System.Collections;

public class PerlinGlDrawer : MonoBehaviour {

	public Material material;
	private Vector3[] vertex;
	public int X;
	public int Y;
	public Texture2D texture;

	// Use this for initialization
	void Start () 
	{
		//this.gameObject.GetComponent<MeshFilter>().mesh = CreateMeshFromTexture(PerlinTest.CreatePerlinNoise(32,32,6));

		vertex = new Vector3[X*Y];
		texture = PerlinTexture.CreatePerlinNoise(X,Y,6);

		for (int i = 0; i < X; i++) 
		{
			for (int j = 0; j <Y; j++) 
			{
				vertex[i*Y  + j] = new Vector3((float)(i), (float)(j), texture.GetPixel(i,j).r);
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Mesh CreateMeshFromTexture(Texture2D texture)
	{
		int sizeX = texture.width;
		int sizeY = texture.height;

		Vector3[] vertices = new Vector3[sizeX*sizeY];
		for (int i = 0; i < sizeX; i++) 
		{
			for (int j = 0; j < sizeY; j++) 
			{
				vertices[i*sizeY  + j] = new Vector3((float)(i), texture.GetPixel(i,j).r, (float)(j));
			}
		}

		int[] triangles = new int[6*(sizeX-1)*(sizeY-1)];
		for (int i = 0; i < (sizeX-1)*(sizeY-1); i += 6) 
		{
			triangles[i] = i;
			triangles[i + 1] = i + 1;
			triangles[i + 2] = i + sizeY + 1;

			triangles[i + 3] = i;
			triangles[i + 4] = i + sizeY + 1;
			triangles[i + 5] = i + sizeY;
		}

		Mesh mesh = new Mesh();

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = new Vector2[sizeX*sizeY];
		mesh.RecalculateNormals();

		return mesh;
	}

	void OnPostRender()
	{
		if (!material) 
		{
			Debug.LogError("Please Assign a material on the inspector");
			return;
		}
		
		GL.PushMatrix();
		material.SetPass(0);

		GL.Begin(GL.TRIANGLES);

		for (int i = 0; i < vertex.Length; i += 6) 
		{
			GL.Vertex3(vertex[i].x, vertex[0].z, vertex[0].y);
			GL.Vertex3(vertex[i + 1].x, vertex[i + 1].z, vertex[i + 1].y);
			GL.Vertex3(vertex[i + Y + 1].x, vertex[i + Y + 1].z, vertex[i + Y + 1].y);

			GL.Vertex3(vertex[i].x, vertex[0].z, vertex[0].y);
			GL.Vertex3(vertex[i + Y + 1].x, vertex[i + Y + 1].z, vertex[i + Y + 1].y);
			GL.Vertex3(vertex[i + Y].x, vertex[i + Y].z, vertex[i + Y].y);
		}
		
		GL.End();
		GL.PopMatrix();
	}
}
