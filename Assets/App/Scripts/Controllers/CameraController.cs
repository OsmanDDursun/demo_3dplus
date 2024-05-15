using App.Scripts.CommonModels;
using App.Scripts.Data;
using App.Scripts.Helpers;
using UnityEngine;

namespace App.Scripts.Controllers
{
    public class CameraController : SingletonBehaviour<CameraController>
    {
        private static readonly Vector2 XMinMax = new Vector2(-755, 3380);
        private static readonly Vector2 YMinMax = new Vector2(1000, 2000);
        private static readonly Vector2 ZMinMax = new Vector2(0, 4050);
        
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
        private Quaternion _initialRotation;

        private bool _leftMouseButtonDown;
        private bool _rightMouseButtonDown;

        protected override void OnAwake()
        {
            _initialRotation = _target.rotation;
            _nextTargetPosition = _target.position;
            _nextTargetRotation = _target.rotation;
            _camera = Camera.main;
            _buildingLayerMask = LayerMask.GetMask("Building");
            _terrainLayerMask = LayerMask.GetMask("Terrain");
        }
        
        public void FocusTo(Vector3 position, float height)
        {
            position += Vector3.up * height / 2;
            var dir = (position - _target.position).normalized;
            var dir2D = new Vector3(dir.x, 0, dir.z);
            
            height = Mathf.Clamp(height, 50, 400);
            var cameraPosition = position + dir2D * (height * -3) + Vector3.up * (height * 2);
            
            var cameraDir = (position - cameraPosition).normalized;
            var cameraRotation = Quaternion.LookRotation(cameraDir);

            _nextTargetPosition = cameraPosition;
            _nextTargetRotation = cameraRotation;
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
            
            if (scroll == 0) return;
            
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
            
            var currentEuler = rotation.eulerAngles;
            
            var xRotationChange = Quaternion.AngleAxis(x, Vector3.up);
            var yRotationChange = Quaternion.AngleAxis(-y, transform.right);
            
            _nextTargetRotation = _target.rotation;
            _nextTargetRotation = _nextTargetRotation * xRotationChange * yRotationChange;
            _nextTargetRotation.eulerAngles = new Vector3(_nextTargetRotation.eulerAngles.x, _nextTargetRotation.eulerAngles.y, 0);
            
            _rightInputStartPoint = currentPoint;
        }

        private void HandleMovement()
        {
            if (AppData.InputMode == InputMode.Edit) return;
            
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
            // _nextTargetPosition.x = Mathf.Clamp(_nextTargetPosition.x, XMinMax.x, XMinMax.y);
            // _nextTargetPosition.y = Mathf.Clamp(_nextTargetPosition.y, YMinMax.x, YMinMax.y);
            // _nextTargetPosition.z = Mathf.Clamp(_nextTargetPosition.z, ZMinMax.x, ZMinMax.y);
            
            _target.position = Vector3.Lerp(_target.position, _nextTargetPosition, Time.deltaTime * 5f);
            _target.rotation = Quaternion.Lerp(_target.rotation, _nextTargetRotation, Time.deltaTime * 20f);
        }
    }
}