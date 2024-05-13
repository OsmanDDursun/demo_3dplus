using System;
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
        [SerializeField] private Button _clearButton;

        
        #region Init&Dispose

        public void Initialize()
        {
            _countText.text = "0";
            RegisterEvents();
        }

        public void Dispose()
        {
            UnregisterEvents();
        }

        #endregion

        private void OnSearchValueChanged(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return;

            AppManager.Instance.MapManager.DisableAllHighlights();
            var count = AppManager.Instance.MapManager.HighlightBuildingsForInfo(searchText);
            _countText.text = count.ToString();
        }
        
        private void OnClearButtonPressed()
        {
            AppManager.Instance.MapManager.DisableAllHighlights();
            _countText.text = "0";
        }

        #region Events

        private void RegisterEvents()
        {
            _searchFieldText.onValueChanged.AddListener(OnSearchValueChanged);
            _clearButton.onClick.AddListener(OnClearButtonPressed);
        }

        private void UnregisterEvents()
        {
            _searchFieldText.onValueChanged.RemoveAllListeners();
            _clearButton.onClick.RemoveAllListeners();
        }

        #endregion Events
    }
}