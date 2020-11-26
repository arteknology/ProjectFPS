using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidScript : MonoBehaviour
{
    public List<IDamageable> EntitiesInside = new List<IDamageable>();

    private void Update()
    {
        if (EntitiesInside.Count > 0)
        {
            InvokeRepeating("DamageEntities", 0f, 1f);
        }

        if (EntitiesInside.Count <= 0)
        {
            CancelInvoke("DamageEntities");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Je détecte un truc");
        IDamageable entity = other.GetComponentInParent<IDamageable>();
        EntitiesInside.Add(entity);
    }

    void DamageEntities()
    {
        foreach (IDamageable entity in EntitiesInside)
        {
            Debug.Log("Ouille");
            entity.TakeDamage(20);
        }
        
    }
}
