using App.Scripts.CommonModels;
using App.Scripts.Controllers;
using App.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scripts.Ui
{
    public class GoBuildingSearchView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _searchInputField;
        [SerializeField] private Button _button;
        
        #region Init&Dispose

        public void Initialize()
        {
            RegisterEvents();
        }

        public void Dispose()
        {
            UnregisterEvents();
        }

        #endregion Init&Dispose

        private void OnButtonClicked()
        {
            var searchKeyword = _searchInputField.text;
            if (!int.TryParse(searchKeyword, out var id)) return;
            var buildingIndexId = new BuildingIndexId(id);
            var buildingId = new BuildingId((ulong)id);
            
            if (AppManager.Instance.MapManager.TryGetBuildingHookForBuildingIndexId(buildingIndexId, out var building))
                CameraController.Instance.FocusTo(building.transform.position, building.GetHeight());
            if (AppManager.Instance.MapManager.TryGetBuildingHookForBuildingId(buildingId, out var buildingHook))
                CameraController.Instance.FocusTo(buildingHook.transform.position, buildingHook.GetHeight());
        }
        
        #region Events

        private void RegisterEvents()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void UnregisterEvents()
        {
            _button.onClick.RemoveAllListeners();
        }

        #endregion
    }
}