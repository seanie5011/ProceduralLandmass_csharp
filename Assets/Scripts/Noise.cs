using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// static since it will only have one instance
public static class Noise
{
    // function to generate the noise map
    // has width and height specified
    // can be scaled, 
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        // noiseMap containing each points noise value
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // apply seed, randomises where we take the noise from
        System.Random prng = new System.Random(seed);

        // for each octave, apply some offset for the sample points taken to randomise where we get noise from if seed is different
        // also have user inputted noise
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // clamp scale
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        // max and min to normalise noise values
        float maxNoiseHeight = float.MinValue;  // max Noise height starts at lowest possible value so it can be assigned up to the new max
        float minNoiseHeight = float.MaxValue;  // likewise but it will be assigned down to new min

        // half values so that scale zooms to center
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        // create noise for each x and y
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    // add scale and frequency effects to sample points
                    // if frequency is bigger, further spacing between sample points to increase changes in height
                    // also have corrections for scale zoom centering, and offsets
                    float sampleX = ((x - halfWidth) / scale + octaveOffsets[i].x) * frequency;
                    float sampleY = ((y - halfHeight) / scale + octaveOffsets[i].y) * frequency;

                    // get value from perlin noise and multiply by amplitude
                    // we want perlin noise to be in range -1, 1 as we get more interesting noise then for noiseHeight
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    // amplitude decreases each octave
                    amplitude *= persistance;

                    // frequency increases each octave
                    frequency *= lacunarity;
                }

                // get maximum and minimum noise heights to be used later
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                // update noiseMap
                noiseMap[x, y] = noiseHeight;
            }
        }

        // normalise all values in noiseMap
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);  // forces noiseMap value at this point to be between 0 and 1
            }
        }

        return noiseMap;
    }
}
