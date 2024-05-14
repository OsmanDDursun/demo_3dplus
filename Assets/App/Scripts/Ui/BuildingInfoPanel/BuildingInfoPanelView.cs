using System;
using System.Globalization;
using App.Scripts.CommonModels;
using App.Scripts.Controllers.InputActionControllers;
using App.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace App.Scripts.Ui.BuildingInfoPanel
{
    public class BuildingInfoPanelView : MonoBehaviour
    {
        [SerializeField] private BuildingInfoView _buildingInfoView;
        [SerializeField] private TMP_InputField _buildingHeightInputField;

        private BuildingId _selectedBuildingId;
        private bool _inputFieldSelected = false;
        
        #region Init&Dispose

        public void Initialize()
        {
            _inputFieldSelected = false;
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

        private void Update()
        {
            if (!_selectedBuildingId.IsValid) return;
            if (_inputFieldSelected) return;
            if (AppManager.Instance.MapManager.TryGetBuildingHookForBuildingId(_selectedBuildingId, out var hook))
                _buildingHeightInputField.text = hook.GetHeight().ToString(CultureInfo.InvariantCulture);
        }

        private void OnBuildingDeselected()
        {
            _buildingInfoView.Clear();
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
        
        private void OnBuildingHeightInputFieldSelected(string arg0)
        {
            _inputFieldSelected = true;
        }
        
        private void OnBuildingHeightInputFieldDeselected(string arg0)
        {
            _inputFieldSelected = false;
        }

        private void RegisterEvents()
        {
            InputActionController.BuildingSelected += OnBuildingSelected;
            InputActionController.BuildingDeselected += OnBuildingDeselected;
            _buildingHeightInputField.onSelect.AddListener(OnBuildingHeightInputFieldSelected);
            _buildingHeightInputField.onDeselect.AddListener(OnBuildingHeightInputFieldDeselected);
        }

        private void UnregisterEvents()
        {
            InputActionController.BuildingSelected -= OnBuildingSelected;
            InputActionController.BuildingDeselected -= OnBuildingDeselected;
            _buildingHeightInputField.onSelect.RemoveAllListeners();
            _buildingHeightInputField.onDeselect.RemoveAllListeners();
        }

        #endregion
    }
}