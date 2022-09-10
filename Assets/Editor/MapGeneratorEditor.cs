using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// specify it is a custom editor of type MapGenerator
[CustomEditor(typeof (MapGenerator))]
public class MapGeneratorEditor : Editor
{
    // override the OnInspectorGUI method with our own
    // this allows us to create a button in the inspector to generate the map
    public override void OnInspectorGUI()
    {
        // get the MapGenerator of the selected object
        MapGenerator mapGen = (MapGenerator)target;

        // draw normal inspector
        if (DrawDefaultInspector())
        {
            // if auto update enable, generate new map
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        // add our generate button that generates the map
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
