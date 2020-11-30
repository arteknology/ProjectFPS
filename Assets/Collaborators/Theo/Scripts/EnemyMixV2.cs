using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public class EnemyMixV2 : MonoBehaviour
{
    private NavMeshAgent _navMesh;
    private GameObject _player;

    public GameObject pointe;

    public GameObject projectile;
    public Transform bulletPoint;
    public float constant;
    public float fireRate = 0.5f;
    private float nextFire;

    public float distanceFromPlayer;
    public float walkDistance;
    public float distanceForMelee;

    public int maxHealth = 150;
    public int currentHealth;

    public bool isGrabbed;
    public bool isStunned;

    private State _state;
    public enum State
    {
        Normal,
        Walking,
        MeleeAttack,
        DistanceAttack,
        Hooked,
        Stunned,
        Dying
    }

    void Start()
    {
        _state = State.Normal;
        currentHealth = maxHealth;
        _navMesh = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Normal:

                break;

            case State.DistanceAttack:
                DistanceAttack();
                break;
            
            case State.Walking:
                WalkingTowardPlayer();
                break;
            
            case State.MeleeAttack:
                MeleeAttack();
                break;
            
            case State.Hooked:
                HarpoonDragged(pointe.transform);
                break;

            case State.Stunned:

                break;
            
            case State.Dying:

                break;
        }

        distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceFromPlayer >= walkDistance && distanceFromPlayer >= distanceForMelee && !isGrabbed || !isStunned)
        {
            _state = State.DistanceAttack;
        }
        if (distanceFromPlayer <= walkDistance && !isGrabbed && !isStunned)
        {
            _state = State.Walking;
        }

        if (isGrabbed) _state = State.Hooked;

        if (currentHealth <= 0) _state = State.Dying;
    }

    private void WalkingTowardPlayer()
    {
        _navMesh.speed = 3.5f;
        Vector3 dirToPlayer = transform.position - _player.transform.position;
        Vector3 newPos = transform.position - dirToPlayer;
        _navMesh.SetDestination(newPos);
        if (distanceFromPlayer <= distanceForMelee)
        {
            _state = State.MeleeAttack;
        }
    }

    private void DistanceAttack()
    {
        if (Time.time > nextFire)
        {
            _navMesh.speed = 0f;
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(projectile, bulletPoint.position, bulletPoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.velocity = (_player.transform.position - bullet.transform.position).normalized * constant;
        }
    }

    private void MeleeAttack()
    {
        _navMesh.speed = 0f;
    }
    
    public void HarpoonDragged(Transform col)
    {
        _navMesh.speed = 0f;
        transform.position = col.transform.position;
        //isStunned = true;
        //Invoke(nameof(UnStunned), 3f);
    }

    private void UnStunned()
    {
        isStunned = false;
    }
}
