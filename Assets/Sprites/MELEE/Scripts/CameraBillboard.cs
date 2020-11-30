using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    Vector3 target = Vector3.zero;

    private void LateUpdate()
    {
        target = Camera.main.transform.position - transform.position;
        target.y = 0;
        transform.rotation = Quaternion.LookRotation(target);
    }
}