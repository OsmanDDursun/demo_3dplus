using System;
using System.IO;
using App.OsmBuildingGenerator.Containers;
using App.OsmBuildingGenerator.Utils;
using App.Scripts.CommonModels;
using App.Scripts.Configs;
using App.Scripts.Controllers;
using App.Scripts.Controllers.InputActionControllers;
using App.Scripts.Data;
using App.Scripts.Helpers;
using UnityEngine;

namespace App.Scripts.Managers
{
    public class AppManager : SingletonBehaviour<AppManager>
    {
        public event Action Initialized;
        public event Action<InputMode> InputModeChanged;
        
        
        [SerializeField] private RealWorldTerrainContainer _terrainContainer;

        public bool IsInitialized { get; private set; }
        public MapManager MapManager { get; private set; }
        public InputActionManager InputActionManager { get; private set; }
        
        protected override void OnAwake() => Initialize();
        private void OnDestroy() => Dispose();

        #region Init&Dispose

        private void Initialize()
        {
            RealWorldTerrainOSMUtils.InitOSMServer();
            
            if (!Directory.Exists(AppConfig.GetSaveDataFolder()))
            {
                Directory.CreateDirectory(AppConfig.GetSaveDataFolder());
            }
            
            InitializeEssentials();
            IsInitialized = true;
            Initialized?.Invoke();
        }
        
        private void Dispose()
        {
            IsInitialized = false;
            DisposeEssentials();
        }

        #endregion Init&Dispose
        
        public void ChangeInputMode(InputMode inputMode)
        {
            AppData.InputMode = inputMode;
            InputModeChanged?.Invoke(inputMode);
        }

        private void Update()
        {
            if (!IsInitialized) return;
            
            InputActionManager.Tick(Time.deltaTime);
        }

        private void InitializeEssentials()
        {
            MapManager = new MapManager();
            InputActionManager = new InputActionManager(this);
            
            
            MapManager.Initialize(_terrainContainer);
            InputActionManager.Initialize();
        }
        
        private void DisposeEssentials()
        {
            MapManager.Dispose();
            InputActionManager.Dispose();
        }
        
        public void QuitApplication()
        {
            Application.Quit();
        }
    }
}
