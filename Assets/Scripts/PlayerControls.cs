using UnityEngine;

    public class PlayerControls : MonoBehaviour
    {
        [SerializeField] private float _speed;


        private Rigidbody rb;
    
        // Start is called before the first frame update
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        private void Update()
        {
            
        } 

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, 0, vertical) * (_speed * Time.deltaTime);

            Vector3 newPosition = rb.position + rb.transform.TransformDirection(movement);
            rb.MovePosition(newPosition);
        }

    }
