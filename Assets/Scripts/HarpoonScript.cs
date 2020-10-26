using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HarpoonScript : MonoBehaviour
{
    public string[] tagsToCheck;

    public float speed, returnSpeed, range, stopRange;

    [HideInInspector]
    public Transform caster, collidedWith;

    private LineRenderer line;
    private bool hasCollided;
    
    // Start is called before the first frame update
    void Start()
    {
        line = transform.Find("Line").GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (caster)
        {
            line.SetPosition(0, caster.position);
            line.SetPosition(1, transform.position);
            if (hasCollided)
            {
                transform.LookAt(caster);

                var dist = Vector3.Distance(transform.position, caster.position);
                if (dist < stopRange)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                var dist = Vector3.Distance(transform.position, caster.position);
                if (dist > range)
                {
                    Collision(null);
                }
            }
            
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (collidedWith)
            {
                collidedWith.transform.position = transform.position;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasCollided && tagsToCheck.Contains(other.tag))
        {
            Collision(other.transform);
        }
    }

    void Collision(Transform col)
    {
        speed = returnSpeed;
        hasCollided = true;

        if (col)
        {
            transform.position = col.position;
            collidedWith = col;
        }
    }
    
}
