using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chank : MonoBehaviour
{
    public Terrain Terrain;
    public Vector2Int ChankPosition;

    public bool EnabledСhank = false;

    public void Init()
    {
        Terrain = transform.GetComponent<Terrain>();

        ChankPosition = new Vector2Int((int)(transform.position.x / Terrain.terrainData.size.x), (int)(transform.position.z / Terrain.terrainData.size.x));        
        
    }
}
