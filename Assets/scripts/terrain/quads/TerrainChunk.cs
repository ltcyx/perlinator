using Assets.scripts.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.terrain.quads
{
    public class TerrainChunk : MonoBehaviour
    {
        private int ChunkSize { get; set; }
        private int WorldHeight { get; set; }
        private int SeaLevel { get; set; }

        private int[,,] Cells;


        public void Init(int chunkSize, int worldHeight, int seaLevel, int[,,] cellValues, Material chunkMaterial)
        {
            ChunkSize = chunkSize;
            WorldHeight = worldHeight;
            SeaLevel = seaLevel;
            Cells = cellValues;

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

            for (int ix = 0; ix < ChunkSize; ++ix)
            {
                for (int iz = 0; iz < ChunkSize; ++iz)
                {
                    for (int iy = 0; iy < WorldHeight; ++iy)
                    {
                        int bv = GetBlockValue(ix, iy, iz);

                        if (isBlockOpaque(bv, iy))
                        {

                            Vector3 v000 = new Vector3(ix + 0, iy + 0, iz + 0);
                            Vector3 v001 = new Vector3(ix + 0, iy + 0, iz + 1);
                            Vector3 v010 = new Vector3(ix + 0, iy + 1, iz + 0);
                            Vector3 v011 = new Vector3(ix + 0, iy + 1, iz + 1);
                            Vector3 v100 = new Vector3(ix + 1, iy + 0, iz + 0);
                            Vector3 v101 = new Vector3(ix + 1, iy + 0, iz + 1);
                            Vector3 v110 = new Vector3(ix + 1, iy + 1, iz + 0);
                            Vector3 v111 = new Vector3(ix + 1, iy + 1, iz + 1);

                            //front
                            if (!isNeighborBlockOpaque(ix, iy, iz, Direction.Front))
                            {
                                this.newTriForBlockMesh(v000, v010, v110, nfront, uv, uv, uv, verts, norms, uvs, tris);
                                this.newTriForBlockMesh(v110, v100, v000, nfront, uv, uv, uv, verts, norms, uvs, tris);
                            }
                            //back
                            if (!isNeighborBlockOpaque(ix, iy, iz, Direction.Back))
                            {
                                this.newTriForBlockMesh(v001, v111, v011, nback, uv, uv, uv, verts, norms, uvs, tris);
                                this.newTriForBlockMesh(v111, v001, v101, nback, uv, uv, uv, verts, norms, uvs, tris);
                            }
                            //left
                            if (!isNeighborBlockOpaque(ix, iy, iz, Direction.Left))
                            {
                                this.newTriForBlockMesh(v000, v001, v011, nleft, uv, uv, uv, verts, norms, uvs, tris);
                                this.newTriForBlockMesh(v011, v010, v000, nleft, uv, uv, uv, verts, norms, uvs, tris);
                            }
                            //right
                            if (!isNeighborBlockOpaque(ix, iy, iz, Direction.Right))
                            {
                                this.newTriForBlockMesh(v100, v111, v101, nright, uv, uv, uv, verts, norms, uvs, tris);
                                this.newTriForBlockMesh(v111, v100, v110, nright, uv, uv, uv, verts, norms, uvs, tris);
                            }
                            //top
                            if (!isNeighborBlockOpaque(ix, iy, iz, Direction.Up))
                            {
                                this.newTriForBlockMesh(v010, v011, v111, ntop, uv, uv, uv, verts, norms, uvs, tris);
                                this.newTriForBlockMesh(v111, v110, v010, ntop, uv, uv, uv, verts, norms, uvs, tris);
                            }
                            //bottom
                            if (!isNeighborBlockOpaque(ix, iy, iz, Direction.Down))
                            {
                                this.newTriForBlockMesh(v000, v101, v001, nbottom, uv, uv, uv, verts, norms, uvs, tris);
                                this.newTriForBlockMesh(v101, v000, v100, nbottom, uv, uv, uv, verts, norms, uvs, tris);
                            }
                        }
                        else
                        {
                            //Debug.Log ("skipping block!");
                        }
                    }
                }
            }

            var vertsArray = verts.ToArray();
            var normsArray = norms.ToArray();
            var uvsArray = uvs.ToArray();
            var trisArray = tris.ToArray();

            ThreadManager.Instance.ExecuteInMainThread(() =>
            {
                var meshFilter = gameObject.AddComponent<MeshFilter>();
                var meshCollider = gameObject.AddComponent<MeshCollider>();
                var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = chunkMaterial;

                var mesh = meshFilter.sharedMesh;
                if (mesh == null)
                {
                    meshFilter.mesh = new Mesh();
                    mesh = meshFilter.sharedMesh;
                }

                //Debug.Log("size of verts: " + verts.Count);

                mesh.Clear();
                mesh.vertices = vertsArray;
                mesh.normals = normsArray;
                mesh.uv = uvsArray;
                mesh.triangles = trisArray;

                mesh.RecalculateBounds();
                mesh.Optimize();

                meshCollider.sharedMesh = mesh;
            });
        }

        private int GetBlockValue(int x, int y, int z)
        {
            try
            {
                return Cells[x, y, z];
            }
            catch (Exception e)
            {
                LogError(string.Format("Exception getting cell value [{0}, {1}, {2}]({3}, {4}, {5}): {6}", x, y, z, Cells.GetLength(0), Cells.GetLength(1), Cells.GetLength(2), e));
                throw e;
            }
        }

        private bool isBlockOpaque(int blockValue, int ySeaLevelOffset)
        {
            return blockValue > 32;
        }

        private bool isBlockOpaque(int x, int y, int z)
        {
            int v = GetBlockValue(x, y, z);
            return isBlockOpaque(v, y);
        }

        private bool isNeighborBlockOpaque(int x, int y, int z, Direction dir)
        {
            int dx = 0, dy = 0, dz = 0;
            switch (dir)
            {
                case Direction.Up:
                    dy = 1;
                    break;
                case Direction.Down:
                    dy = -1;
                    break;
                case Direction.Left:
                    dx = -1;
                    break;
                case Direction.Right:
                    dx = 1;
                    break;
                case Direction.Front:
                    dz = -1;
                    break;
                case Direction.Back:
                    dz = 1;
                    break;
            }

            var tx = x + dx;
            var ty = y + dy;
            var tz = z + dz;

            if (tx < 0 || tx >= ChunkSize
                || tz < 0 || tz >= ChunkSize
                || ty < 0 || ty >= WorldHeight)
            {
                return false;
            }
            
            return isBlockOpaque(tx, ty, tz);
        }

        private void newTriForBlockMesh(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 normal, Vector3 uv0, Vector3 uv1, Vector3 uv2, List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris)
        {
            int firstVert = verts.Count;
            verts.Add(p0);
            uvs.Add(uv0);
            verts.Add(p1);
            uvs.Add(uv1);
            verts.Add(p2);
            uvs.Add(uv2);
            for (int i = 0; i < 3; ++i)
            {
                //add indexes for the last 3 points
                tris.Add(firstVert + i);
                norms.Add(normal);
            }

        }

        private void LogError(string error, params object[] formattedParams)
        {
            var msg = string.Format(error, formattedParams);
            ThreadManager.Instance.ExecuteInMainThread(() => { Debug.Log(msg); });
        }


    }
}
