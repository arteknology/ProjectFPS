using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PrefabReplacement : MonoBehaviour
{
    [Header("OBJECTS REPLACED MUST BE CHILD AND CONTAIN FOLLOWING STRING IN NAME")]
    public string objectsName;

    [Header("THE PREFAB USED FOR REPLACEMENT")]
    public GameObject prefab;
    //[Header("USE WITH CAUTION. NO BACKUP!")]

    public void Replace()
    {
        var objectsToReplace = new List<GameObject>();
        foreach (Transform child in transform)
            if (child.name.Contains(objectsName) && child.gameObject.activeSelf)
                objectsToReplace.Add(child.gameObject);

        foreach (var toBeReplaced in objectsToReplace)
        {
            Debug.Log("Replacing " + toBeReplaced.name);
            var copy = (GameObject) PrefabUtility.InstantiatePrefab(prefab, transform);
            copy.transform.position = toBeReplaced.transform.position;
            copy.transform.rotation = toBeReplaced.transform.rotation;
            //Instantiate(prefab, toBeReplaced.transform.position, toBeReplaced.transform.rotation, transform);
            DestroyImmediate(toBeReplaced);
        }

        objectsToReplace = new List<GameObject>();
    }
}