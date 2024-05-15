using System;
using System.Collections.Generic;
using App.OsmBuildingGenerator.Containers;
using App.Scripts.CommonModels;
using App.Scripts.Controllers;
using App.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scripts.Ui
{
    public class SearchView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _searchFieldText;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private TextMeshProUGUI _currentText;
        [SerializeField] private Button _clearButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;

        private List<BuildingHook> _foundedBuildings = new List<BuildingHook>();
        
        private int _currentIndex = -1;
        
        #region Init&Dispose

        public void Initialize()
        {
            _countText.text = "0";
            _currentText.text = "0";
            RegisterEvents();
        }

        public void Dispose()
        {
            UnregisterEvents();
        }

        #endregion

        private void OnSearchValueChanged(string searchText)
        {
            _foundedBuildings.Clear();
            _countText.text =  "0";
            _currentText.text = "0";
            _currentIndex = -1;
            
            if (string.IsNullOrEmpty(searchText))
            {
                OnClearButtonPressed();
                return;
            }
            if (string.IsNullOrWhiteSpace(searchText)) return;
            
            AppManager.Instance.MapManager.DisableAllHighlights();
            AppManager.Instance.MapManager.HighlightBuildingsForInfo(searchText, ref _foundedBuildings);
            _countText.text = _foundedBuildings.Count.ToString();
        }
        
        private void OnClearButtonPressed()
        {
            AppManager.Instance.MapManager.DisableAllHighlights();
            _searchFieldText.text = string.Empty;
            _countText.text = "0";
            _currentText.text = "0";
            _foundedBuildings.Clear();
            _currentIndex = -1;
        }
        
        private void OnPreviousButtonPressed()
        {
            if (_foundedBuildings.Count == 0) return;
            if (_currentIndex <= 0) return;
            
            _currentIndex--;
            _currentText.text = (_currentIndex + 1).ToString();
            var building = _foundedBuildings[_currentIndex];
            CameraController.Instance.FocusTo(building.transform.position, building.GetHeight());
        }

        private void OnNextButtonPressed()
        {
            if (_foundedBuildings.Count == 0) return;
            if (_currentIndex == _foundedBuildings.Count - 1) return;
            
            _currentIndex++;
            _currentText.text = (_currentIndex + 1).ToString();
            var building = _foundedBuildings[_currentIndex];
            CameraController.Instance.FocusTo(building.transform.position, building.GetHeight());
        }

        #region Events

        private void RegisterEvents()
        {
            _searchFieldText.onValueChanged.AddListener(OnSearchValueChanged);
            _clearButton.onClick.AddListener(OnClearButtonPressed);
            _nextButton.onClick.AddListener(OnNextButtonPressed);
            _previousButton.onClick.AddListener(OnPreviousButtonPressed);
        }

        private void UnregisterEvents()
        {
            _searchFieldText.onValueChanged.RemoveAllListeners();
            _clearButton.onClick.RemoveAllListeners();
            _nextButton.onClick.RemoveAllListeners();
            _previousButton.onClick.RemoveAllListeners();
        }

        #endregion Events
    }
}