using System;
using Cinemachine;
using UnityEngine;

namespace App.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _scrollSpeed = 10f;

        private LayerMask _buildingLayerMask;
        private LayerMask _terrainLayerMask;
        private Camera _camera;
        private Vector3 _inputStartPoint;
        private Vector3 _rightInputStartPoint;
        private Vector3 _nextTargetPosition;
        private Quaternion _nextTargetRotation;
        private Plane _intersectPlane;

        private bool _leftMouseButtonDown;
        private bool _rightMouseButtonDown;

        private void Awake()
        {
            _intersectPlane = new Plane(Vector3.up, Vector3.zero);
            _nextTargetPosition = _target.position;
            _nextTargetRotation = _target.rotation;
            _camera = Camera.main;
            _buildingLayerMask = LayerMask.GetMask("Building");
            _terrainLayerMask = LayerMask.GetMask("Terrain");
        }
        
        private bool IsPointerOverBuilding()
        {
            return Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out _, int.MaxValue, _buildingLayerMask);
        }

        private void Update()
        {
            HandleScroll();
            HandleMovement();
            HandleRotation();
        }
        
        private void HandleScroll()
        {
            var scroll = Input.mouseScrollDelta.y;
            Debug.Log(scroll);
            
            if (scroll == 0) return;
            
            //calculate with mouse position
            
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, int.MaxValue, _terrainLayerMask))
            {
                var inputTarget = hit.point;
                //inputTarget.y = _target.position.y;
                var motionDir = (inputTarget - _target.position).normalized;
                _nextTargetPosition += motionDir * (scroll * Time.deltaTime * _scrollSpeed * 10);
                return;
            }
            _nextTargetPosition += _target.forward * (scroll * Time.deltaTime * _scrollSpeed);
        }
        
        private void HandleRotation()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _rightInputStartPoint = Input.mousePosition;
                _rightMouseButtonDown = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                _rightMouseButtonDown = false;
            }
            
            if (!_rightMouseButtonDown) return;
            
            var currentPoint = Input.mousePosition;
            
            var x = (currentPoint.x - _rightInputStartPoint.x) * _rotationSpeed * Time.deltaTime;
            var y = (currentPoint.y - _rightInputStartPoint.y) * _rotationSpeed * Time.deltaTime;
            
            var rotation = _target.rotation;
            
            var xRotationChange = Quaternion.AngleAxis(x, Vector3.up);
            var yRotationChange = Quaternion.AngleAxis(-y, transform.right);
            //no change in z rotation
            
            _nextTargetRotation = _target.rotation;
            _nextTargetRotation = Quaternion.Euler(_nextTargetRotation.eulerAngles.x, _nextTargetRotation.eulerAngles.y, 0);
            _nextTargetRotation = _nextTargetRotation * xRotationChange * yRotationChange;
            
            _rightInputStartPoint = currentPoint;
        }

        private void HandleMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverBuilding()) return;
                _inputStartPoint = Input.mousePosition;
                _leftMouseButtonDown = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _leftMouseButtonDown = false;
            }
            
            if (!_leftMouseButtonDown) return;
            
            var currentPoint = Input.mousePosition;
            
            var x = (currentPoint.x - _inputStartPoint.x) * _speed * Time.deltaTime;
            var y = (currentPoint.y - _inputStartPoint.y) * _speed * Time.deltaTime;
            
            var forward = _target.forward;
            forward.y = 0;
            
            var right = _target.right;
            right.y = 0;
            
            var motion = forward * y + right * x;
            _nextTargetPosition -= motion;
            
            _inputStartPoint = currentPoint;
        }

        private void LateUpdate()
        {
            _target.position = Vector3.Lerp(_target.position, _nextTargetPosition, Time.deltaTime * 5f);
            _target.rotation = Quaternion.Lerp(_target.rotation, _nextTargetRotation, Time.deltaTime * 20f);
        }

        // private void Update()
        // {
        //     _rightMouseButtonDown = Input.GetMouseButton(1);
        //     
        //     if (!_rightMouseButtonDown) return;
        //     
        //     var x = Input.GetAxis("Mouse X");
        //     var y = Input.GetAxis("Mouse Y");
        //
        //     var rotation = _target.rotation;
        //     
        //     var xRotationChange = Quaternion.AngleAxis(x * _rotationSpeed, Vector3.up);
        //     var yRotationChange = Quaternion.AngleAxis(-y * _rotationSpeed, transform.right);
        //     
        //     _target.rotation = rotation * xRotationChange * yRotationChange;
        //     _target.localEulerAngles = new Vector3(_target.localEulerAngles.x, _target.localEulerAngles.y, 0);
        //
        //     var positionChange = Vector3.zero;
        //     var speed = _speed;
        //     if (Input.GetKey(KeyCode.LeftShift))
        //     {
        //         speed *= 2;
        //     }
        //     if (Input.GetKey(KeyCode.W))
        //     {
        //         positionChange += _target.forward * Time.deltaTime * speed;
        //     }
        //     if (Input.GetKey(KeyCode.S))
        //     {
        //         positionChange -= _target.forward * Time.deltaTime * speed;
        //     } 
        //     if (Input.GetKey(KeyCode.A))
        //     {
        //         positionChange -= _target.right * Time.deltaTime * speed;
        //     }
        //     if (Input.GetKey(KeyCode.D))
        //     {
        //         positionChange += _target.right * Time.deltaTime * speed;
        //     }
        //     if(Input.GetKey(KeyCode.Q))
        //     {
        //         positionChange -= Vector3.up * Time.deltaTime * speed;
        //     }
        //     if(Input.GetKey(KeyCode.E))
        //     {
        //         positionChange += Vector3.up * Time.deltaTime * speed;
        //     }
        //     
        //     _target.position += positionChange;
        // }
    }
}