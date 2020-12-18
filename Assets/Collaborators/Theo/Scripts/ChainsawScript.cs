using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawScript : MonoBehaviour
{
    public PlayerHandler player;

    public Animator _animator;
        
    private List<IDamageable> EnemiesInside = new List<IDamageable>();

    private AudioSource _audio;
    public AudioClip Frappe;
    
    public GameObject pointeImage;
    
    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (EnemiesInside.Count < 1)
        {
            CancelInvoke("Chainsaw");
        }
        
        if (Input.GetButtonDown("Fire1") && EnemiesInside.Count >0 && PauseMenu.GameIsPaused == false)
        {
            if (!_audio.isPlaying)
            {
                _audio.PlayOneShot(Frappe);
            }

            _animator.SetTrigger("ATTACK");
            pointeImage.SetActive(false);
            StartCoroutine(WaitForDamage());
            StartCoroutine(WaitForPointe());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            CancelInvoke("Chainsaw");
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            IDamageable enemy = collider.GetComponentInParent<IDamageable>();
            if (enemy != null && !EnemiesInside.Contains(enemy))
            {
                EnemiesInside.Add(enemy);
            }
        }

        if (EnemiesInside.Count > 0)
        {
            player.isDetectingEnemy = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable enemy = other.GetComponentInParent<IDamageable>();
        if (enemy != null && EnemiesInside.Contains(enemy))
        {
            EnemiesInside.Remove(enemy);
        }

        if (EnemiesInside.Count < 1)
        {
            player.isDetectingEnemy = false;
        }
    }

    public IEnumerator WaitForDamage()
    {
        yield return new WaitForSeconds(0.3f);
        Chainsaw();

    }
    public IEnumerator WaitForPointe()
    {
        yield return new WaitForSeconds(0.8f);
        pointeImage.SetActive(true);
    }

    public void Chainsaw()
    {
        foreach (IDamageable enemy in EnemiesInside)
        {
            enemy.TakeDamage(10);
        }
        ScreenShake.Shake(2f);
    }
    
}
