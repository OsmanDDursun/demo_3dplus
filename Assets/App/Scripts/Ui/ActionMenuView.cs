using System;
using System.Collections.Generic;
using App.Scripts.CommonModels;
using App.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scripts.Ui
{
    public class ActionMenuView : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private List<GameObject> _terrainActions;
        [SerializeField] private List<GameObject> _buildingActions;
        [SerializeField] private Button _addBuildingActionButton;
        [SerializeField] private Button _convertBuildingActionButton;
        [SerializeField] private Button _deleteBuildingActionButton;
        [SerializeField] private AddBuildingMenuView _addBuildingView;

        private BuildingId _selectedBuildingId;
        private LayerMask _terrainLayerMask;
        private Camera _camera;
        private Vector3 _terrainHitPoint;
        private Vector3 _menuPivot;

        #region Init&Dispose

        public void Initialize()
        {
            _camera = Camera.main;
            _terrainLayerMask = LayerMask.GetMask("Terrain");
            RegisterEvents();
        }

        public void Dispose()
        {
            UnregisterEvents();
        }

        #endregion Init&Dispose
        
        public void OpenWithBuilding(BuildingId buildingId)
        {
            _terrainActions.ForEach(action => action.SetActive(false));
            _buildingActions.ForEach(action => action.SetActive(true));
            Open();
        }
        
        public void OpenWithTerrain()
        {
            _terrainActions.ForEach(action => action.SetActive(true));
            _buildingActions.ForEach(action => action.SetActive(false));
            Open();
        }
        
        private void Open()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, int.MaxValue, _terrainLayerMask))
            {
                _terrainHitPoint = hit.point;
            }
            _content.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
            _menuPivot = Input.mousePosition;
            _content.position = _menuPivot;
        }

        private void Close()
        {
            _content.gameObject.SetActive(false);
        }
        
        private bool IsPointerOverContent()
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_content, Input.mousePosition);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !IsPointerOverContent())
            {
                Close();
            }
        }
        
        private void OnDeleteBuildingButtonPressed()
        {
            if (!_selectedBuildingId.IsValid) return;
            
            AppManager.Instance.MapManager.RemoveBuilding(_selectedBuildingId);
            Close();
        }

        private void OnConvertBuildingButtonPressed()
        {
            if (!_selectedBuildingId.IsValid) return;
            
            AppManager.Instance.MapManager.ConvertBuildingToSize(_selectedBuildingId, new Vector3(10, 80, 10));
            Close();
        }

        private void OnAddBuildingButtonPressed()
        {
            _addBuildingView.Open(_terrainHitPoint, _menuPivot);
            Close();
        }
        
        private void OnBuildingSelected(BuildingId buildingId)
        {
            _selectedBuildingId = buildingId;
        }
        
        private void OnBuildingDeselected()
        {
            _selectedBuildingId = BuildingId.Invalid;
        }

        #region Events

        private void RegisterEvents()
        {
            _addBuildingActionButton.onClick.AddListener(OnAddBuildingButtonPressed);
            _convertBuildingActionButton.onClick.AddListener(OnConvertBuildingButtonPressed);
            _deleteBuildingActionButton.onClick.AddListener(OnDeleteBuildingButtonPressed);
            AppManager.Instance.InputActionController.BuildingSelected += OnBuildingSelected;
            AppManager.Instance.InputActionController.BuildingDeselected += OnBuildingDeselected;
        }

        private void UnregisterEvents()
        {
            _addBuildingActionButton.onClick.RemoveAllListeners();
            _convertBuildingActionButton.onClick.RemoveAllListeners();
            _deleteBuildingActionButton.onClick.RemoveAllListeners();
            if (AppManager.Instance)
            {
                AppManager.Instance.InputActionController.BuildingSelected -= OnBuildingSelected;
                AppManager.Instance.InputActionController.BuildingDeselected -= OnBuildingDeselected;
            }
        }

        #endregion Events
    }
}