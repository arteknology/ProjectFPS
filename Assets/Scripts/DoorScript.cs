using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject Enemies;
    public List<GameObject> EnemyList = new List<GameObject>();
    private Transform _transform;
    public Animator _anim;
    public BoxCollider DoorCollider;
    // Start is called before the first frame update
    void Start()
    {
        _transform = Enemies.transform;
        Enemies.SetActive(false);
        foreach (Transform child in _transform)
        {
            if (child.gameObject.CompareTag("Enemy"))
            {
                EnemyList.Add(child.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("J'ai touché le joueur");
            Enemies.SetActive(true);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        EnemyList.Remove(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyList.Count == 0)
        {
            DoorCollider.enabled = false;
            _anim.SetBool("IsOpen", true);
        }
    }
}
