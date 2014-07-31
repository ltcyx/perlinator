using UnityEngine;
using System.Collections.Generic;

public class PerlinQuadsTerrainBehaviour : MonoBehaviour {

	public double scale = 1;

	public Material chunkMaterial;

	private PerlinNoise perlin;
	private int chunkSize = 4;
	private int worldHeight = 64;
	private int seaLevel = 32;

	// Use this for initialization
	void Start () {
		perlin = new PerlinNoise(new SmoothNoiseMatrix3(new NoiseMatrix3(64, 0), new LinearInterpolator()), 1);

		for (int ix=0; ix<16; ++ix) {
			for (int iz=0; iz<16; ++iz) {
				generateChunk(ix, iz);
			}
		}
	}

	private void generateChunk(int x, int z) {
		GameObject chunk = new GameObject("chunk"+x+"-"+z);
		chunk.AddComponent<MeshFilter>();
		chunk.AddComponent<MeshRenderer>();
		chunk.GetComponent<MeshRenderer>().sharedMaterial = chunkMaterial;
		chunk.transform.position = new Vector3(x*chunkSize, 0, z*chunkSize);
		chunk.transform.parent = this.transform;

		List<Vector3> verts = new List<Vector3>();
		List<Vector3> norms = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> tris = new List<int>();

		Vector3 nfront = new Vector3(0, 0, -1);
		Vector3 nback = new Vector3(0, 0, 1);
		Vector3 nleft = new Vector3(-1, 0, 0);
		Vector3 nright = new Vector3(1, 0, 0);
		Vector3 ntop = new Vector3(0, 1, 0);
		Vector3 nbottom = new Vector3(0, -1, 0);

		Vector2 uv = new Vector2(0, 0);

		for (int ix=0; ix<chunkSize; ++ix) {
			float px = x * chunkSize + ix;
			for (int iz=0; iz<chunkSize; ++iz) {
				float pz = z * chunkSize + iz;
				for (int iy=0; iy<worldHeight; ++iy) {
					float py = iy;

					double pv = perlin.getValue(px*scale, py*scale, pz*scale);
					//Debug.Log("pv = "+pv);
					int perlinVal = (int)(pv * 32) + (seaLevel - iy);

					if (perlinVal > 16) {

						Vector3 v000 = new Vector3(ix + 0, iy + 0, iz + 0);
						Vector3 v001 = new Vector3(ix + 0, iy + 0, iz + 1);
						Vector3 v010 = new Vector3(ix + 0, iy + 1, iz + 0);
						Vector3 v011 = new Vector3(ix + 0, iy + 1, iz + 1);
						Vector3 v100 = new Vector3(ix + 1, iy + 0, iz + 0);
						Vector3 v101 = new Vector3(ix + 1, iy + 0, iz + 1);
						Vector3 v110 = new Vector3(ix + 1, iy + 1, iz + 0);
						Vector3 v111 = new Vector3(ix + 1, iy + 1, iz + 1);

						//front
						this.newTriForBlockMesh(v000, v010, v110, nfront, uv, uv, uv, verts, norms, uvs, tris);
						this.newTriForBlockMesh(v110, v100, v000, nfront, uv, uv, uv, verts, norms, uvs, tris);
						//back
						this.newTriForBlockMesh(v001, v111, v011, nback, uv, uv, uv, verts, norms, uvs, tris);
						this.newTriForBlockMesh(v111, v001, v101, nback, uv, uv, uv, verts, norms, uvs, tris);
						//left
						this.newTriForBlockMesh(v000, v001, v011, nleft, uv, uv, uv, verts, norms, uvs, tris);
						this.newTriForBlockMesh(v011, v010, v000, nleft, uv, uv, uv, verts, norms, uvs, tris);
						//right
						this.newTriForBlockMesh(v100, v111, v101, nright, uv, uv, uv, verts, norms, uvs, tris);
						this.newTriForBlockMesh(v111, v100, v110, nright, uv, uv, uv, verts, norms, uvs, tris);
						//top
						this.newTriForBlockMesh(v010, v011, v111, ntop, uv, uv, uv, verts, norms, uvs, tris);
						this.newTriForBlockMesh(v111, v110, v010, ntop, uv, uv, uv, verts, norms, uvs, tris);
						//bottom
						this.newTriForBlockMesh(v000, v101, v001, nbottom, uv, uv, uv, verts, norms, uvs, tris);
						this.newTriForBlockMesh(v101, v000, v100, nbottom, uv, uv, uv, verts, norms, uvs, tris);
					} else {
						//Debug.Log ("skipping block!");
					}
				}
			}
		}

		MeshFilter meshfilter = chunk.GetComponent<MeshFilter>();
		Mesh mesh = meshfilter.sharedMesh;
		if (mesh==null) {
			meshfilter.mesh = new Mesh();
			mesh = meshfilter.sharedMesh;
		}

		Debug.Log("size of verts: "+verts.Count);

		mesh.Clear();
		mesh.vertices = verts.ToArray();
		mesh.normals = norms.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = tris.ToArray();

		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	private void newTriForBlockMesh(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 normal, Vector3 uv0, Vector3 uv1, Vector3 uv2, List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris) {
		int firstVert = verts.Count;
		verts.Add(p0);
		uvs.Add(uv0);
		verts.Add(p1);
		uvs.Add(uv1);
		verts.Add(p2);
		uvs.Add(uv2);
		for (int i=0; i<3; ++i) {
			//add indexes for the last 3 points
			tris.Add(firstVert + i);
			norms.Add(normal);
		}

	}

	// Update is called once per frame
	void Update () {
	
	}
}
