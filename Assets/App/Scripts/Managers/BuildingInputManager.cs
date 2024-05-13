using System;
using App.OsmBuildingGenerator.Containers;
using App.Scripts.CommonModels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace App.Scripts.Managers
{
    public class BuildingInputManager
    {
        public event Action<BuildingId> BuildingSelected;
        public event Action BuildingDeselected;

        private Camera _camera;
        private LayerMask _buildingLayerMask;
        private LayerMask _terrainLayerMask;
        private bool _clickedOnBuilding;
        private BuildingHook _selectedBuildingHook;
        private float _time = 0;
        private Vector3 _moveOffset;
        
        #region Init&Dispose

        public void Initialize()
        {
            _camera = Camera.main;
            _buildingLayerMask = LayerMask.GetMask("Building");
            _terrainLayerMask = LayerMask.GetMask("Terrain");
        }

        public void Dispose()
        {
            BuildingSelected = delegate { };
        }

        #endregion
        
        private bool IsInputOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
        
        public void Tick(float deltaTime)
        {
            _time += deltaTime;
            if (IsInputOverUI()) return;
            if (Input.GetMouseButtonDown(0))
            {
                _time = 0;
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, int.MaxValue, _buildingLayerMask))
                {
                    if (hit.collider.TryGetComponent<BuildingHook>(out var buildingHook))
                    {
                        if (Physics.Raycast(ray, out var terrainHit, int.MaxValue, _terrainLayerMask))
                        {
                            _moveOffset = buildingHook.transform.position - terrainHit.point;
                        }
                        if (_selectedBuildingHook && _selectedBuildingHook != buildingHook)
                        {
                            _selectedBuildingHook.ToggleOutline(false);
                            BuildingDeselected?.Invoke();
                        }
                        
                        _clickedOnBuilding = true;
                        buildingHook.ToggleOutline(true);
                        _selectedBuildingHook = buildingHook;
                        BuildingSelected?.Invoke(buildingHook.BuildingId);
                    }
                }
                else
                {
                    if (_selectedBuildingHook)
                        _selectedBuildingHook.ToggleOutline(false);
                    BuildingDeselected?.Invoke();
                }
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                _clickedOnBuilding = false;
                _time = 0;
                _moveOffset = Vector3.zero;
            }
            
            var buildingOnMove = false;
            if (_time > .2f && _clickedOnBuilding && _selectedBuildingHook)
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, int.MaxValue, _terrainLayerMask))
                {
                    _selectedBuildingHook.transform.position = hit.point + _moveOffset;
                    buildingOnMove = true;
                }
            }
            
            if (buildingOnMove)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    _selectedBuildingHook.transform.Rotate(Vector3.up, 10 * deltaTime);
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    _selectedBuildingHook.transform.Rotate(Vector3.up, -10 * deltaTime);
                }
            }
        }
    }
}