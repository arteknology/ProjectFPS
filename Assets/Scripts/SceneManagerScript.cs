using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public GameObject optionMenuUI;
    public GameObject mainMenuUI;
    public AudioMixer audioMixer;

    public AudioSource audio;
    public AudioClip intro;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }

    private void Awake()
    {
        audio.clip = intro;
        audio.loop = true;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GAME");
        audio.Stop();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void DisplayOptions()
    {
        mainMenuUI.SetActive(false);
        optionMenuUI.SetActive(true);
    }
    
    public void ReturnToMenu()
    {
        mainMenuUI.SetActive(true);
        optionMenuUI.SetActive(false);
    }
    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }
}