using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMixAIV3 : MonoBehaviour, IDamageable
{
    //NavMeshStuff
    private NavMeshAgent _navMesh;
    private GameObject _player;
    private float _enemySpeed = 4.5f;
    private float _enemyStoppedSpeed = 0f;
    
    public GameObject pointe;
    
    private int _maxHealth = 150;
    private int _currentHealth;
    
    

    //Shoot stuff
    public GameObject projectilePrefab;
    public Transform bulletPoint;
    public float constant;
    public float fireRate = 0.5f;
    private float _nextFire;
    
    //Distance to the player stuff
    private bool _playerIsInSight;
    public float distanceFromPlayer;
    private bool _playerIsInMeleeRange => Vector3.Distance(transform.position, _player.transform.position) < 2;
    private bool _playerIsInRangeAttack => Vector3.Distance(transform.position, _player.transform.position) <20;

    //Attack stuff
    private bool _haveRangedAttack;
    private bool _haveMeleeAttack;

    public bool isGrabbed;

    private State _currentState;

    private enum State
    {
        Idle,
        Walking,
        Melee,
        Distance,
        Hooked,
        Flee,
        Dead
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.Idle:
                if (_haveRangedAttack && _playerIsInRangeAttack)
                {
                    _currentState = State.Distance;
                }
                else if (_haveMeleeAttack && _playerIsInMeleeRange)
                {
                    _currentState = State.Melee;
                }
                else if (_playerIsInSight)
                {
                    _currentState = State.Walking;
                }
                
                else if (isGrabbed)
                {
                    _currentState = State.Hooked;
                }
                break;
            
            case State.Walking:
                ChasePlayer();
                if (!_playerIsInSight)
                {
                    _currentState = State.Idle;
                }
                else if (_playerIsInRangeAttack)
                {
                    _currentState = State.Distance;
                }
                break;
            
            case State.Melee:
                MeleeAttack();
                if (!_playerIsInMeleeRange)
                {
                    _currentState = State.Idle;
                }
                break;
            
            case State.Distance:
                DistanceAttack();
                if (!_playerIsInMeleeRange && _haveMeleeAttack)
                {
                    _currentState = State.Melee;
                }
                else if (!_playerIsInMeleeRange && !_haveMeleeAttack)
                {
                    _currentState = State.Flee;
                }
                break;
            
            case State.Hooked:
                IsHarpooned(pointe.transform);
                if (!isGrabbed)
                    _currentState = State.Idle;
                break;
            
            case State.Flee:
                Flee();
                break;
            
            case State.Dead:
                Die();
                break;
            
            default:
                throw  new ArgumentOutOfRangeException();
        }

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _currentState = State.Dead;
        }
        
    }


    private void MeleeAttack()
    {
        Debug.Log("PAF");
    }

    private void DistanceAttack()
    {
        if (Time.time > _nextFire)
        {
            _navMesh.speed = 0f;
            _nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(projectilePrefab, bulletPoint.position, bulletPoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.velocity = (_player.transform.position - bullet.transform.position).normalized * constant;
        } 
    }

    private void ChasePlayer()
    {
        _navMesh.speed = _enemySpeed;
        Vector3 dirToPlayer = transform.position - _player.transform.position;
        Vector3 newPos = transform.position + dirToPlayer;
        _navMesh.SetDestination(newPos);
    }

    private void Flee()
    {
        
    }

    private void IsHarpooned(Transform col)
    {
        transform.position = col.transform.position;
    }

    private void Die()
    {
        Debug.Log("AAAAAAAAAAAAAAAAH");
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = _currentHealth - amount;
    }
}
