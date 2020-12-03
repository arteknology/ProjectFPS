using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MixAI : MonoBehaviour
{
    private NavMeshAgent navMesh;
    private GameObject _player;
    //private Animator _anim;
    
    public float DistanceToAttack;
    public float AttackDist;
    public float ToAway;
    
    public GameObject Projectile;
    public Transform BulletPoint;
    public float Constant;
    public float fireRate = 0.5f;
    private float nextFire = 0.0f;
    
    public int Life;
    public int PlayerDamages;
    public int Damages;
    
    public bool Grabbed;
    public DoorScript Door;
    public bool Dead = false;

    void Start()
    {
        Grabbed = false;
        navMesh = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player");
        //_anim = GetComponent<Animator>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        
        if (Life > 0)
        {
            if (distance <= ToAway)
            {
                Run();
            }
            else
            {
                DistAttack();
            }
        }
        else
        {
            Die();
        }

        if (Dead)
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
            if (distance > DistanceToAttack )
            {
                navMesh.isStopped = false;
                Vector3 dirToPlayer = transform.position - _player.transform.position;
                Vector3 newPos = transform.position - dirToPlayer;
                navMesh.SetDestination(newPos);
                //_anim.SetBool("Run", true);
            }
            else
            {
                //_anim.SetBool("Run", false);
                navMesh.isStopped = true;
                CacAttack();
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
        //_anim.SetBool("Run", false);
        //_anim.SetBool("RunBack", false);
        StartCoroutine(WaitForStunToEnd());
    }
    
    //Attack & Die
    private void Die()
    {
        Door.RemoveEnemy(this.gameObject);
        navMesh.isStopped = true; 
        //_anim.SetTrigger("Die");
        Destroy(this.gameObject, 1f);
        
    }
    
    private void CacAttack()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= DistanceToAttack)
        {
            //_anim.SetBool("attack", true);
        }
        else
        {
            //_anim.SetBool("attack", false);
        }
    }

    private void DistAttack()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        
        //_anim.SetBool("Run", false);
        navMesh.isStopped = true;
        
        if (!Grabbed)
        {
            if (distance > ToAway)
            {
                //_anim.SetBool("Attack", true);
                Shoot();
            }
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
    
    
    public void DamagePlayer()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= AttackDist )
        {
            //_playerHandler.TakeDamage(Damages);
            //_anim.SetBool("attack", false);
        }
        else
        {
            //_anim.SetBool("attack", false);
        }
    }
//Triggers
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Harpoon"))
        {
            Grabbed = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Harpoon"))
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
