using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.Animations;

public class CacAI : MonoBehaviour, IDamageable
{
    private NavMeshAgent navMesh;
    private GameObject _player;

    public GameObject Pointe;
    //private Animator _anim;
    
    public float DistanceToAttack;
    public float AttackDist;
    public float GoBackDist;

    public int MaxLife = 50;
    public int CurrentLife;
    
    public int PlayerDamages;
    public int Damages;
    
    public bool Grabbed;
    public bool _attacked;

    public Animator animator;
    
    void Start()
    {
        CurrentLife = MaxLife;
        _attacked = false;
        Grabbed = false;
        navMesh = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player");
        //_anim = GetComponent<Animator>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (CurrentLife > 0)
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
                animator.SetBool("IsChasing", true);
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
                
            }
            else
            {
                //_anim.SetBool("RunBack", false);
                navMesh.isStopped = true;
                _attacked = false;
                animator.SetBool("IsFleeing", false);
                animator.SetBool("IsChasing", true);
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
        HarpoonDragged(Pointe.transform);
        StartCoroutine(WaitForStunToEnd());
    }
    //Attack & Die
    private void Die()
    {
        navMesh.isStopped = true; 
        //_anim.SetTrigger("Die");
        Destroy(this.gameObject, 1f);
        
        animator.SetBool("IsDead", true);
    }
    
    private void Attack()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= DistanceToAttack)
        {
            //_anim.SetBool("attack", true);
            animator.SetBool("IsAttacking", true);
            animator.SetBool("IsChasing", false);
        }
        else
        {
            _attacked = false;
            //_anim.SetBool("attack", false);
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsFleeing", true);
        }
    }

    //Damages    
    public void TakeDamage(int amount)
    {
        CurrentLife = CurrentLife - amount;
        Debug.Log("Il me reste " + CurrentLife);
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

        //if (other.gameObject.CompareTag("Weapon"))
        //{
            //TakeDamage();
        //}
    }
    
    //Routine
    IEnumerator WaitForStunToEnd() {
        // Wait a frame
        yield return null;
        // Wait 0.2 seconds
        yield return new WaitForSeconds(2f);
        Grabbed = false;
    }
    
    public void HarpoonDragged(Transform col)
    {
        transform.position = col.transform.position;

    }
}
