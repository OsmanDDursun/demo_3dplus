using UnityEngine;

namespace App.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _rotationSpeed = 10f;
        
        private bool _rightMouseButtonDown;
        
        private void Update()
        {
            _rightMouseButtonDown = Input.GetMouseButton(1);
            
            if (!_rightMouseButtonDown) return;
            
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");

            var rotation = _target.rotation;
            
            var xRotationChange = Quaternion.AngleAxis(x * _rotationSpeed, Vector3.up);
            var yRotationChange = Quaternion.AngleAxis(-y * _rotationSpeed, transform.right);
            
            _target.rotation = rotation * xRotationChange * yRotationChange;
            _target.localEulerAngles = new Vector3(_target.localEulerAngles.x, _target.localEulerAngles.y, 0);

            var positionChange = Vector3.zero;
            var speed = _speed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed *= 2;
            }
            if (Input.GetKey(KeyCode.W))
            {
                positionChange += _target.forward * Time.deltaTime * speed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                positionChange -= _target.forward * Time.deltaTime * speed;
            } 
            if (Input.GetKey(KeyCode.A))
            {
                positionChange -= _target.right * Time.deltaTime * speed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                positionChange += _target.right * Time.deltaTime * speed;
            }
            if(Input.GetKey(KeyCode.Q))
            {
                positionChange -= Vector3.up * Time.deltaTime * speed;
            }
            if(Input.GetKey(KeyCode.E))
            {
                positionChange += Vector3.up * Time.deltaTime * speed;
            }
            
            _target.position += positionChange;
        }
    }
}