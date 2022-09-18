using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// to create the endless terrain effect
// spawns the chunks as viewer gets closer
// reduces detail of chunks further away
// despawns those outside of viewer distance
public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDistance = 450;  // how far user can see, const means the value cannot change at runtime
    public Transform viewer;

    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    // set values
    void Start()
    {
        chunkSize = MapGenerator.mapChunkSize - 1;  // since mapChunkSize is defined as the number of points in line, but last cant be used as starting point of a trangle
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);  // essentially how many triangles away we can see
    }

    // set viewer position and check chunks each frame
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    // check which chunks are to be visible or not
    void UpdateVisibleChunks()
    {
        // first, set chunks from previous update back to invisible
        // they wont be set after this if they are not in viewer range
        // so any chunks outside of viewer range that were in last frame need to be reset
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();  // reset list so we can reassign later

        // chunks coordinates are defined as one chunk being one unit,
        // so when user moves to chunk immediately to right of their current chunk (looking upon from above),
        // they have moved to chunk +1 over on x
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        // loop through surrounding chunks in viewer distance
        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                // get this chunks coordinate
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDict.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();  // updates chunks visibility

                    // if it is visible, add to visible chunks list
                    if (terrainChunkDict[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDict[viewedChunkCoord]);
                    }
                } 
                else
                {
                    // add a new terrain chunk to the dictionary, visibility is checked
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }


    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        // constructor
        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            // get the position of chunk in regular mesh units, not chunk units
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);  // creates a bounding box at position of size units wide
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);  // 3D version

            // create the chunk mesh object, start as primitive plane
            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size / 10f;  // PrimitiveType.Plane is actually by default 10 units, so we scale back by 10
            meshObject.transform.parent = parent;  // parent just to clean up inspector
            SetVisible(false);  // default state of chunk mesh is disabled
        }

        // find point on perimiter that is closest to view position
        // if the distance from this point to viewer is less than viewer distance, enable chunk
        // otherwise, disable chunk
        public void UpdateTerrainChunk()
        {
            // get distance using bounds square distance function
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            // get and set visibility
            bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;
            SetVisible(visible);
        }

        // sets visible state of mesh
        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
