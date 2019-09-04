using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuadTileBoardPCG))]
public class QuadTileBoardPCGInspector : Editor
{

    public override void OnInspectorGUI()
    {
        QuadTileBoardPCG pcg = target as QuadTileBoardPCG;
        if(GUILayout.Button("Clear"))
        {
            for(int i = pcg.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(pcg.transform.GetChild(i).gameObject);
            }
        }

        pcg.tilePrefab = EditorGUILayout.ObjectField("Tile Prefab", pcg.tilePrefab, typeof(GameObject), true) as GameObject;
        EditorGUILayout.BeginHorizontal();

        pcg.stepCount = EditorGUILayout.IntField(pcg.stepCount);
        pcg.walkCount = EditorGUILayout.IntField(pcg.walkCount);

        if (GUILayout.Button("Create Map"))
        {
            DrunkenWalk(pcg, pcg.tilePrefab, Vector3.zero, new List<Vector3>(){
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left },
                pcg.stepCount,
                pcg.walkCount);
        }
        EditorGUILayout.EndHorizontal();

        pcg.wanderWalk = EditorGUILayout.Toggle("Wander", pcg.wanderWalk);
        base.OnInspectorGUI();
    }

    void DrunkenWalk(QuadTileBoardPCG pcg, GameObject prefab, Vector3 start, List<Vector3> directions, int stepCount, int walkCount = 1)
    {
        System.Random seedGenerator = new System.Random();
        System.Random rnd = new System.Random(seedGenerator.Next());

        List<GameObject> tiles = new List<GameObject>();

        for(int j = 0; j < walkCount; j++)
        {
            Vector3 curPoint = Vector3.zero;
            if (pcg.wanderWalk)
            {
                curPoint = tiles.Count == 0 ? start : tiles[rnd.Next(0, tiles.Count)].transform.position;
            }
            else
            {
                curPoint = start;
            }
            
            for (int i = 0; i < stepCount; i++)
            {
                GameObject tile = Instantiate<GameObject>(prefab, curPoint, Quaternion.identity, pcg.transform);
                tiles.Add(tile);
                int dirIndex = rnd.Next(0, directions.Count);
                Vector3 dir = directions[dirIndex];
                curPoint += dir;
            }
        }
    }
}
