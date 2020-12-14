using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject Enemies;
    public int enemiesAlive;
    public Animator _anim;
    public BoxCollider DoorCollider;



    void Awake()
    {
        Enemies.SetActive(false);
        enemiesAlive = 0;
        foreach (Transform child in Enemies.transform)
        {
            if (child.gameObject.CompareTag("Enemy")) enemiesAlive+=1;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (Enemies.activeSelf==false && other.gameObject.CompareTag("Player"))
        {
            Enemies.SetActive(true);
        }
    }

    public void RemoveEnemy()
    {
        enemiesAlive -= 1;
        if (enemiesAlive<1 && DoorCollider.enabled == false) OpenDoor();
    }


    void OpenDoor()
    {
        DoorCollider.enabled = false;
        _anim.SetBool("IsOpen", true);
    }

}
