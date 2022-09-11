using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    // get the renderer of the object we apply noise to (plane)
    public Renderer textureRenderer;

    // get the filter (for mesh drawing) and renderer (for texture drawing) of the mesh
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    // draws the texture onto the textureRenderer
    public void DrawTexture(Texture2D texture)
    {
        // apply texture to renderer, scale it to be as width and height
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    // draws the mesh and the color onto the texture onto the mesh
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        // assign mesh
        meshFilter.sharedMesh = meshData.CreateMesh();

        // assign texture
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
