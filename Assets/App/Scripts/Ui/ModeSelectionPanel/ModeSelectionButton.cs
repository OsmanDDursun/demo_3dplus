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
        [SerializeField] private Button _button;
        [SerializeField] private Image _SelectionFrame;

        public InputMode InputMode => _inputMode;
        
        #region Init&Dispose

        public void Initialize()
        {
            _button.onClick.AddListener(OnButtonClicked);
            OnDeselect();
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