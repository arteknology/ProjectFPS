using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class DistAIV2 : MonoBehaviour, IDamageable, IHarpoonable
{
   private NavMeshAgent _navMeshAgent;
   private GameObject _player;

   public GameObject projectile;
   public Transform bulletPoint;
   public float constant;
   public float fireRate;
   private float _nextFire;
   
   private bool _isFarEnough => Vector3.Distance(transform.position, _player.transform.position) >20 ;
   
   private float _currentHealth;
   private float _maxHealth = 80;

   private State _currentState;

   private float unhookedSince = 0;

   private bool _isAlive = true;

   private bool _playerIsInSight;

   private enum State
   {
      Idle,
      Attacking,
      Running,
      Hooked,
      Dead
   }

   private void Awake()
   {
      _currentState = State.Idle;

      _currentHealth = _maxHealth;

      _navMeshAgent = GetComponent<NavMeshAgent>();
      _player = GameObject.FindWithTag("Player");
   }

   private void Start()
   {
      _currentHealth = _maxHealth;
      _isAlive = true;
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
               _currentState = State.Attacking;
            }

            break;

         case State.Attacking:
            Shoot();
            SetAnimation("IsMelee");
            break;

         case State.Running:
            SetAnimation("IsFleeing");
            Run();
            break;

         case State.Hooked:
            InHarpoon();
            SetAnimation("IsIdle");
            break;

         case State.Dead:
            SetAnimation("IsDead");
            break;
      }
      
      if (_isFarEnough && _currentState != State.Hooked && unhookedSince>2f)
      {
         _currentState = State.Attacking;
      }
      if (!_isFarEnough && _currentState != State.Hooked && unhookedSince>2f)
      {
         _currentState = State.Running;
      }
   }

   private void Run()
      {
         _navMeshAgent.speed = 4.5f;
         Vector3 dirToPlayer = transform.position - _player.transform.position;
         Vector3 newPos = transform.position + dirToPlayer;
         _navMeshAgent.SetDestination(newPos);
      }

   private void Shoot()
   {
      if (unhookedSince > 2f)
      {
         if (Time.time > _nextFire)
         {
            _navMeshAgent.speed = 0f;
            _nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(projectile, bulletPoint.position, bulletPoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.velocity = (_player.transform.position - bullet.transform.position).normalized * constant;
         }
   }
}

      public void Harpooned() // LE MOMENT OÙ IL EST HARPONNÉ
      {
         if (!_isAlive) return;
         Debug.Log("Crochet crochet j't'ai accroché");
         _navMeshAgent.isStopped = true;
         unhookedSince = 0;
         _currentState = State.Hooked;
      }
      
      void InHarpoon() // CHAQUE UPDATE QUAND IL EST DANS LE HARPON
      {
         transform.position = PlayerHandler.Pointe.transform.position;
      }

      public void Released() // LE MOMENT OÙ IL EST RELÂCHÉ
      {
         if (!_isAlive) return;
         transform.position = PlayerHandler.releasedEnemy.position;
         _navMeshAgent.isStopped = false;
         _currentState = State.Idle;
      }


      public void TakeDamage(int amount)
      {
         _currentHealth = _currentHealth - amount;
         Debug.Log("Il me reste " + _currentHealth);
      }
      
      public void Die()
      {
         if (_isAlive == false) return;
         _isAlive = false;
         Debug.Log("J'AI MAAAAAAAAAAAAAAAAL");
         Destroy(_navMeshAgent);
         foreach (BoxCollider box in GetComponentsInChildren<BoxCollider>())
         {
            box.isTrigger = true;
         }
         _currentState = State.Dead;
         SetAnimation("IsDead");
      }

      void SetAnimation(string animationSelected)
      {
         /*_animator.SetBool("IsIdle", false);
         _animator.SetBool("IsChasing", false);
         _animator.SetBool("IsMelee", false);
         _animator.SetBool("IsFleeing", false);
         _animator.SetBool("IsDead", false);
           
         _animator.SetBool(animationSelected, true);*/
      }

}
