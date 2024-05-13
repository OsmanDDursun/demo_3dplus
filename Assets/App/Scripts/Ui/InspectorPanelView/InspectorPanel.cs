using System.Globalization;
using App.Scripts.CommonModels;
using App.Scripts.Managers;
using App.Scripts.Ui.InspectorPanelView;
using TMPro;
using UnityEngine;

namespace App.Scripts.Ui
{
    public class InspectorPanel : MonoBehaviour
    {
        [SerializeField] private BuildingInfoView _buildingInfoView;
        [SerializeField] private TMP_InputField _buildingHeightInputField;

        private BuildingId _selectedBuildingId;
        
        #region Init&Dispose

        public void Initialize()
        {
            RegisterEvents();
        }

        public void Dispose()
        {
            UnregisterEvents();
        }

        #endregion
        
        public void ShowBuildingInfo(BuildingInfoViewData data)
        {
            _buildingInfoView.Initialize(data);
        }

        #region Events
        
        private void OnBuildingSelected(BuildingId buildingId)
        {
            if (_selectedBuildingId == buildingId) return;
            
            _selectedBuildingId = buildingId;
            var hook = AppManager.Instance.MapManager.GetBuildingHookForBuildingId(buildingId);
            var infoData = new BuildingInfoViewData(buildingId, hook.Index, hook.GetMetaTags());
            _buildingHeightInputField.text = hook.GetHeight().ToString(CultureInfo.InvariantCulture);
            _buildingHeightInputField.onValueChanged.AddListener(OnBuildingHeightInputFieldChanged);
            ShowBuildingInfo(infoData);
        }
        
        private void OnBuildingDeselected()
        {
            _buildingHeightInputField.onValueChanged.RemoveAllListeners();
            _selectedBuildingId = BuildingId.Invalid;
        }

        private void OnBuildingHeightInputFieldChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            if (!float.TryParse(value, out var height)) return;
            if (!_selectedBuildingId.IsValid) return;
            
            AppManager.Instance.MapManager.ChangeBuildingHeight(_selectedBuildingId, height);
        }

        private void RegisterEvents()
        {
            AppManager.Instance.BuildingInputManager.BuildingSelected += OnBuildingSelected;
            AppManager.Instance.BuildingInputManager.BuildingDeselected += OnBuildingDeselected;
        }

        private void UnregisterEvents()
        {
            if (AppManager.Instance)
            {
                AppManager.Instance.BuildingInputManager.BuildingSelected -= OnBuildingSelected;
                AppManager.Instance.BuildingInputManager.BuildingDeselected -= OnBuildingDeselected;
            }
        }

        #endregion
    }
}