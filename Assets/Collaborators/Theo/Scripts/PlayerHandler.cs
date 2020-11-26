﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class PlayerHandler : MonoBehaviour, IDamageable
{
    [SerializeField] private float mouseSensivity = 1f;
    
    [SerializeField] private Transform harpoonTransform;

    [SerializeField] public float maximumRange;
    
    public float maxHealth = 100f;
    public float currentHealth;
    public TextMeshProUGUI healthDisplay;
    
    [SerializeField] public GameObject harpoonedEnemy;

    //private float _clampedXRotation = 30f;
    
    //Harpoon stuff
    private Vector3 _harpoonPosition;
    private float _harpoonSize;
    public Transform Pointe;
    private Vector3 _oldPointePos;
    private IHarpoonable enemy;
    private bool hasHarpooned = false;
    
    //Player stuff
    private CharacterController _characterController;
    private float _cameraVerticalAngle;
    private float _characterVelocityY;
    private Camera _playerCamera;
    
    //Chainsaw stuff
    public bool isDetectingEnemy;

    
    //State machine stuff
    private State _state;
    private enum  State 
    {
        Normal,
        HarpoonThrown,
        HarpoonMovingPlayer,
        HarpoonRetract,
        Chainsaw
    }
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerCamera = transform.Find("Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        _state = State.Normal;
        //harpoonTransform.gameObject.SetActive(false);

        currentHealth = maxHealth;
    }

    private void Update()
    {
        switch (_state)
        {
            //default:
            case State.Normal:
                HandleCharacterLook();
                HandleCharacterMovement();
                    if (TestInputLeftClick() && !isDetectingEnemy) HandleHarpoonShotStart();
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
        }
        
        healthDisplay.text = currentHealth + "/100";
    }

    void HandleCharacterLook()
    {
        float lookX = Input.GetAxisRaw("Mouse X");
        
        if (lookX != 0)
            
            transform.Rotate(new Vector3(0f, lookX * mouseSensivity, 0f), Space.Self);

        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, 0f, 0f);

        _playerCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
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
        _harpoonSize = 0f;
        harpoonTransform.gameObject.SetActive(true);
        _oldPointePos = Pointe.position;
        _state = State.HarpoonThrown;
        
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
        if (harpoonedEnemy == null && _harpoonSize <= 1f)
        {
            StopHarpoon();
        }

        if (harpoonedEnemy && _harpoonSize <= 2f)
        {
            StopHarpoon();
        }
    }

    void StopHarpoon()
    {
        if (enemy!= null) enemy.Released();
        enemy = null;
        Pointe.localPosition = Vector3.forward;
        _state = State.Normal;
        ResetGravityEffect();
        //harpoonTransform.gameObject.SetActive(false);
        hasHarpooned = false;

        if (harpoonedEnemy)
        {
            harpoonedEnemy.GetComponent<EnemyTestScript>().hasCollided = false;
        }

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
                Debug.Log("Le harpon a touché "+hit.transform.name);
                _harpoonSize = (hit.point - harpoonTransform.position).magnitude;
                /*IHarpoonable tructouche = hit.transform.GetComponentInParent<IHarpoonable>();
                

                if (tructouche!=null) // SI LE HARPON A TOUCHÉ UN ENNEMI
                {
                    //enemy = tructouche;
                    //enemy.Harpooned();
                    
                    harpoonedEnemy = hit.transform.gameObject;
                    Debug.Log(harpoonedEnemy.name);
                    harpoonedEnemy.GetComponent<EnemyTestScript>().Harpoon(Pointe);
                    
                    _state = State.HarpoonRetract;
                }*/
                
                if (hit.collider.CompareTag("Enemy")) // SI LE HARPON A TOUCHÉ UN ENNEMI
                {

                    harpoonedEnemy = hit.transform.gameObject;
                    Debug.Log(harpoonedEnemy.name);
                    harpoonedEnemy.GetComponent<EnemyTestScript>().hasCollided = true;
                    
                    _state = State.HarpoonRetract;
                }
                else if (speed > 0 && hit.transform.CompareTag("Wall")) // SI LE HARPON A TOUCHÉ UN MUR HARPONNABLE
                {
                    _state = State.HarpoonMovingPlayer;
                    
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
        currentHealth -= amount;
    }
}
