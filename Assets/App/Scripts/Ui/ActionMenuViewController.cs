using App.Scripts.CommonModels;
using App.Scripts.Managers;
using UnityEngine;

namespace App.Scripts.Ui
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
            AppManager.Instance.InputActionController.RightClickOnBuilding += OnRightClickOnBuilding;
            AppManager.Instance.InputActionController.RightClickOnTerrain += OnRightClickOnTerrain;
        }

        private void UnregisterEvents()
        {
            if (AppManager.Instance == null) return;
            AppManager.Instance.InputActionController.RightClickOnBuilding -= OnRightClickOnBuilding;
            AppManager.Instance.InputActionController.RightClickOnTerrain -= OnRightClickOnTerrain;
        }

        #endregion
    }
}