using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScripts : MonoBehaviour
{

    public AudioSource AudioSource;
    public AudioClip fin;

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }
    void Awake()
    {
        AudioSource.clip = fin;
        StartCoroutine(Pompelope());
    }

    IEnumerator Pompelope()
    {
        yield return new WaitForSeconds(65f);
        ReturnToMenu();
    }
    
    void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
}
