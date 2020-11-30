using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CacAIV2 : MonoBehaviour, IDamageable
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _player;
    private IDamageable _playerHealth;
    private float _enemySpeed = 6f;
    private float _enemySpeedBack = 6f;
    private float _enemyStoppedSpeed = 0f;

    private Animator _animator;

    public GameObject pointe;

    private int _maxHealth = 10;
    private int _currentHealth;

    private bool _playerIsInMeleeRange => Vector3.Distance(transform.position, _player.transform.position) < 3;
    private bool _playerIsEnoughFar => Vector3.Distance(transform.position, _player.transform.position) > 10;

    public bool isGrabbed;
    private bool _hasAttacked;

    private State _currentState;

    public bool _playerIsInSight;

    private bool _isAlive;

    private enum State
    {
        Idle,
        Chasing,
        Melee,
        Flee,
        Hooked,
        Dead
    }

    private void Start()
    {
        _isAlive = true;
        _player = GameObject.FindWithTag("Player");
        _playerHealth = _player.GetComponent<IDamageable>();
        _currentHealth = _maxHealth;
        _animator = GetComponent<Animator>();
        _animator.SetBool("IsIdle", true);
        isGrabbed = false;
        _hasAttacked = false;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _playerIsInSight = true;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.Idle:
                if (!_playerIsInSight)
                {
                    _currentState = State.Idle;
                }
                else
                {
                    _currentState = State.Chasing;
                }
                if (_playerIsInMeleeRange)
                {
                    _currentState = State.Melee;
                }
                if (isGrabbed)
                {
                    _currentState = State.Hooked;
                }
                break;
            
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Melee:
                MeleeAttack();
                SetAnimation("IsMelee");
                return;
            case State.Flee:
                SetAnimation("IsFleeing");
                Flee();
                return;
            case State.Hooked:
                Harpooned(pointe.transform);
                SetAnimation("IsIdle");
                return;
            case State.Dead:
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_currentHealth <= 0 && _isAlive)
        {
            _currentHealth = 0;
            Die();
        }
        
    }

    private void MeleeAttack()
    {
        Debug.Log("PAF");
        SetAnimation("IsMelee");
        _playerHealth.TakeDamage(10);
        _currentState = State.Flee;
    }

    private void ChasePlayer()
    {
        _navMeshAgent.speed = _enemySpeed;
        Vector3 dirToPlayer = transform.position - _player.transform.position;
        Vector3 newPos = transform.position - dirToPlayer;
        _navMeshAgent.SetDestination(newPos);
        if (_playerIsInMeleeRange)
        {
            _currentState = State.Melee;
        }
    }

    private void Flee()
    {
        _navMeshAgent.speed = _enemySpeedBack;
        Vector3 dirToPlayer = transform.position + _player.transform.position;
        Vector3 newPos = transform.position + dirToPlayer;
        _navMeshAgent.SetDestination(newPos);

        if (_playerIsEnoughFar)
        {
            _currentState = State.Chasing;
        }
        
    }

    private void Harpooned(Transform col)
    {
        transform.position = col.transform.position;
    }

    public void Die()
    {
        if (_isAlive == false) return;
        _isAlive = false;
        Debug.Log("J'AI MAAAAAAAAAAAAAAAAL");
        _navMeshAgent.speed = 0;
        SetAnimation("IsDead");
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = _currentHealth - amount;
    }

    void SetAnimation(string animationSelected)
    {
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsChasing", false);
        _animator.SetBool("IsMelee", false);
        _animator.SetBool("IsFleeing", false);
        _animator.SetBool("IsDead", false);
        
        _animator.SetBool(animationSelected, true);
    }
}
