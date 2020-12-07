using UnityEngine;

namespace Monobehaviours
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float look_sensivity;
        [SerializeField] private float _smoothing;

        private Transform _player;
        private Vector2 smoothVelocity;
        private Vector2 currentLooking;
    
    
        // Start is called before the first frame update
        private void Start()
        {
            _player = transform.parent;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        private void Update()
        {
            RotateCamera();
        }

        private void RotateCamera()
        {
            Vector2 inputValues = new Vector2(Input.GetAxisRaw("Mouse X"),0f);
            inputValues = Vector2.Scale(inputValues, new Vector2(look_sensivity * _smoothing, look_sensivity * _smoothing));
        
            smoothVelocity.x = Mathf.Lerp(smoothVelocity.x, inputValues.x, 1f / _smoothing);
            smoothVelocity.y = Mathf.Lerp(smoothVelocity.y, inputValues.y, 1f / _smoothing);

            currentLooking += smoothVelocity;
        
            transform.localRotation = Quaternion.AngleAxis(-currentLooking.y, Vector3.right);
            _player.localRotation = Quaternion.AngleAxis(currentLooking.x, _player.transform.up);
        }
    }
}
