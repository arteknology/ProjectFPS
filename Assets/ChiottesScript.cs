using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChiottesScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GoToCredits();
    }

    void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
