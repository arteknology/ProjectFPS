using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabReplacement))]
public class ReplaceByPrefab : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = (PrefabReplacement) target;
        if (GUILayout.Button("/!\\ REPLACE /!\\")) myScript.Replace();
    }
}