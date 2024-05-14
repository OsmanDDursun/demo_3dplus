using System;
using App.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scripts.Ui
{
    public class AddBuildingMenuView : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private TMP_InputField _xInputField;
        [SerializeField] private TMP_InputField _yInputField;
        [SerializeField] private TMP_InputField _zInputField;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _closeButton;

        private Vector3 _placementPosition;

        public void Open(Vector3 placementPosition, Vector3 menuPivot)
        {
            _content.position = menuPivot;
            _placementPosition = placementPosition;
            _content.gameObject.SetActive(true);
        }
        
        public void Close()
        {
            _content.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            RegisterEvents();
        }
        
        private void OnDisable()
        {
            UnregisterEvents();
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
        
        private void OnConfirmButtonClick()
        {
            if (!float.TryParse(_xInputField.text, out var x)) return;
            if (!float.TryParse(_yInputField.text, out var y)) return;
            if (!float.TryParse(_zInputField.text, out var z)) return;
            
            var size = new Vector3(x, y, z);
            
            AppManager.Instance.MapManager.AddBuilding(size, _placementPosition);
            Close();
        }

        #region Events

        private void RegisterEvents()
        {
            _confirmButton.onClick.AddListener(OnConfirmButtonClick);
            _closeButton.onClick.AddListener(Close);
        }

        private void UnregisterEvents()
        {
            _confirmButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();
        }

        #endregion Events
    }
}