using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpinScript : MonoBehaviour
{
    public Transform collidedWith;

    private void Update()
    {
        if (collidedWith) { collidedWith.transform.position = transform.position; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Collision(other.transform);
        }

        if (other.CompareTag("Wall"))
        {
            
        }
    }

    void Collision(Transform col)
    {
        if (col)
        {
            transform.position = col.position;
            collidedWith = col;
        }
    }
}
