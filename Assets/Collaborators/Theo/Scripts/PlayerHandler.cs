﻿using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerHandler : MonoBehaviour, IDamageable
{
    [SerializeField] private float mouseSensivity = 1f;
    
    [SerializeField] private Transform harpoonTransform;

    [SerializeField] public float maximumRange;

    public static Transform releasedEnemy;

    public float maxHealth = 100f;
    public float currentHealth;
    public HealthBarScript HealthBar;

    private bool godMod;
    
    
    //Harpoon stuff
    private Vector3 _harpoonPosition;
    private float _harpoonSize;
    public static Transform Pointe;
    private Vector3 _oldPointePos;
    private IHarpoonable enemy;
    Transform enemyTransform;
    private bool hasHarpooned = false;

    public GameObject pointe;
    public GameObject pointeImage;

    private Vector3 _pointeStartPos;
    
    //Player stuff
    private CharacterController _characterController;
    private float _cameraVerticalAngle;
    private float _characterVelocityY;
    private Camera _playerCamera;

    // Feedback stuff
    public AudioSource damageSound;
    public AudioClip deathSound;
    public AudioClip HarponMur;
    public AudioClip HarponEnnemi;
    public AudioClip TirHarpon;
    public ParticleSystem hookParticles;
    
    
    //Chainsaw stuff
    public bool isDetectingEnemy;

    
    //State machine stuff
    public State _state;
    public enum  State 
    {
        Normal,
        HarpoonThrown,
        HarpoonMovingPlayer,
        HarpoonRetract,
        Chainsaw,
        Dead
    }
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        _state = State.Normal;
        //harpoonTransform.gameObject.SetActive(false);
        Pointe = GameObject.FindGameObjectWithTag("Harpoon").transform;
        currentHealth = maxHealth;
        transform.Find("GRAPHICS").gameObject.SetActive(false);
        releasedEnemy = transform.Find("ReleasedEnemy");
        HealthBar.SetMaxHealth(maxHealth);
        _pointeStartPos = Pointe.transform.localPosition;
        pointe.SetActive(false);
        pointeImage.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) godMod = !godMod;
        
        
        switch (_state)
        {
            //default:
            case State.Normal:
                HandleCharacterLook();
                HandleCharacterMovement();
                    if (TestInputLeftClick() && !isDetectingEnemy && PauseMenu.GameIsPaused == false) HandleHarpoonShotStart();
                break;
            case State.HarpoonThrown:
                //Debug.Log("Harpoon Thrown");
                HandleHarpoonShotThrow();
                HandleCharacterLook();
                HandleCharacterMovement();
                HandleHarpoonCancel();
                //Debug.Log(_harpoonSize);
                break;
            case State.HarpoonMovingPlayer:
                //Debug.Log("Harpoon Moving Player");
                HandleHarpoonMovement();
                //HandleCharacterLook();
                break;
            case State.HarpoonRetract:
                //Debug.Log("Harpoon Retract");
                HandleCharacterMovement();
                HandleCharacterLook();
                HandleHarpoonBack();
                break;
            case State.Dead:
                break;
        }
        
        HealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0 && _state!= State.Dead)
        {
            Die();
        }
    }

    void HandleCharacterLook()
    {
        if (PauseMenu.GameIsPaused == false)
        {
            float lookX = Input.GetAxisRaw("Mouse X");

            if (lookX != 0)

                transform.Rotate(new Vector3(0f, lookX * mouseSensivity, 0f), Space.Self);

            _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, 0f, 0f);

            _playerCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
        }
    }

    void HandleCharacterMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        float moveSpeed = 10f;

        Vector3 characterVelocity = transform.right * (moveX * moveSpeed) + transform.forward * (moveZ * moveSpeed);

        float gravityDownForce = -60f;
        _characterVelocityY += gravityDownForce * Time.deltaTime;

        characterVelocity.y = _characterVelocityY;

        _characterController.Move(characterVelocity * Time.deltaTime);
    }

    private void ResetGravityEffect()
    {
        _characterVelocityY = 0;
    }

    void HandleHarpoonShotStart() // PAN ! LE HARPON PART
    {
        pointe.SetActive(true);
        pointeImage.SetActive(false);
        _harpoonSize = 0f;
        harpoonTransform.gameObject.SetActive(true);
        _oldPointePos = Pointe.position;
        _state = State.HarpoonThrown;
        if (TirHarpon!=null) damageSound.clip = TirHarpon;
        damageSound.Play();
        
        
    }

        void HandleHarpoonCancel()
        {
            if (Input.GetButtonUp("Fire1"))
            {
                _state = State.HarpoonRetract;
            }
        }

        
        private void HandleHarpoonShotThrow()
        {
            MoveHarpoon(80f);
        }
    
    
    void HandleHarpoonMovement() // LE JOUEUR EST ATTIRÉ VERS LE HARPON
    {
        //harpoonTransform.LookAt(_harpoonPosition);
        
        /*Vector3 harpoonDir = (_harpoonPosition - transform.position).normalized;

        float harpoonSpeedMin = 10f;
        float harpoonSpeedMax = 40f;
        float harpoonSpeed = Mathf.Clamp(Vector3.Distance(transform.position, _harpoonPosition), min:harpoonSpeedMin, max: harpoonSpeedMax);
        float harpoonshotSpeedMultiplier = 3f;
        _characterController.Move(harpoonDir * (harpoonSpeed * harpoonshotSpeedMultiplier * Time.deltaTime));

        float reachedHarpoonPositionDistance = 2f;*/

        Vector3 Movement = (Pointe.position - transform.position).normalized * (45f * Time.deltaTime);
        _characterController.Move(Movement);
        _harpoonSize -= Movement.magnitude;
        Pointe.localPosition = Vector3.forward * _harpoonSize;

        if (_harpoonSize <= 2f)
        {
            StopHarpoon();
        }
    }
    
    void HandleHarpoonBack() // LE HARPON REVIENT SANS RIEN
    {
        MoveHarpoon(-80f);

        if (_harpoonSize <= 2f)
        {
            StopHarpoon();
        }
    }

    void StopHarpoon()
    {
        if (enemy!= null)
        {
            enemy.Released();
            enemy = null;
        }
        Pointe.localPosition = _pointeStartPos;
        _state = State.Normal;
        ResetGravityEffect();
        hasHarpooned = false;
        pointe.SetActive(false);
        pointeImage.SetActive(true);
    }


    void MoveHarpoon(float speed)
    {
        _harpoonSize += speed * Time.deltaTime;
        Pointe.localPosition = Vector3.forward * _harpoonSize;

        if (hasHarpooned == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(_oldPointePos, Pointe.forward, out hit, (Pointe.position - _oldPointePos).magnitude))
            {
                hasHarpooned = true;
                _harpoonSize = (hit.point - harpoonTransform.position).magnitude;
                IHarpoonable tructouche = hit.transform.GetComponentInParent<IHarpoonable>();
                
                hookParticles.Play();
                
                if (tructouche!=null) // SI LE HARPON A TOUCHÉ UN ENNEMI
                {
                    enemy = tructouche;
                    enemy.Harpooned();
                    enemyTransform = hit.transform;
                    _state = State.HarpoonRetract;
                    if (HarponEnnemi!=null) damageSound.clip = HarponEnnemi;
                    damageSound.Play();
                }
                else if (speed > 0 && hit.transform.CompareTag("Wall")) // SI LE HARPON A TOUCHÉ UN MUR HARPONNABLE
                {
                    _state = State.HarpoonMovingPlayer;
                    if (HarponMur!=null) damageSound.clip = HarponMur;
                    damageSound.Play();
                    
                }
                else // SI LE HARPON A TOUCHÉ N'IMPORTE QUOI D'AUTRE
                {
                    _state = State.HarpoonRetract;
                }

            }
            else // SI LE HARPON N'A RIEN TOUCHÉ
            {
                
            }

            if (_harpoonSize >= maximumRange) // SI LE HARPON A ATTEINT SA LONGUEUR MAX
            {
                _state = State.HarpoonRetract;
            }
        }

        _oldPointePos = Pointe.position;
    }
    
    bool TestInputLeftClick()
    {
        return (Input.GetButtonDown("Fire1"));
    }


    public void TakeDamage(int amount)
    {
        if (_state==State.Dead || godMod) return;
        damageSound.Play();
        currentHealth -= amount;
        ScreenShake.Shake(10f);
    }

    public void Die()
    {
        if (_state==State.Dead) return;
        transform.Translate(Vector3.up* -1.25f);
        transform.Rotate(new Vector3(0,0,45f));
        if (deathSound!=null) damageSound.clip = deathSound;
        damageSound.Play();
        currentHealth = 0;
        _state= State.Dead;
        Cursor.lockState = CursorLockMode.None;
        Invoke("SceneReload", 2f);
    }

    void SceneReload()
    {
        SceneManager.LoadScene("Menu");
    }
    
}
