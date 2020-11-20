using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DistAI : MonoBehaviour
{
    private NavMeshAgent navMesh;
    private GameObject _player;
    //private Animator _anim;

    public GameObject Projectile;
    public Transform BulletPoint;
    public float Constant;
    public float fireRate = 0.5f;
    private float nextFire = 0.0f;
    public float DistanceToAttack;
    
    public int Life;
    public int PlayerDamages;
    
    public bool Grabbed;
    private bool _attacked;
    
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
            Run();
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
            if (distance < DistanceToAttack)
            {

                    navMesh.isStopped = false;
                    Vector3 dirToPlayer = transform.position - _player.transform.position;
                    Vector3 newPos = transform.position + dirToPlayer;
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
        navMesh.isStopped = true; 
        //_anim.SetTrigger("Die");
        Destroy(this.gameObject, 1f);
        
    }
    
    private void Attack()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (!Grabbed)
        {
            if (distance >= DistanceToAttack)
            {
                //_anim.SetBool("Attack", true);
                Shoot();
            }
            /*else
            {
                //_anim.SetBool("Attack", false);
                Run();
            }*/
        }
        else
        { 
            Stun();
        }
    }

    public void Shoot()
    {
        if (Time.time > nextFire )
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(Projectile, BulletPoint.position, BulletPoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.velocity = (_player.transform.position - bullet.transform.position).normalized * Constant;
            //_anim.SetBool("Attack", false);
        }
    }

    //Damages    
    private void TakeDamage()
    {
        Life -= PlayerDamages;
        Die();
    }
    
    
//Triggers
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Harpon"))
        {
            Grabbed = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Harpon"))
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
