using System;
using App.Models;
using App.OsmBuildingGenerator;
using App.OsmBuildingGenerator.Containers;
using App.Scripts.CommonModels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace App.Scripts.Managers
{
    public class InputActionController
    {
        public event Action<BuildingId> BuildingSelected;
        public event Action BuildingDeselected;
        public event Action<BuildingId> RightClickOnBuilding;
        public event Action RightClickOnTerrain;

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
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && AppData.InputMode == InputMode.Selection)
            {
                _time = 0;
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

            if (Input.GetMouseButtonDown(0) && AppData.InputMode == InputMode.Edit)
            {
                if (Physics.Raycast(ray, out var hit, int.MaxValue, _buildingLayerMask))
                {
                    if (hit.collider.TryGetComponent<BuildingHook>(out var buildingHook))
                    {
                        if (buildingHook.TryGetSurfaceVertices(hit.point, out var surfaceIndexes, out var worldCenterPoint))
                        {
                            buildingHook.ExtendInDirection(surfaceIndexes, hit.normal * 2);
                        }
                    }
                }
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(ray, out var hit, int.MaxValue, _buildingLayerMask))
                {
                    if (hit.collider.TryGetComponent<BuildingHook>(out var buildingHook))
                    {
                        if (_selectedBuildingHook && _selectedBuildingHook != buildingHook)
                        {
                            _selectedBuildingHook.ToggleOutline(false);
                            BuildingDeselected?.Invoke();
                        }
                        
                        buildingHook.ToggleOutline(true);
                        _selectedBuildingHook = buildingHook;
                        BuildingSelected?.Invoke(buildingHook.BuildingId);
                        RightClickOnBuilding?.Invoke(buildingHook.BuildingId);
                    }
                }
                else
                {
                    if (_selectedBuildingHook)
                        _selectedBuildingHook.ToggleOutline(false);
                    BuildingDeselected?.Invoke();
                    RightClickOnTerrain?.Invoke();
                }
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                _clickedOnBuilding = false;
                _time = 0;
                _moveOffset = Vector3.zero;
            }

            if (AppData.InputMode == InputMode.Placement)
            {
                var buildingOnMove = false;
                if (_time > .2f && _clickedOnBuilding && _selectedBuildingHook && AppData.InputMode == InputMode.Placement)
                {
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
}