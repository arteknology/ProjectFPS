using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HarpoonScript : MonoBehaviour
{
    /*public string[] tagsToCheck;

    public float speed, returnSpeed, range, stopRange;

    [HideInInspector]
    public Transform caster, collidedWith;

    private LineRenderer line;
    private bool hasCollided;

    private bool _enemyCollided = false, _wallCollided = false;
    
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
                    _enemyCollided = false;
                    _wallCollided = false;
                }
            }
            else
            {
                var dist = Vector3.Distance(transform.position, caster.position);
                if (dist > range)
                {
                    CollisionEnemy(null);
                    _enemyCollided = false;
                    _wallCollided = false;
                }
            }
            
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (collidedWith && _enemyCollided == true)
            {
                collidedWith.transform.position = transform.position;
            }
            if (collidedWith && _wallCollided == true)
            {
                GetComponent<PlayerControls>().HarpoonMovement();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasCollided && CompareTag("Enemy"))
        {
            _enemyCollided = true;
            CollisionEnemy(other.transform);
        }
        if (!hasCollided && CompareTag("Wall"))
        {
            _wallCollided = true;
            CollisionWall(other.transform);
        }
    }

    void CollisionEnemy(Transform col)
    {
        speed = returnSpeed;
        hasCollided = true;

        if (col)
        {
            transform.position = col.position;
            collidedWith = col;
        }
    }
    void CollisionWall(Transform col)
    {
        speed = returnSpeed;
        hasCollided = true;

        if (col)
        {
            transform.position = col.position;
            collidedWith = col;
        }
    }*/

    
}
