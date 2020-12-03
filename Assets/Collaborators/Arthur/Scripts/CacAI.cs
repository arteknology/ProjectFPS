﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CacAI : MonoBehaviour
{
    private NavMeshAgent navMesh;
    private GameObject _player;
    //private Animator _anim;
    
    public float DistanceToAttack;
    public float AttackDist;
    public float GoBackDist;
    
    public int Life;
    public int PlayerDamages;
    public int Damages;
    
    public bool Grabbed;
    public bool _attacked;

    public DoorScript Door;
    public bool Dead = false;
    void Start()
    {
        _attacked = false;
        Grabbed = false;
        navMesh = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player");
        //_anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Life > 0)
        {
            if (!_attacked)
            {
                Run();
            }
            else
            {
                RunBack();
            }
        }
        else
        {
            Die();
        }

        if (Dead)
        {
            Die();
        }
    }

    //Movements
    private void Run()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (!Grabbed)
        {
            if (distance > DistanceToAttack )
            {
                navMesh.isStopped = false;
                Vector3 dirToPlayer = transform.position - _player.transform.position;
                Vector3 newPos = transform.position - dirToPlayer;
                navMesh.SetDestination(newPos);
                //_anim.SetBool("Run", true);
            }
            else
            {
                //_anim.SetBool("Run", false);
                navMesh.isStopped = true;
                Attack();
            }
        }
        else
        { 
            Stun();
        }
    }

    private void RunBack()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (!Grabbed)
        {
            if (distance < GoBackDist)
            {
                navMesh.isStopped = false;
                Vector3 dirToPlayer = transform.position - _player.transform.position;
                Vector3 newPos = transform.position + dirToPlayer;
                navMesh.SetDestination(newPos);
                //_anim.SetBool("RunBack", true);
            }
            else
            {
                //_anim.SetBool("RunBack", false);
                navMesh.isStopped = true;
                _attacked = false;
            }
        }
        else
        {
            Stun();
        }
    }
    
    private void Stun()
    {
        navMesh.isStopped = true;
        _attacked = false;
        //_anim.SetBool("Run", false);
        //_anim.SetBool("RunBack", false);
        StartCoroutine(WaitForStunToEnd());
    }
    //Attack & Die
    private void Die()
    {
        Door.RemoveEnemy(this.gameObject);
        navMesh.isStopped = true; 
        //_anim.SetTrigger("Die");
        Destroy(this.gameObject, 1f);
        
    }
    
    private void Attack()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= DistanceToAttack)
        {
            //_anim.SetBool("attack", true);
        }
        else
        {
            _attacked = false;
            //_anim.SetBool("attack", false);
        }
    }

    //Damages    
    private void TakeDamage()
    {
        Life -= PlayerDamages;
        Die();
    }
    
    
    public void DamagePlayer()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= AttackDist )
        {
            //_playerHandler.TakeDamage(Damages);
            //_anim.SetBool("attack", false);
            _attacked = true;
        }
        else
        {
            //_anim.SetBool("attack", false);
            _attacked = true;
        }
    }
//Triggers
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Harpoon"))
        {
            Grabbed = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Harpoon"))
        {
            Grabbed = true;
        }

        if (other.gameObject.CompareTag("Weapon"))
        {
            TakeDamage();
        }
    }
    
    //Routine
    IEnumerator WaitForStunToEnd() {
        // Wait a frame
        yield return null;
        // Wait 0.2 seconds
        yield return new WaitForSeconds(2f);
        Grabbed = false;
    }
}
