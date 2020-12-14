using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScieCirculaireRotation : MonoBehaviour
{
    private float speed = 500f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.up * speed * Time.deltaTime);
    }
}
