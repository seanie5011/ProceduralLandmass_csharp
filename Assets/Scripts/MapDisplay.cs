using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    // get the renderer of the object we apply noise to (plane)
    public Renderer textureRenderer;

    // draws the texture onto the textureRenderer
    public void DrawTexture(Texture2D texture)
    {
        // apply texture to renderer, scale it to be as width and height
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
}
