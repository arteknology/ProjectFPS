using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CacAIV2 : MonoBehaviour, IDamageable
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _player;
    private float _enemySpeed = 6f;
    private float _enemySpeedBack = -6f;
    private float _enemyStoppedSpeed = 0f;

    private Animator _animator;

    public GameObject pointe;

    private int _maxHealth = 80;
    private int _currentHealth;

    private bool _playerIsInMeleeRange => Vector3.Distance(transform.position, _player.transform.position) < 2;
    private bool _playerIsEnoughFar => Vector3.Distance(transform.position, _player.transform.position) > 10;

    public bool isGrabbed;
    private bool _hasAttacked;

    private State _currentState;

    public bool _playerIsInSight;

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
        _currentHealth = _maxHealth;
        _animator = GetComponent<Animator>();
        _animator.SetBool("isIdle", true);
        isGrabbed = false;
        _hasAttacked = false;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.Idle:
                if (!_playerIsInSight)
                {
                    _currentState = State.Chasing;
                }
                else if (_playerIsInSight)
                {
                    _currentState = State.Chasing;
                }
                else if (_playerIsInMeleeRange)
                {
                    _currentState = State.Melee;
                }
                else if (_hasAttacked)
                {
                    _currentState = State.Flee;
                }
                else if (isGrabbed)
                {
                    _currentState = State.Hooked;
                }
                break;
            
            case State.Chasing:
                ChasePlayer();
                _animator.SetBool("IsIdle", false);
                _animator.SetBool("IsChasing", true);
                _animator.SetBool("IsMelee", false);
                _animator.SetBool("IsFleeing", false);
                break;
            case State.Melee:
                MeleeAttack();
                _animator.SetBool("IsIdle", false);
                _animator.SetBool("IsChasing", false);
                _animator.SetBool("IsMelee", true);
                _animator.SetBool("IsFleeing", false);
                return;
            case State.Flee:
                _animator.SetBool("IsIdle", false);
                _animator.SetBool("IsChasing", false);
                _animator.SetBool("IsMelee", false);
                _animator.SetBool("IsFleeing", true);
                Flee();
                return;
            case State.Hooked:
                Harpooned(pointe.transform);
                _animator.SetBool("IsIdle", true);
                _animator.SetBool("IsChasing", false);
                _animator.SetBool("IsMelee", false);
                _animator.SetBool("IsFleeing", false);
                return;
            case State.Dead:
                Die();
                _animator.SetBool("IsIdle", false);
                _animator.SetBool("IsChasing", false);
                _animator.SetBool("IsMelee", false);
                _animator.SetBool("IsFleeing", false);
                _animator.SetBool("IsDead", true);
                
                return;
            default:
                throw new ArgumentOutOfRangeException();
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

    private void ChasePlayer()
    {
        _navMeshAgent.speed = _enemySpeed;
        Vector3 dirToPlayer = transform.position - _player.transform.position;
        Vector3 newPos = transform.position + dirToPlayer;
        _navMeshAgent.SetDestination(newPos);
    }

    private void Flee()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        _navMeshAgent.speed = _enemySpeedBack;
        Vector3 dirToPlayer = transform.position - _player.transform.position;
        Vector3 newPos = transform.position + dirToPlayer;
        _navMeshAgent.SetDestination(newPos);

    }

    private void Harpooned(Transform col)
    {
        transform.position = col.transform.position;
    }

    public void Die()
    {
        Debug.Log("J'AI MAAAAAAAAAAAAAAAAL");
        _navMeshAgent.speed = _enemyStoppedSpeed;
        _animator.SetBool("isDead", true);
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = _currentHealth - amount;
    }
}
