using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damages = 10;
    private AudioSource _audio;
    public AudioClip Touch;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }
    
    public void DestroyBullet()
    {
        Destroy(gameObject, 0.7f);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerHandler player = other.GetComponentInParent<PlayerHandler>();

        if (player != null)
        {
            player.TakeDamage(damages);
            _audio.PlayOneShot(Touch);
            DestroyBullet();
        }
        

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Harpoon"))
        {
            
        }
        
        else
        {
            _audio.PlayOneShot(Touch);
            DestroyBullet();
        }
    }
}
