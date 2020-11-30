using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidScript : MonoBehaviour
{
    public List<IDamageable> EntitiesInside = new List<IDamageable>();


    private void Start()
    {
        InvokeRepeating("DamageEntities", 0f, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Je détecte un truc");
        IDamageable entity = other.GetComponentInParent<IDamageable>();
        EntitiesInside.Add(entity);
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable entity = other.GetComponentInParent<IDamageable>();
        Debug.Log(entity + "S'en va");
        EntitiesInside.Remove(entity);
    }

    void DamageEntities()
    {
        foreach (IDamageable entity in EntitiesInside)
        {
            Debug.Log("Ouille");
            entity.TakeDamage(5);
        }

        if (EntitiesInside == null)
        {
            Debug.Log("Y'a personne wesh");
        }

    }
}
