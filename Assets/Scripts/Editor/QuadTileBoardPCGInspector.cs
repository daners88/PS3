using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuadTileBoardPCG))]
public class QuadTileBoardPCGInspector : Editor
{
    static IEnumerator thingy = null;
    private void OnEnable()
    {
        EditorApplication.update += InEditorGenerate;
    }

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
        pcg.waterPrefab = EditorGUILayout.ObjectField("Water Prefab", pcg.waterPrefab, typeof(GameObject), true) as GameObject;
        pcg.placementCounter = EditorGUILayout.ObjectField("Placement Count UI Object", pcg.placementCounter, typeof(GameObject), true) as GameObject;
        pcg.timer = EditorGUILayout.ObjectField("Timer UI Object", pcg.timer, typeof(GameObject), true) as GameObject;
        EditorGUILayout.BeginHorizontal();
        pcg.seed = EditorGUILayout.IntField(pcg.seed);
        pcg.stepCount = EditorGUILayout.IntField(pcg.stepCount);
        pcg.walkCount = EditorGUILayout.IntField(pcg.walkCount);
        pcg.waterBuffer = EditorGUILayout.IntField(pcg.waterBuffer);

        if (GUILayout.Button("Create Map"))
        {
            thingy = pcg.DrunkenWalk();
        }
        EditorGUILayout.EndHorizontal();

        pcg.wanderWalk = EditorGUILayout.Toggle("Wander", pcg.wanderWalk);
        base.OnInspectorGUI();
    }

    private void InEditorGenerate()
    {
        if(thingy != null)
        {
            if (!thingy.MoveNext())
            {
                thingy = null;
            }
        }
        
    }
}
