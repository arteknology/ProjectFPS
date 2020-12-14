using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawScript : MonoBehaviour
{
    public PlayerHandler player;

    public Animator _animator;
        
    private List<IDamageable> EnemiesInside = new List<IDamageable>();

    private void Awake()
    {
        //_animator = GetComponentInChildren<Animator>();
        //SetAnimation("IsIdle");
    }

    private void Update()
    {
        if (EnemiesInside.Count < 1)
        {
            CancelInvoke("Chainsaw");
            //SetAnimation("IsIdle");
        }
        
        if (Input.GetButtonDown("Fire1") && EnemiesInside.Count >0)
        {
            _animator.SetTrigger("ATTACK");
            InvokeRepeating("Chainsaw", 0f, 0.5f);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            CancelInvoke("Chainsaw");
            //SetAnimation("IsIdle");
            
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

    public void Chainsaw()
    {
        foreach (IDamageable enemy in EnemiesInside)
        {
            enemy.TakeDamage(10);
        }
        Debug.Log("COUPE COUPE COUPE");
    }
    
    /*void SetAnimation(string animationSelected)
    {
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsChainsaw", false);
        
        _animator.SetBool(animationSelected, true);
    }*/
}
