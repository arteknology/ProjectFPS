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
    private IHarpoonable enemy;
    
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
                if (TestInputDownHarpoonShot()) HandleHarpoonShotStart();
                break;
            case State.HarpoonThrown:
                //Debug.Log("Harpoon Thrown");
                HandleHarpoonShotThrow();
                HandleCharacterLook();
                HandleCharacterMovement();
                HandleHarpoonStop();
                //Debug.Log(_harpoonSize);
                break;
            case State.HarpoonMovingPlayer:
                //Debug.Log("Harpoon Moving Player");
                HandleHarpoonMovement();
                HandleCharacterLook();
                break;
            case State.HarpoonRetract:
                //Debug.Log("Harpoon Retract");
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

    void HandleHarpoonShotStart() // PAN ! LE HARPON PART
    {
        _harpoonSize = 0f;
        harpoonTransform.gameObject.SetActive(true);
        _oldPointePos = Pointe.position;
        _state = State.HarpoonThrown;
        
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

            float harpoonSpeed = 70f;
            _harpoonSize += harpoonSpeed * Time.deltaTime;
            Pointe.localPosition = Vector3.forward * _harpoonSize;


            RaycastHit hit;
            if (Physics.Raycast(_oldPointePos, Pointe.forward, out hit, (Pointe.position - _oldPointePos).magnitude))
            {
                Debug.Log("Le harpon a touché "+hit.transform.name);
                _harpoonSize = (hit.point - harpoonTransform.position).magnitude;
                IHarpoonable tructouche = hit.transform.GetComponentInParent<IHarpoonable>();

                if (tructouche!=null) // SI LE HARPON A TOUCHÉ UN ENNEMI
                {
                    enemy = tructouche;
                    enemy.Harpooned();
                    _state = State.HarpoonRetract;
                }
                else if (hit.transform.CompareTag("Wall")) // SI LE HARPON A TOUCHÉ UN MUR HARPONNABLE
                {
                    //_state = State.HarpoonMovingPlayer;
                    _state = State.HarpoonRetract;
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

            _oldPointePos = Pointe.position;
    }
    
    
    void HandleHarpoonMovement() // LE JOUEUR EST ATTIRÉ VERS LE HARPON
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


    void HandleHarpoonBack() // LE HARPON REVIENT SANS RIEN
    {
        float harpoonSpeed = 80f;
        _harpoonSize -= harpoonSpeed * Time.deltaTime;
        Pointe.localPosition = Vector3.forward * _harpoonSize;
        if (_harpoonSize <= 1)
        {
            StopHarpoon();
        }
    }

    void StopHarpoon()
    {
        if (enemy!=null) enemy.Released();
        enemy = null;
        Pointe.localPosition = Vector3.forward;
        _state = State.Normal;
        ResetGravityEffect();
        //harpoonTransform.gameObject.SetActive(false);
    }

    bool TestInputDownHarpoonShot()
    {
        return (Input.GetButtonDown("Fire1"));
    }
    
}
