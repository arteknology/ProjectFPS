using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScieCirculaireScript : MonoBehaviour
{
    public int damage = 10;
    public float timeBetweenDamage = 1f;
    List<IDamageable> entitiesTouching = new List<IDamageable>();
    Coroutine MakingDamage;
    public Transform scies;


    void Update()
    {
        scies.Rotate(Vector3.up * Time.deltaTime * 180f);
    }


    IEnumerator ApplyDamage()
    {
        while (entitiesTouching.Count>0)
        {
            yield return new WaitForSeconds(timeBetweenDamage);
            foreach (IDamageable entity in entitiesTouching) entity.TakeDamage(damage);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        IDamageable entity = other.GetComponentInParent<IDamageable>();
        if (entity!=null && entitiesTouching.Contains(entity)==false)
        {
            entity.TakeDamage(damage);
            entitiesTouching.Add(entity);
            if (MakingDamage==null) MakingDamage = StartCoroutine(ApplyDamage());
        }
    }

    void OnTriggerExit(Collider other)
    {
        IDamageable entity = other.GetComponentInParent<IDamageable>();
        if (entity!=null && entitiesTouching.Contains(entity)) entitiesTouching.Remove(entity);
    }
}
