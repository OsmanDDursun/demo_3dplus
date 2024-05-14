using App.OsmBuildingGenerator.Containers;
using App.Scripts.CommonModels;
using App.Scripts.Data;
using UnityEngine;

namespace App.Scripts.Controllers.InputActionControllers
{
    public class EditModeInputActionController : InputActionController
    {
        
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (IsInputOverUI()) return;
            var ray = Camera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
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
                    }
                }
                else
                {
                    if (SelectedBuildingHook)
                        SelectedBuildingHook.ToggleOutline(false);
                    OnBuildingDeselected();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out var hit, int.MaxValue, BuildingLayerMask))
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
        }
    }
}