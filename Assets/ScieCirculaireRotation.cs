using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScieCirculaireRotation : MonoBehaviour
{
    private float speed = 30f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(transform.up * speed * Time.deltaTime);
    }
}
