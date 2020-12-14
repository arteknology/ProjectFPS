using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damages = 10;
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerHandler player = other.GetComponentInParent<PlayerHandler>();

        if (player != null)
        {
            player.TakeDamage(damages);
            DestroyBullet();
        }
        

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Harpoon"))
        {
            
        }
        else
        {
            Debug.Log("BOOM");
            DestroyBullet();
        }
    }
}
