using App.OsmBuildingGenerator.Containers;
using App.Scripts.CommonModels;
using App.Scripts.Data;
using UnityEngine;

namespace App.Scripts.Controllers.InputActionControllers
{
    public class SelectionModeInputActionController : InputActionController
    {
        private float _timeForRightClick;
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (IsInputOverUI()) return;
            var ray = Camera.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(1))
            {
                _timeForRightClick = Time.time;
            }
            
            if (Input.GetMouseButtonUp(1) && Time.time - _timeForRightClick < .2f)
            {
                if (Physics.Raycast(ray, out var hit, int.MaxValue, BuildingLayerMask))
                {
                    if (hit.collider.TryGetComponent<BuildingHook>(out var buildingHook))
                    {
                        if (SelectedBuildingHook && SelectedBuildingHook != buildingHook)
                        {
                            SelectedBuildingHook.ToggleOutline(false);
                            OnBuildingDeselected();
                        }
                        
                        buildingHook.ToggleOutline(true);
                        SelectedBuildingHook = buildingHook;
                        OnBuildingSelected(buildingHook.BuildingId);
                        OnRightClickOnBuilding(buildingHook.BuildingId);
                    }
                }
                else
                {
                    if (SelectedBuildingHook)
                        SelectedBuildingHook.ToggleOutline(false);
                    OnBuildingDeselected();
                    OnRightClickOnTerrain();
                }
            }
        }
    }
}