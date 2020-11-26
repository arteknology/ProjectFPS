﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyTestScript : MonoBehaviour, IDamageable
{
    public GameObject Pointe; 
    public bool hasCollided = false;
    

    private void Update()
    {
        if (hasCollided)
        {
            HarpoonDragged(Pointe.transform);
        }
        else
        {
            
        }
    }

    public void TakeDamage(int amount)
    {
        
    }

    public void HarpoonDragged(Transform col)
    {
        transform.position = col.transform.position;

    }
}
