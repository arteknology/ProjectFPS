using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScieCirculaireScript : MonoBehaviour
{
    public int damage = 10;
    public float timeBetweenDamage = 1f;
    List<IDamageable> entitiesTouching = new List<IDamageable>();


    void Start()
    {
        InvokeRepeating("ApplyDamage", 0, timeBetweenDamage);
    }

    void ApplyDamage()
    {
        foreach (IDamageable entity in entitiesTouching) entity.TakeDamage(damage);
    }


    void OnTriggerEnter(Collider other)
    {
        IDamageable entity = other.GetComponentInParent<IDamageable>();
        if (entity!=null && entitiesTouching.Contains(entity)==false) entitiesTouching.Add(entity);
    }

    void OnTriggerExit(Collider other)
    {
        IDamageable entity = other.GetComponentInParent<IDamageable>();
        if (entity!=null && entitiesTouching.Contains(entity)) entitiesTouching.Remove(entity);
    }
}
