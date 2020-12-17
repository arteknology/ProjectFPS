using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DistAIV2 : MonoBehaviour, IDamageable, IHarpoonable
{
    private Animator _animator;

    private float _currentHealth;

    private State _currentState;

    private bool _isAlive = true;
    private readonly float _maxHealth = 80;
    private NavMeshAgent _navMeshAgent;
    private float _nextFire;
    private GameObject _player;

    private bool _playerIsInSight;

    public ParticleSystem blood, deathGeyser;
    public Transform bulletPoint;
    public float constant;

    public DoorScript Door;
    public float fireRate;

    public GameObject projectile;


    private Coroutine ShootProjectileRoutine;

    private float unhookedSince;

    private Coroutine WaitToEndStunRoutine;
    private bool _isFarEnough => Vector3.Distance(transform.position, _player.transform.position) > 20;
    
    private AudioSource _audio;
    public AudioClip ProjectileLaunched;
    public AudioClip Mort;

    public void TakeDamage(int amount)
    {
        _currentHealth = _currentHealth - amount;
        blood.Play();
        Debug.Log("Il me reste " + _currentHealth);
    }

    public void Harpooned() // LE MOMENT OÙ IL EST HARPONNÉ
    {
        if (!_isAlive) return;
        Debug.Log("Crochet crochet j't'ai accroché");
        _navMeshAgent.isStopped = true;
        unhookedSince = 0;
        _currentState = State.Hooked;
    }

    public void Released() // LE MOMENT OÙ IL EST RELÂCHÉ
    {
        if (!_isAlive) return;
        transform.position = PlayerHandler.releasedEnemy.position;
        _navMeshAgent.isStopped = false;
        _currentState = State.Stunned;
        if (WaitToEndStunRoutine != null) StopCoroutine(WaitToEndStunRoutine);
        WaitToEndStunRoutine = StartCoroutine(WaitToEndStun());
    }

    private void Awake()
    {
        _currentState = State.Idle;

        _currentHealth = _maxHealth;

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player");
        _animator = GetComponentInChildren<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        _isAlive = true;
        _playerIsInSight = true;
        _currentState = State.Idle;
        _nextFire = fireRate * Random.Range(0.8f, 1.2f);
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
                if (_playerIsInSight) _currentState = State.Attacking;
                break;

            case State.Attacking:
                WaitToShoot();
                break;

            case State.Running:
                SetAnimation("IsFleeing");
                Run();
                break;

            case State.Hooked:
                InHarpoon();
                SetAnimation("IsIdle");
                break;
            case State.Stunned:
                SetAnimation("IsIdle");
                break;

            case State.Dead:
                SetAnimation("IsDead");
                break;
        }

        if (_isFarEnough && _currentState != State.Hooked && unhookedSince > 2f) _currentState = State.Attacking;
        if (!_isFarEnough && _currentState != State.Hooked && unhookedSince > 2f) _currentState = State.Running;
    }

    private void Run()
    {
        _navMeshAgent.speed = 4.5f;
        var dirToPlayer = transform.position - _player.transform.position;
        var newPos = transform.position + dirToPlayer;
        _navMeshAgent.SetDestination(newPos);

        if (_isFarEnough) _currentState = State.Attacking;
    }

    private void WaitToShoot()
    {
        if (ShootProjectileRoutine == null) SetAnimation("IsIdle");

        if (!_isFarEnough)
        {
            _currentState = State.Running;
            return;
        }


        if (unhookedSince > 2f)
            if (Time.time > _nextFire && ShootProjectileRoutine == null)
                Shoot();
    }

    private void Shoot()
    {
        _navMeshAgent.speed = 0f;
        SetAnimation("IsShooting");
        if (ShootProjectileRoutine != null) StopCoroutine(ShootProjectileRoutine);
        ShootProjectileRoutine = StartCoroutine(ShootProjectile());
        _nextFire = Time.time + fireRate * Random.Range(0.8f, 1.2f);
    }

    private IEnumerator ShootProjectile()
    {
        if (_isAlive)
        {
            //Debug.Log("je commence mon shoot");
            yield return new WaitForSeconds(1f);
            //Debug.Log("je lance le projectile");
            var bullet = Instantiate(projectile, bulletPoint.position, bulletPoint.rotation);
            var bulletRb = bullet.GetComponent<Rigidbody>();
            _audio.PlayOneShot(ProjectileLaunched);
            bulletRb.velocity = (_player.transform.position - bullet.transform.position).normalized * constant;
            yield return new WaitForSeconds(0.5f);
            //Debug.Log("j'ai terminé mon attaque");
            ShootProjectileRoutine = null;
        }
    }

    private void InHarpoon() // CHAQUE UPDATE QUAND IL EST DANS LE HARPON
    {
        transform.position = PlayerHandler.Pointe.transform.position;
    }

    private IEnumerator WaitToEndStun()
    {
        while (unhookedSince < 2f) yield return null;
        _navMeshAgent.isStopped = false;
        _currentState = State.Idle;
        WaitToEndStunRoutine = null;
    }

    public void Die()
    {
        if (_isAlive == false) return;
        _isAlive = false;
        _navMeshAgent.isStopped = true;
        foreach (var box in GetComponentsInChildren<BoxCollider>()) box.isTrigger = true;
        _currentState = State.Dead;
        _audio.PlayOneShot(Mort);
        _animator.SetTrigger("Die");
        Door.RemoveEnemy();
    }

    private void SetAnimation(string animationSelected)
    {
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsFleeing", false);
        _animator.SetBool("IsShooting", false);
        _animator.SetBool("IsDead", false);

        _animator.SetBool(animationSelected, true);
    }

    private enum State
    {
        Idle,
        Attacking,
        Running,
        Hooked,
        Stunned,
        Dead
    }
}