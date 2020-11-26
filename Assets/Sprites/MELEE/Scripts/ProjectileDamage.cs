using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int Damages;
    public void DestroyBullet()
    {
        Destroy(this.gameObject);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //_player.TakeDamage(Damages);
            DestroyBullet();
        }
        //TODO: Destroy bullet when collide Walls/Ground/Acid
    }
}
