using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainHeightChecker
{
    public static float getHeight(Vector3 c)
    {
        Vector3 coordinates = new Vector3(c.x, c.y, c.z);
        foreach(Terrain t in Terrain.activeTerrains)
        {
            Vector3 siz = t.terrainData.size;
            Vector3 pos = t.gameObject.transform.position;
            if(coordinates.x < pos.x || coordinates.x > pos.x + siz.x)
                continue;
            if(coordinates.z < pos.z || coordinates.z > pos.z + siz.z)
                continue;
            coordinates = coordinates - pos;
            return pos.y + t.SampleHeight(coordinates);
        }
        
        return 0.0F;
    }
}
