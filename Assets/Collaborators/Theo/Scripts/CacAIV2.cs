using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CacAIV2 : MonoBehaviour, IDamageable, IHarpoonable
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _player;
    private IDamageable _playerHealth;
    private float _enemySpeed = 6f;
    private float _enemySpeedBack = 6f;
    private float _enemyStoppedSpeed = 0f;

    private Animator _animator;

    private int _maxHealth = 50;
    private int _currentHealth;

    private bool _playerIsInMeleeRange => Vector3.Distance(transform.position, _player.transform.position) < 3;

    public bool isGrabbed;
    private bool _hasAttacked;

    private State _currentState;

    public bool _playerIsInSight;

    private bool _isAlive;

    Coroutine attack;
    Coroutine fleeing;

    float unhookedSince = 0;

    public DoorScript Door;


    private enum State
    {
        Idle,
        Chasing,
        Melee,
        Flee,
        Hooked,
        Dead
    }

    void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _playerHealth = _player.GetComponent<IDamageable>();
        _animator = GetComponentInChildren<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        _isAlive = true;
        isGrabbed = false;
        _hasAttacked = false;
        _playerIsInSight = true;
        _currentState = State.Idle;
    }

    private void Update()
    {

        if (_isAlive)
        {
            unhookedSince += Time.deltaTime;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
        }



        switch (_currentState)
        {
            case State.Idle:
                SetAnimation("IsIdle");
                if (_playerIsInSight)
                {
                    _currentState = State.Chasing;
                }
                break;
            
            case State.Chasing:
                ChasePlayer();
            break;

            case State.Melee:
                MeleeAttack();
                SetAnimation("IsMelee");
            break;

            case State.Flee:
                SetAnimation("IsFleeing");
                Flee();
            break;

            case State.Hooked:
                InHarpoon();
                SetAnimation("IsIdle");
            break;

            case State.Dead:
                SetAnimation("IsDead");
            break;
        }
        
        
    }

    private void MeleeAttack()
    {
        SetAnimation("IsMelee");
        if (attack ==null)
        {
            attack = StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        Debug.Log("PAF");
        _navMeshAgent.speed = 0;
        _navMeshAgent.SetDestination(transform.position);
        yield return new WaitForSeconds(0.2f);
        _playerHealth.TakeDamage(10);
        yield return new WaitForSeconds(0.25f);
        _currentState = State.Flee;
        attack = null;
    }



    private void ChasePlayer()
    {
        SetAnimation("IsChasing");
        _navMeshAgent.speed = _enemySpeed;
        _navMeshAgent.SetDestination(_player.transform.position);
        if (_playerIsInMeleeRange && unhookedSince>2f)
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

        if (fleeing==null) fleeing = StartCoroutine(Fleeing());
    }

    IEnumerator Fleeing()
    {
        yield return new WaitForSeconds(2f+ UnityEngine.Random.value*2f);
        _currentState = State.Idle;
        Debug.Log("Je retourne au contact");
        fleeing = null;
    }



    public void Harpooned() // LE MOMENT OÙ IL EST HARPONNÉ
    {
        if (!_isAlive) return;
        Debug.Log("Crochet crochet j't'ai accroché");
        _navMeshAgent.isStopped = true;
        unhookedSince = 0;
        _currentState = State.Hooked;
        isGrabbed = true;
    }

    void InHarpoon() // CHAQUE UPDATE QUAND IL EST DANS LE HARPON
    {
        transform.position = PlayerHandler.Pointe.transform.position;
    }

    public void Released() // LE MOMENT OÙ IL EST RELÂCHÉ
    {
        if (!_isAlive) return;
        isGrabbed = false;
        transform.position = PlayerHandler.releasedEnemy.position;
        _navMeshAgent.isStopped = false;
        _currentState = State.Idle;
    }

    public void Die()
    {
        if (_isAlive == false) return;
        _isAlive = false;
        Debug.Log("J'AI MAAAAAAAAAAAAAAAAL");
        _navMeshAgent.isStopped = true;
        foreach (BoxCollider box in GetComponentsInChildren<BoxCollider>())
        {
            box.isTrigger = true;
        }
        _currentState = State.Dead;
        _animator.SetTrigger("Die");
        Door.GetComponent<DoorScript>().RemoveEnemy(this.gameObject);
    }

    public void TakeDamage(int amount)
    {
        if (!_isAlive) return;
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
