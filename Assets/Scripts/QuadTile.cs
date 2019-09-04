using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTile : MonoBehaviour
{
    [SerializeField]
    private Vector2Int position = Vector2Int.zero;
    public Vector2Int Position
    {
        get { return position; }
        set { position = value; }
    }
}
