using System.Security.Cryptography;
using UnityEngine;

    public class PlayerControls : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private CharacterController _characterController;
        
        private Rigidbody rb;
        private Vector3 _harpoonPosition;
        
        //Harpoon prefab
        public GameObject harpoonOBJ;

        public Transform cam;

        private State _state;

        private float _currentGrav = 0;
        private enum State
        {
            Normal, 
            HarpoonMovingPlayer,
        }
    
        // Start is called before the first frame update
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            rb = GetComponent<Rigidbody>();
            HandleHarpoonShotStart();
            _state = State.Normal;
            
        }

        // Update is called once per frame
        private void Update()
        {
            switch (_state)
            {
                default:
                    case State.Normal:
                    Move();
                    HandleHarpoonShotStart();
                    break;
                case State.HarpoonMovingPlayer:
                    HarpoonMovement();
                    break;
            }
        } 
        

        private void Move()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (_characterController.isGrounded) _currentGrav = -0.01f;
            else _currentGrav -= 10f * Time.deltaTime;
            Vector3 movement = new Vector3(horizontal, 0, vertical) * (_speed * Time.deltaTime);
            movement.y = _currentGrav;
            //Vector3 newPosition = rb.position + rb.transform.TransformDirection(movement);
            //rb.MovePosition(newPosition);
            movement = transform.TransformDirection(movement);
            _characterController.Move(movement);
        }
        
        
        private void HandleHarpoonShotStart()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit raycastHit))
                {
                    _harpoonPosition = raycastHit.point;
                    _state = State.HarpoonMovingPlayer;
                }
            }
        }

        private void HarpoonMovement()
        {
            Vector3 _harpoonShotDir = (_harpoonPosition - transform.position).normalized;
            Move();
        }
    }
