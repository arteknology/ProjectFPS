using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScieCirculaireRotation : MonoBehaviour
{
    private float speed = 300f;

    public bool isRightToLeft;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRightToLeft)
        {
            transform.Rotate(transform.up * speed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(transform.up *-speed * Time.deltaTime);
        }
    }
}
