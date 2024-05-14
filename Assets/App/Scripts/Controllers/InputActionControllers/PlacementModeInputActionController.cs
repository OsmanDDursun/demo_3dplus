using App.OsmBuildingGenerator.Containers;
using App.Scripts.CommonModels;
using App.Scripts.Data;
using UnityEngine;

namespace App.Scripts.Controllers.InputActionControllers
{
    public class PlacementModeInputActionController : InputActionController
    {
        private float _time;
        private Vector3 _moveOffset;
        private bool _clickedOnBuilding;
        
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _time += deltaTime;
            if (IsInputOverUI()) return;
            
            var ray = Camera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                _time = 0;
                if (Physics.Raycast(ray, out var hit, int.MaxValue, BuildingLayerMask))
                {
                    if (hit.collider.TryGetComponent<BuildingHook>(out var buildingHook))
                    {
                        _clickedOnBuilding = true;
                        if (Physics.Raycast(ray, out var terrainHit, int.MaxValue, TerrainLayerMask))
                        {
                            _moveOffset = buildingHook.transform.position - terrainHit.point;
                        }
                    }
                }
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                _clickedOnBuilding = false;
                _time = 0;
                _moveOffset = Vector3.zero;
            }

            var buildingOnMove = false;
            if (_time > .2f && _clickedOnBuilding && SelectedBuildingHook)
            {
                if (Physics.Raycast(ray, out var hit, int.MaxValue, TerrainLayerMask))
                {
                    SelectedBuildingHook.transform.position = hit.point + _moveOffset;
                    buildingOnMove = true;
                }
            }
            
            if (buildingOnMove)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    SelectedBuildingHook.transform.Rotate(Vector3.up, 20 * deltaTime);
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    SelectedBuildingHook.transform.Rotate(Vector3.up, -20 * deltaTime);
                }
            }
        }
    }
}