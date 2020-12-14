using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMixAIV3 : MonoBehaviour, IDamageable, IHarpoonable
{
    //NavMeshStuff
    private NavMeshAgent _navMeshAgent;
    private GameObject _player;
    private float _enemySpeed = 4.5f;
    private float _enemyStoppedSpeed = 0f;
    
    private Animator _animator;
    
    private int _maxHealth = 130;
    private int _currentHealth;
    private IDamageable _playerHealth;
    

    //Shoot stuff
    public GameObject projectilePrefab;
    public Transform bulletPoint;
    public float constant;
    public float fireRate = 0.5f;
    private float _nextFire;
    
    //Distance to the player stuff
    private bool _playerIsInSight;
    public float distanceFromPlayer;
    private bool _playerIsInMeleeRange => Vector3.Distance(transform.position, _player.transform.position) < 3;
    private bool _playerIsOnWalkingDistance => Vector3.Distance(transform.position, _player.transform.position) < 10;
    private bool _playerIsInRangeAttack => Vector3.Distance(transform.position, _player.transform.position) > 20;

    private bool _hasAttacked;

    public bool isGrabbed;

    private Coroutine attack;

    private State _currentState;

    private bool _isAlive;
    
    public DoorScript Door;
    
    float unhookedSince = 0;

    public ParticleSystem blood, deathGeyser;
    private enum State
    {
        Idle,
        Walking,
        Melee,
        Distance,
        Hooked,
        Stunned,
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
                if (_playerIsInRangeAttack)
                {
                    _currentState = State.Distance;
                }
                if (_playerIsOnWalkingDistance)
                {
                    _currentState = State.Walking;
                }
                if (_playerIsInMeleeRange)
                {
                    _currentState = State.Melee;
                }
                if (!_playerIsInSight)
                {
                    _currentState = State.Idle;
                }
                
                /*if (isGrabbed)
                {
                    _currentState = State.Hooked;
                }*/
                break;
            
            case State.Walking:
                ChasePlayer();
                SetAnimation("IsChasing");
                if (!_playerIsInSight)
                {
                    _currentState = State.Idle;
                }
                if (_playerIsInRangeAttack && unhookedSince>2f)
                {
                    _currentState = State.Distance;
                }

                if (_playerIsInMeleeRange && unhookedSince>2f)
                {
                    _currentState = State.Melee;
                }
                break;
            
            case State.Melee:
                MeleeAttack();
                SetAnimation("IsMelee");
                if (_playerIsInRangeAttack)
                {
                    _currentState = State.Distance;
                }

                if (_playerIsOnWalkingDistance)
                {
                    _currentState = State.Walking;
                }
                
                if (_playerIsInMeleeRange)
                {
                    _currentState = State.Melee;
                }
                break;
            
            case State.Distance:
                WaitToShoot();
                
                if (_playerIsInMeleeRange)
                {
                    _currentState = State.Melee;
                }
                if (_playerIsOnWalkingDistance)
                {
                    _currentState = State.Melee;
                }
                break;
            
            case State.Hooked:
                SetAnimation("IsIdle");
                InHarpoon();
                break;
            
            case State.Stunned:
                SetAnimation("IsIdle");
                break;

            case State.Dead:
                Die();
                break;
            
            default:
                throw  new ArgumentOutOfRangeException();
        }

    }


    private void MeleeAttack()
    {
        SetAnimation("IsMelee");
        if (unhookedSince > 2f)
        {
            if (attack == null)
            {
                attack = StartCoroutine(Attack());
            }
        }
    }
    
    IEnumerator Attack()
    {
        Debug.Log("PAF");
        _navMeshAgent.speed = 0;
        _navMeshAgent.SetDestination(transform.position);
        yield return new WaitForSeconds(1f);
        _playerHealth.TakeDamage(10);
        yield return new WaitForSeconds(0.25f);
        _currentState = State.Idle;
        attack = null;
    }
    
    
    private void WaitToShoot()
    {
        if (ShootProjectileRoutine==null) SetAnimation("IsIdle");
        
        if (unhookedSince > 2f)
        {
            if (Time.time > _nextFire && ShootProjectileRoutine==null)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        _navMeshAgent.speed = 0f;
        SetAnimation("IsDistance");
        if (ShootProjectileRoutine!=null) StopCoroutine(ShootProjectileRoutine);
        ShootProjectileRoutine = StartCoroutine(ShootProjectile());
        _nextFire = Time.time + fireRate * UnityEngine.Random.Range(0.8f,1.2f);
    }
    
    private Coroutine ShootProjectileRoutine;
   
    IEnumerator ShootProjectile()
    {
        if (_isAlive)
        {
            Debug.Log("je commence mon shoot");
            yield return new WaitForSeconds(0.5f);
            Debug.Log("je lance le projectile");
            GameObject bullet = Instantiate(projectilePrefab, bulletPoint.position, bulletPoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.velocity = (_player.transform.position - bullet.transform.position).normalized * constant;
            yield return new WaitForSeconds(1f);
            Debug.Log("j'ai terminé mon attaque");
            ShootProjectileRoutine = null;
        }
    }
    

    private void ChasePlayer()
    {
        if (unhookedSince > 2f)
        {
            _navMeshAgent.speed = _enemySpeed;
            Vector3 dirToPlayer = transform.position - _player.transform.position;
            Vector3 newPos = transform.position - dirToPlayer;
            _navMeshAgent.SetDestination(newPos);
        }
    }
    

    private void Die()
    { 
        if (_isAlive == false) return;
        _isAlive = false;
        _navMeshAgent.isStopped = true;
        foreach (BoxCollider box in GetComponentsInChildren<BoxCollider>())
        {
            box.isTrigger = true;
        }
        _animator.SetTrigger("Die");
        deathGeyser.Play();
        Door.RemoveEnemy();
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = _currentHealth - amount;
        blood.Play();
        Debug.Log(_currentHealth);
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
    public void Released() // LE MOMENT OÙ IL EST RELÂCHÉ
    {
        if (!_isAlive) return;
        isGrabbed = false;
        transform.position = PlayerHandler.releasedEnemy.position;
        _navMeshAgent.isStopped = false;
        _currentState = State.Stunned;
        StopAllCoroutines();
        StartCoroutine(WaitToEndStun());
    }
    
    IEnumerator WaitToEndStun()
    {
        while (unhookedSince < 2f)
        {
            yield return null;
        }
        _navMeshAgent.isStopped = false;
        _currentState = State.Idle;
    }
    
    void InHarpoon() // CHAQUE UPDATE QUAND IL EST DANS LE HARPON
    {
        transform.position = PlayerHandler.Pointe.transform.position;
    }
    
    void SetAnimation(string animationSelected)
    {
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsChasing", false);
        _animator.SetBool("IsMelee", false);
        _animator.SetBool("IsDistance", false);
        _animator.SetBool("IsDead", false);
        
        _animator.SetBool(animationSelected, true);
        
    }
}
