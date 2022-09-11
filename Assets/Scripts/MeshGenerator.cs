using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve)
    {
        // variables
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;  // formula to get topleft offset, so we can move mesh to center of screen
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        // assign vertices of all points
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);  // y on its own acts as height (z-axis)
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);  // weird indices as we are skipping rows to get to next point in triangle
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

// create vector for each vertex and assign list of identifiers for triangles
public class MeshData
{
    public Vector3[] vertices;  // each vertex is a point in 3D space
    public int[] triangles;  // we can represent each vertex of a triangle by a number, so one individual triangle would be 3 of these numbers in a row

    public Vector2[] uvs;  // uvs tell mesh how to draw, essentially markers for which points to draw in space (so they must be normalised)

    int triangleIndex;  // which triangle we are interested in

    // constructor to assign the vertices and triangles
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];  // total amount of vertices is every point
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];  // total amount of triangles follows this formula; note we dont create triangles at bottom or right edge of mesh
    }

    // create a triangle by assigning 3 digits to be each vertex of this triangle
    // so if we have a triangle at 1, 2, 3; we represent this in triangles[] as [...1, 2, 3...]
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;  // make sure we move to next to available spot
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
