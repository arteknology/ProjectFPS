using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChiottesScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<PlayerHandler>()._state = PlayerHandler.State.Dead;
        SceneManager.LoadScene(Credits);
    }
}
