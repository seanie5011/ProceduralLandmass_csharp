using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap }
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public int seed;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]  // persistance must be between 0 and 1
    public float persistance;
    public float lacunarity;
    public Vector2 offset;

    public bool autoUpdate = false;

    public TerrainType[] regions;

    // create a map based on the noisemap gotten
    public void GenerateMap()
    {
        // get noise map from Noise script
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        // assign colors based on regions and height
        Color[] colorMap = new Color[mapWidth * mapHeight];  // color map for all points
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapHeight; x++)
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
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        // get map display from MapDisplay script
        MapDisplay display = FindObjectOfType<MapDisplay>();

        // draw the noise map or color map depending on which is selected
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }

    // OnValidate is called each time a value in the editor is changed
    private void OnValidate()
    {
        // clamp values
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = 1;
        }

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
