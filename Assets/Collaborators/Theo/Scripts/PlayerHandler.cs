using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private float mouseSensivity = 1f;
    
    [SerializeField] private Transform harpoonTransform;

    [SerializeField] public float maximumRange;

    private float _clampedXRotation = 30f;
    
    //Harpoon stuff
    private Vector3 _harpoonPosition;
    private float _harpoonSize;
    public Transform Pointe;
    private Vector3 _oldPointePos;
    
    //Player stuff
    private CharacterController _characterController;
    private float _cameraVerticalAngle;
    private float _characterVelocityY;
    private Camera _playerCamera;
    

    //State machine stuff
    private State _state;
    private enum  State 
    {
        Normal,
        HarpoonThrown,
        HarpoonMovingPlayer,
        HarpoonRetract
    }
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerCamera = transform.Find("Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        _state = State.Normal;
        //harpoonTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (_state)
        {
            default:
            case State.Normal: 
                HandleCharacterLook();
                HandleCharacterMovement();
                HandleHarpoonShotStart();
                
                break;
            case State.HarpoonThrown:
                Debug.Log("Harpoon Thrown");
                HandleHarpoonShotThrow();
                HandleCharacterLook();
                HandleCharacterMovement();
                HandleHarpoonStop();
                Debug.Log(_harpoonSize);
                break;
            case State.HarpoonMovingPlayer:
                Debug.Log("Harpoon Moving Player");
                HandleHarpoonMovement();
                HandleCharacterLook();
                break;
            case State.HarpoonRetract:
                Debug.Log("Harpoon Retract");
                HandleCharacterMovement();
                HandleCharacterLook();
                HandleHarpoonBack();
                break;
        }
    }

    void HandleCharacterLook()
    {
        float lookX = Input.GetAxisRaw("Mouse X");

        transform.Rotate(new Vector3(0f, lookX * mouseSensivity, 0f), Space.Self);

        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, 0f, 0f);

        _playerCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
    }

    void HandleCharacterMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        float moveSpeed = 20f;

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

    void HandleHarpoonShotStart()
    {
        if (TestInputDownHarpoonShot())
        {
            //if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit raycastHit))
            //{
                //_harpoonPosition = raycastHit.point;
                _harpoonSize = 0f;
                harpoonTransform.gameObject.SetActive(true);
                //harpoonTransform.localScale = Vector3.zero;
                _state = State.HarpoonThrown;
            //}
        }
    }

        void HandleHarpoonStop()
        {
            if (Input.GetButtonUp("Fire1"))
            {
                _state = State.HarpoonRetract;
            }
        }

        
        private void HandleHarpoonShotThrow()
        {
            //harpoonTransform.LookAt(_harpoonPosition);

            float harpoonSpeed = 70f;
            _harpoonSize += harpoonSpeed * Time.deltaTime;
            //harpoonTransform.localScale = new Vector3(1, 1, _harpoonSize);
            Pointe.localPosition = Vector3.forward * _harpoonSize;

            /*if (_harpoonSize >= Vector3.Distance(transform.position, _harpoonPosition))
            {
                _state = State.HarpoonMovingPlayer; 
            }*/
            RaycastHit hit;
            if (Physics.Raycast(_oldPointePos, Pointe.forward, out hit, (Pointe.position - _oldPointePos).magnitude))
            {
                if (hit.transform.CompareTag("Wall"))
                {
                    _state = State.HarpoonMovingPlayer;
                }
            }

            else
            {
                
            }

            if (_harpoonSize >= maximumRange)
            {
                _state = State.HarpoonRetract;
            }
    }
    void HandleHarpoonMovement()
    {
        //harpoonTransform.LookAt(_harpoonPosition);
        
        Vector3 harpoonDir = (_harpoonPosition - transform.position).normalized;

        float harpoonSpeedMin = 10f;
        float harpoonSpeedMax = 40f;
        float harpoonSpeed = Mathf.Clamp(Vector3.Distance(transform.position, _harpoonPosition), min:harpoonSpeedMin, max: harpoonSpeedMax);
        float harpoonshotSpeedMultiplier = 3f;
        _characterController.Move(harpoonDir * (harpoonSpeed * harpoonshotSpeedMultiplier * Time.deltaTime));

        float reachedHarpoonPositionDistance = 2f;
        if (Vector3.Distance(transform.position, _harpoonPosition) < reachedHarpoonPositionDistance)
        {
            StopHarpoon();
        }
    }

    /*void HarpoonLook()
    {
        float lookX = Input.GetAxisRaw("Mouse X");

        transform.Rotate(new Vector3(0f, lookX * mouseSensivity, 0f), Space.Self);

        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, 0f, 0f);
        _clampedXRotation = Mathf.Clamp(_clampedXRotation, -30f, 30f);

        _playerCamera.transform.localEulerAngles = new Vector3(_clampedXRotation, _cameraVerticalAngle, 0);
        
    }*/

    void HandleHarpoonBack()
    {
        float harpoonSpeed = 80f;
        _harpoonSize -= harpoonSpeed * Time.deltaTime;
        //harpoonTransform.localScale = new Vector3(1, 1, _harpoonSize);
        Pointe.localPosition = Vector3.forward * _harpoonSize;
        if (_harpoonSize <= 1)
        {
            _state = State.Normal;
        }
    }

    void StopHarpoon()
    {
        _state = State.Normal;
        ResetGravityEffect();
        harpoonTransform.gameObject.SetActive(false);
    }

    bool TestInputDownHarpoonShot()
    {
        return (Input.GetButtonDown("Fire1"));
    }
    
}
