using System.Collections;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator _anim;
    public BoxCollider DoorCollider;
    public GameObject Enemies;
    public int enemiesAlive;
    private PlayerHandler _player;

    private AudioSource _audio;
    public AudioClip ArenaWin, Intro, Loop;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerHandler>();
        Enemies.SetActive(false);
        enemiesAlive = 0;
        _audio = GetComponent<AudioSource>();
        _audio.clip = Intro;
        _audio.loop = false;
        _audio.Play();
        StartCoroutine( AttendreFinMorceau() );
        foreach (Transform child in Enemies.transform)
            if (child.gameObject.CompareTag("Enemy"))
                enemiesAlive += 1;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (Enemies.activeSelf == false && other.gameObject.CompareTag("Player")) Enemies.SetActive(true);
    }

    public void RemoveEnemy()
    {
        enemiesAlive -= 1;
        if (enemiesAlive < 1 && DoorCollider.enabled != false) OpenDoor();
    }


    private void OpenDoor()
    {
        _audio.Stop();
        _audio.PlayOneShot(ArenaWin);
        _player.currentHealth = _player.maxHealth;
        DoorCollider.enabled = false;
        _anim.SetBool("IsOpen", true);
    }
    
    IEnumerator AttendreFinMorceau()
    {
        yield return 1;
        while (_audio.isPlaying) yield return null;
        _audio.clip = Loop;
        _audio.loop = true;
        _audio.Play();
    }
}