using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator
{
    // takes in a color map, width and height and creates a texture
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        // settings for texture
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // apply these colours to the new texture
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

    // takes in a height map, creates the necessary color map, width and height and uses TextureFromColorMap to get texture
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        // get width and height for new texture
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // we use a colour map of length big enough for every point in noise map
        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // we multiply y * width as we are getting which row, add x to signify column
                // we then lerp bewteen black and white depending on noise value
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }
}
