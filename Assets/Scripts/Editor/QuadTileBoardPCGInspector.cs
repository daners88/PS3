using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuadTileBoardPCG))]
public class QuadTileBoardPCGInspector : Editor
{
    static IEnumerator PCGEnumertor = null;
    private void OnEnable()
    {
        EditorApplication.update += InEditorGenerate;
    }

    public override void OnInspectorGUI()
    {
        QuadTileBoardPCG pcg = target as QuadTileBoardPCG;
        if(GUILayout.Button("Clear"))
        {
            for(int i = pcg.tileParent.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(pcg.tileParent.transform.GetChild(i).gameObject);
            }
        }

        pcg.tileParent = EditorGUILayout.ObjectField("Tile Parent", pcg.tileParent, typeof(GameObject), true) as GameObject;
        pcg.cubePrefab = EditorGUILayout.ObjectField("Cube Prefab", pcg.cubePrefab, typeof(GameObject), true) as GameObject;
        pcg.waterPrefab = EditorGUILayout.ObjectField("Water Prefab", pcg.waterPrefab, typeof(GameObject), true) as GameObject;
        pcg.desertMaterial = EditorGUILayout.ObjectField("Desert Material", pcg.desertMaterial, typeof(Material), true) as Material;
        pcg.grassMaterial = EditorGUILayout.ObjectField("Grass Material", pcg.grassMaterial, typeof(Material), true) as Material;
        pcg.forestMaterial = EditorGUILayout.ObjectField("Forest Material", pcg.forestMaterial, typeof(Material), true) as Material;
        pcg.snowMaterial = EditorGUILayout.ObjectField("Snow Material", pcg.snowMaterial, typeof(Material), true) as Material;
        EditorGUILayout.BeginHorizontal();
        pcg.seed = EditorGUILayout.IntField(pcg.seed);
        pcg.stepCount = EditorGUILayout.IntField(pcg.stepCount);
        pcg.walkCount = EditorGUILayout.IntField(pcg.walkCount);
        pcg.waterBuffer = EditorGUILayout.IntField(pcg.waterBuffer);

        if (GUILayout.Button("Create Map"))
        {
            PCGEnumertor = pcg.DrunkenWalk();
        }
        EditorGUILayout.EndHorizontal();

        pcg.wanderWalk = EditorGUILayout.Toggle("Wander", pcg.wanderWalk);
        pcg.elevationBased = EditorGUILayout.Toggle("Biomes: Elevation", pcg.elevationBased);
        pcg.nsBased = EditorGUILayout.Toggle("Biomes: North South", pcg.nsBased);
        base.OnInspectorGUI();
    }

    private void InEditorGenerate()
    {
        if(PCGEnumertor != null)
        {
            if (!PCGEnumertor.MoveNext())
            {
                PCGEnumertor = null;
            }
        }
        
    }
}
