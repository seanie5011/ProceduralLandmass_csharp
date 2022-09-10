using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridRenderer : Graphic
{
    // We want to give the renderer vertices for a square
    // then connect with Triangles
    //
    // we will override this class from Graphic as this is where our mesh is drawn to screen
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // clear the VertexHelp Cache
        vh.Clear();

        // define the area to draw into
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        // create the vertex var we will use
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        // create 4 vertices and add to vh
        vertex.position = new Vector3(0, 0);
        vh.AddVert(vertex);

        vertex.position = new Vector3(0, height);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width, height);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width, 0);
        vh.AddVert(vertex);

        // connect these, we will create 2 Triangles for the square
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}
