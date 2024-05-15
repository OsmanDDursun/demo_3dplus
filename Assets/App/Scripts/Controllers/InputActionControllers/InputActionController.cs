using System;
using App.OsmBuildingGenerator.Containers;
using App.Scripts.CommonModels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace App.Scripts.Controllers.InputActionControllers
{
    public class InputActionController
    {
        public static event Action<BuildingId> BuildingSelected;
        public static event Action BuildingDeselected;
        public static event Action<BuildingId> RightClickOnBuilding;
        public static event Action RightClickOnTerrain;

        protected Camera Camera;
        protected LayerMask BuildingLayerMask;
        protected LayerMask TerrainLayerMask;
        protected BuildingHook SelectedBuildingHook;
        
        private float _time = 0;
        private float _timeForRightClick;
        
        #region Init&Dispose

        public void Initialize()
        {
            Camera = Camera.main;
            BuildingLayerMask = LayerMask.GetMask("Building");
            TerrainLayerMask = LayerMask.GetMask("Terrain");
        }

        public void Dispose()
        {
            if (SelectedBuildingHook)
                SelectedBuildingHook.ToggleOutline(false);
            SelectedBuildingHook = null;
            
            OnBuildingDeselected();
        }
        
        protected void OnBuildingSelected(BuildingId buildingId) => BuildingSelected?.Invoke(buildingId);
        protected void OnBuildingDeselected() => BuildingDeselected?.Invoke();
        protected void OnRightClickOnBuilding(BuildingId buildingId) => RightClickOnBuilding?.Invoke(buildingId);
        protected void OnRightClickOnTerrain() => RightClickOnTerrain?.Invoke();

        #endregion
        
        protected bool IsInputOverUI() => EventSystem.current.IsPointerOverGameObject();
        
        public virtual void Tick(float deltaTime)
        {
            _time += deltaTime;
            _timeForRightClick += deltaTime;
            if (IsInputOverUI()) return;
            var ray = Camera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                _time = 0;
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
        }
    }
}