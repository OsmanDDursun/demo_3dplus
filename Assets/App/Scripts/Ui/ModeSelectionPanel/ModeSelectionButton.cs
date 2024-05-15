using System;
using App.Scripts.CommonModels;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scripts.Ui.ModeSelectionPanel
{
    public class ModeSelectionButton : MonoBehaviour
    {
        public event Action<InputMode> Pressed;
        
        [SerializeField] private InputMode _inputMode;
        [SerializeField] private UiButtonBase _button;
        [SerializeField] private Image _SelectionFrame;
        [SerializeField] private GameObject _tooltip;

        public InputMode InputMode => _inputMode;
        private float _toolTipTimer = 0;
        private bool _isPointerHovered = false;
        
        #region Init&Dispose

        public void Initialize()
        {
            _button.onClick.AddListener(OnButtonClicked);
            _button.OnPointerEnterEvent.AddListener(OnPointerHover);
            OnDeselect();
        }

        private void OnPointerHover(bool isEnter)
        {
            _isPointerHovered = isEnter;
        }
        
        private void ToggleTooltip(bool show)
        {
            _tooltip.SetActive(show);
        }

        private void Update()
        {
            if (!_isPointerHovered)
            {
                ToggleTooltip(false);
                _toolTipTimer = 0;
                return;
            }
            
            _toolTipTimer += Time.deltaTime;
            if (_toolTipTimer >= .5f)
            {
                ToggleTooltip(true);
            }
        }

        public void Dispose()
        {
            _button.onClick.RemoveAllListeners();
        }

        #endregion Init&Dispose
        
        private void OnButtonClicked()
        {
            Pressed?.Invoke(_inputMode);
        }

        public void OnSelect()
        {
            _SelectionFrame.gameObject.SetActive(true);
        }
        
        public void OnDeselect()
        {
            _SelectionFrame.gameObject.SetActive(false);
        }
    }
}