using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Assets.scripts.util;

namespace Assets.scripts.terrain.quads
{

    public class PerlinQuadsTerrainBehaviour : MonoBehaviour
    {

        public double scale = 1;
        public double verticalScale = 1;

        public Material chunkMaterial;

        private PerlinNoise Perlin;
        private int ChunkSize = 16;
        private int WorldHeight = 128;
        private int SeaLevel = 64;

        private int WorldHeightOffset = 64;

        private int InitialGenerationSize = 64;

        private Dictionary<string, TerrainChunk> Chunks;

        // Use this for initialization
        IEnumerator Start()
        {
            var tm = ThreadManager.Instance;
            yield return null;

            Perlin = new PerlinNoise(new SmoothNoiseMatrix3(new NoiseMatrix3(64, 2), new LinearInterpolator()), 1, 0.3, 4);
            Chunks = new Dictionary<string, TerrainChunk>();

            for (int ix = 0; ix < InitialGenerationSize; ++ix)
            {
                for (int iz = 0; iz < InitialGenerationSize; ++iz)
                {
                    generateChunk(ix, iz);
                }
                //yield return null;
            }
            yield return null;
        }

        private void generateChunk(int x, int z)
        {
            var chunk = new GameObject("chunk" + GetChunkKey(x, z)).AddComponent<TerrainChunk>();

            chunk.transform.position = new Vector3(x * ChunkSize, 0, z * ChunkSize);
            chunk.transform.parent = this.transform;

            ThreadManager.Instance.ExecuteInBackground(() =>
            {
                var blockValues = new int[ChunkSize, WorldHeight, ChunkSize];

                for (int ix = 0; ix < ChunkSize; ++ix)
                {
                    int px = x * ChunkSize + ix;
                    for (int iz = 0; iz < ChunkSize; ++iz)
                    {
                        int pz = z * ChunkSize + iz;
                        for (int iy = 0; iy < WorldHeight; ++iy)
                        {
                            int py = iy;

                            var pv = getPerlinValue(px, py, pz);
                            var cpv = convertPerlinValue(pv, py);
                            blockValues[ix, iy, iz] = cpv;
                        }
                    }
                }

                Chunks.Add(GetChunkKey(x, z), chunk);

                chunk.Init(ChunkSize, WorldHeight, SeaLevel, blockValues, chunkMaterial);
            });
        }

        private string GetChunkKey(int x, int z)
        {
            return string.Format("{0},{1}", x, z);
        }

        private int convertPerlinValue(double perlinValue, int ySeaLevelOffset)
        {
            return (int)((perlinValue * WorldHeightOffset) + (SeaLevel - ySeaLevelOffset));
        }

        private double getPerlinValue(int x, int y, int z)
        {
            return Perlin.getValue(x * scale, y * verticalScale, z * scale);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}