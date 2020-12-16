using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator _anim;
    public BoxCollider DoorCollider;
    public GameObject Enemies;
    public int enemiesAlive;


    private void Awake()
    {
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
        if (enemiesAlive < 1 && DoorCollider.enabled == false) OpenDoor();
    }


    private void OpenDoor()
    {
        DoorCollider.enabled = false;
        _anim.SetBool("IsOpen", true);
    }
}