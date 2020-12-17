using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator _anim;
    public BoxCollider DoorCollider;
    public GameObject Enemies;
    public int enemiesAlive;
    private PlayerHandler _player;


    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerHandler>();
        Enemies.SetActive(false);
        enemiesAlive = 0;
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
        _player.currentHealth = _player.maxHealth;
        DoorCollider.enabled = false;
        _anim.SetBool("IsOpen", true);
    }
}