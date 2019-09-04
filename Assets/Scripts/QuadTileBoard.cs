using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTileBoard : MonoBehaviour
{
    [SerializeField]
    private List<QuadTile> allTiles = null;

    public void AddTile(QuadTile tile)
    {
        allTiles.Add(tile);
    }

    public void Remove(QuadTile tile)
    {
        allTiles.Remove(tile);
    }
}
