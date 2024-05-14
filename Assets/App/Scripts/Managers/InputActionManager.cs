using System.Collections.Generic;
using App.Scripts.CommonModels;
using App.Scripts.Controllers;
using App.Scripts.Controllers.InputActionControllers;
using App.Scripts.Data;

namespace App.Scripts.Managers
{
    public class InputActionManager
    {
        private readonly Dictionary<InputMode, InputActionController> _inputActionControllerMode = new()
        {
            {InputMode.Selection, new SelectionModeInputActionController()},
            {InputMode.Edit, new EditModeInputActionController()},
            {InputMode.Placement, new PlacementModeInputActionController()}
        };

        private InputActionController _activeInputActionController;
        private readonly AppManager _appManager;

        public InputActionManager(AppManager appManager)
        {
            _appManager = appManager;
        }
        
        #region Init&Dispose

        public void Initialize()
        {
            ChangeInputController(AppData.InputMode);
            RegisterEvents();
        }

        public void Dispose()
        {
            UnregisterEvents();
        }

        #endregion Init&Dispose
        
        public void Tick(float deltaTime)
        {
            _activeInputActionController?.Tick(deltaTime);
        }
        
        private void ChangeInputController(InputMode inputMode)
        {
            if (!_inputActionControllerMode.TryGetValue(inputMode, out var inputActionController)) return;
            
            _activeInputActionController?.Dispose();
            _activeInputActionController = inputActionController;
            inputActionController.Initialize();
        }
        
        private void OnInputModeChanged(InputMode inputMode)
        {
            ChangeInputController(inputMode);
        }

        #region Events

        private void RegisterEvents()
        {
            _appManager.InputModeChanged += OnInputModeChanged;
        }

        private void UnregisterEvents()
        {
            _appManager.InputModeChanged -= OnInputModeChanged;
        }

        #endregion Events
    }
}