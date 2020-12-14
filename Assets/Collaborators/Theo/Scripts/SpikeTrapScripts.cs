using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapScripts : MonoBehaviour
{
    public float startDelay, delayBetweenDamage;
    public int damage = 20;
    public GameObject pics;
    bool activated;

    List<IDamageable> entitiesInside = new List<IDamageable>();

    Coroutine picroutine;

    void Start()
    {
        pics.SetActive(false);
    }


    IEnumerator StartPics()
    {
        //Debug.Log("ATTENTION LES PICS VONT SORTIR");
        yield return new WaitForSeconds(startDelay);
        //Debug.Log("ON BALANCE LES PICS");
        pics.SetActive(true);
        float chrono = 0;
        while (entitiesInside.Count >0)
        {
            chrono+= delayBetweenDamage;
            ApplyDamage();
            yield return new WaitForSeconds(delayBetweenDamage);
        }

        if (chrono<2f) yield return new WaitForSeconds(2f-chrono);
        RentrerPics();
    }

    void ApplyDamage()
    {
        foreach (IDamageable entity in entitiesInside) entity.TakeDamage(damage);
    }

    void RentrerPics()
    {
        pics.SetActive(false);
    }




    
    private void OnTriggerEnter(Collider other)
    {
        IDamageable entity = other.GetComponentInParent<IDamageable>();
        if (entity!=null && !entitiesInside.Contains(entity))
        {
            entitiesInside.Add(entity);
            if (pics.activeSelf) entity.TakeDamage(damage);
            if (picroutine==null) picroutine = StartCoroutine(StartPics());
        }
    }


    private void OnTriggerExit(Collider other)
    {
        IDamageable entity = other.GetComponentInParent<IDamageable>();
        if (entity!=null && entitiesInside.Contains(entity))
        {
            entitiesInside.Remove(entity);
            /*if (entitiesInside.Count <1)
            {
                StopAllCoroutines();
                picroutine = null;
                RentrerPics();
            }*/
        }
    }


}
