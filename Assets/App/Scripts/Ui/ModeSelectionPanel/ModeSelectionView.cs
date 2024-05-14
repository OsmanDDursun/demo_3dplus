using App.Scripts.CommonModels;
using App.Scripts.Data;
using App.Scripts.Managers;
using UnityEngine;

namespace App.Scripts.Ui.ModeSelectionPanel
{
    public class ModeSelectionView : MonoBehaviour
    {
        private ModeSelectionButton[] _modeSelectionButtons;

        #region Init&Dispose

        public void Initialize()
        {
            _modeSelectionButtons = GetComponentsInChildren<ModeSelectionButton>();
            foreach (var modeSelectionButton in _modeSelectionButtons)
            {
                modeSelectionButton.Initialize();
            }
            OnChangeMode(AppData.InputMode);
            RegisterEvents();
        }

        public void Dispose()
        {
            UnregisterEvents();
            
            foreach (var modeSelectionButton in _modeSelectionButtons)
            {
                modeSelectionButton.Dispose();
            }
        }
        
        private void OnChangeMode(InputMode mode)
        {
            foreach (var modeSelectionButton in _modeSelectionButtons)
            {
                if (modeSelectionButton.InputMode == mode)
                {
                    modeSelectionButton.OnSelect();
                }
                else
                {
                    modeSelectionButton.OnDeselect();
                }
            }
        }

        #endregion Init&Dispose
        
        private void OnModeSelectionButtonPressed(InputMode mode)
        {
            if (AppData.InputMode == mode) return;
            
            AppManager.Instance.ChangeInputMode(mode);
            OnChangeMode(mode);
        }

        #region Events

        private void RegisterEvents()
        {
            foreach (var modeSelectionButton in _modeSelectionButtons)
            {
                modeSelectionButton.Pressed += OnModeSelectionButtonPressed;
            }
        }

        private void UnregisterEvents()
        {
            foreach (var modeSelectionButton in _modeSelectionButtons)
            {
                modeSelectionButton.Pressed -= OnModeSelectionButtonPressed;
            }
        }

        #endregion Events
    }
}