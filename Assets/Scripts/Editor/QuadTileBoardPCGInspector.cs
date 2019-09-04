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
            pcg.StartCoroutine(pcg.DrunkenWalk(pcg, pcg.tilePrefab, Vector3.zero, new List<Vector3>(){
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left },
                pcg.stepCount,
                pcg.walkCount));
        }
        EditorGUILayout.EndHorizontal();

        pcg.wanderWalk = EditorGUILayout.Toggle("Wander", pcg.wanderWalk);
        base.OnInspectorGUI();
    }
}
