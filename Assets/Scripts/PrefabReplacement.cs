using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[System.Serializable]
public class PrefabReplacement: MonoBehaviour
{
    [Header("THE PREFAB USED FOR REPLACEMENT")]
    public GameObject prefab;
    [Header("OBJECTS REPLACED MUST BE CHILD AND CONTAIN FOLLOWING STRING IN NAME")]
    public string objectsName;
    //[Header("USE WITH CAUTION. NO BACKUP!")]

    public void Replace()
    {
        List<GameObject> objectsToReplace = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.name.Contains(objectsName) && child.gameObject.activeSelf) objectsToReplace.Add(child.gameObject);
        }

        foreach(GameObject toBeReplaced in objectsToReplace)
        {
            Debug.Log("Replacing "+toBeReplaced.name);
            GameObject copy = (GameObject) PrefabUtility.InstantiatePrefab(prefab,transform);
            copy.transform.position = toBeReplaced.transform.position;
            copy.transform.rotation = toBeReplaced.transform.rotation;
            //Instantiate(prefab, toBeReplaced.transform.position, toBeReplaced.transform.rotation, transform);
            DestroyImmediate(toBeReplaced);
        }
        objectsToReplace = new List<GameObject>();
    }
}


[CustomEditor(typeof(PrefabReplacement))]
public class ReplaceByPrefab : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrefabReplacement myScript = (PrefabReplacement)target;
        if(GUILayout.Button("/!\\ REPLACE /!\\"))
        {
            myScript.Replace();
        }
    }
}