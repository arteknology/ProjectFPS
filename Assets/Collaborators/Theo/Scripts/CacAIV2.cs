using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CacAIV2 : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _player;
    private float _enemySpeed = 6f;
    private float _enemySpeedBack = -6f;
    private float _enemyStoppedSpeed = 0f;

    public GameObject pointe;

    private int _maxHealth = 80;
    private int _currentHealth;
    
    private bool _playerIsInMeleeRange => Vector3.Distance(transform.position, _player.transform.position)

}
