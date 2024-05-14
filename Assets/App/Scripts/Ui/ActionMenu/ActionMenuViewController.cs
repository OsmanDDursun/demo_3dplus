using App.Scripts.CommonModels;
using App.Scripts.Controllers.InputActionControllers;
using App.Scripts.Managers;
using UnityEngine;

namespace App.Scripts.Ui.ActionMenu
{
    public class ActionMenuViewController : MonoBehaviour
    {
        [SerializeField] private ActionMenuView _actionMenuView;

        #region Init&Dispose

        public void Initialize()
        {
            _actionMenuView.Initialize();
            RegisterEvents();
        }

        public void Dispose()
        {
            _actionMenuView.Dispose();
            UnregisterEvents();
        }

        #endregion Init&Dispose

        #region Events
        
        private void OnRightClickOnTerrain()
        {
            _actionMenuView.OpenWithTerrain();
        }

        private void OnRightClickOnBuilding(BuildingId buildingId)
        {
            _actionMenuView.OpenWithBuilding(buildingId);
        }

        private void RegisterEvents()
        {
            InputActionController.RightClickOnBuilding += OnRightClickOnBuilding;
            InputActionController.RightClickOnTerrain += OnRightClickOnTerrain;
        }

        private void UnregisterEvents()
        {
            if (AppManager.Instance == null) return;
            InputActionController.RightClickOnBuilding -= OnRightClickOnBuilding;
            InputActionController.RightClickOnTerrain -= OnRightClickOnTerrain;
        }

        #endregion
    }
}