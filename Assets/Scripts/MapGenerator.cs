using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh }
    public DrawMode drawMode;

    public const int mapChunkSize = 241;  // max amount of triangles in a mesh is 255*255

    [Range(0, 6)]  // these will be multiplied by 2 to ensure we get 2, 4, 6, 8, 10, 12, since these are factors of 241 - 1 = 240
    public int levelOfDetail;  // to determine mesh size at large distances
    public int seed;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]  // persistance must be between 0 and 1
    public float persistance;
    public float lacunarity;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;  // lets us define a falloff for height multiplier

    public bool autoUpdate = false;

    public TerrainType[] regions;

    // create a map based on the noisemap gotten
    public void GenerateMap()
    {
        // get noise map from Noise script
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        // assign colors based on regions and height
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];  // color map for all points
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                // get current height from noise map
                float currentHeight = noiseMap[x, y];

                // pass through all regions
                for (int i = 0; i < regions.Length; i++)
                {
                    // if we are in this region
                    if (currentHeight <= regions[i].height)
                    {
                        // assign color and break out of region loop
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        // get map display from MapDisplay script
        MapDisplay display = FindObjectOfType<MapDisplay>();

        // draw the noise map or color map or mesh depending on which is selected
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            // draw mesh draws both the mesh and the colour on top of the mesh
            // takes in the mesh and the color map
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }

    // OnValidate is called each time a value in the editor is changed
    private void OnValidate()
    {
        // clamp values
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 1)
        {
            octaves = 1;
        }
    }

    // struct containing the details of each terrain region
    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color color;
    }

}
