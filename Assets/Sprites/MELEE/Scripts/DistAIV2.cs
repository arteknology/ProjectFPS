using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class DistAIV2 : MonoBehaviour, IDamageable
{
   private NavMeshAgent _navMesh;
   private GameObject _player;

   public GameObject pointe;

   public GameObject projectile;
   public Transform bulletPoint;
   public float constant;
   public float fireRate;
   private float _nextFire;
   public float distanceToAttack;

   public float distanceFromPlayer;
   
   public float currentHealth;
   public float maxHealth = 80;

   public bool isGrabbed;
   public bool hasAttacked;
   
   private State _state;

   private enum State
   {
      Normal,
      Attacking,
      Running,
      Hooked,
      Dying
   }

   private void Awake()
   {
      _state = State.Normal;

      currentHealth = maxHealth;

      hasAttacked = false;
      isGrabbed = false;
      
      _navMesh = GetComponent<NavMeshAgent>();
      _player = GameObject.FindWithTag("Player");
   }

   private void Update()
   {
      switch (_state)
      {
         case State.Normal:
            
            break;
         case State.Attacking:
            Shoot();
            break;
         case State.Running:
            Run();
            break;
         case State.Hooked:
            HarpoonDragged(pointe.transform);
            break;
         case State.Dying:
            Destroy(gameObject, 1f);
            break;
      }

      distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);
      
      if (distanceFromPlayer >= distanceToAttack)
      {
         _state = State.Attacking;
      }
      else
      {
         _state = State.Running;
      }

      if (isGrabbed) _state = State.Hooked;

      if (currentHealth <= 0) _state = State.Dying;
   }

   private void Run()
   {
      _navMesh.speed = 4.5f;
      Vector3 dirToPlayer = transform.position - _player.transform.position;
      Vector3 newPos = transform.position + dirToPlayer;
      _navMesh.SetDestination(newPos);
   }

   private void Shoot()
   {
      if (Time.time > _nextFire)
      {
         _navMesh.speed = 0f;
         _nextFire = Time.time + fireRate;
         GameObject bullet = Instantiate(projectile, bulletPoint.position, bulletPoint.rotation);
         Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
         bulletRb.velocity = (_player.transform.position - bullet.transform.position).normalized * constant;
      }
   }

   void HarpoonDragged(Transform col)
   {
      _navMesh.speed = 0;
      StartCoroutine(WaitForStunToEnd(2f));
      transform.position = col.transform.position;
   }

   IEnumerator WaitForStunToEnd(float time)
   {
      yield return new WaitForSeconds(time);
      isGrabbed = false;
      _state = State.Running;
   }

   public void TakeDamage(int amount)
   {
      currentHealth = currentHealth - amount;
      Debug.Log("Il me reste " + currentHealth);
   }
}
