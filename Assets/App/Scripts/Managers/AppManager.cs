using System;
using System.Collections.Generic;
using System.IO;
using App.OsmBuildingGenerator;
using App.OsmBuildingGenerator.Containers;
using App.OsmBuildingGenerator.Utils;
using App.Scripts.Helpers;
using HighlightPlus;
using UnityEngine;

namespace App.Scripts.Managers
{
    public class AppManager : SingletonBehaviour<AppManager>
    {
        [SerializeField] private Material _roofMaterial;
        [SerializeField] private Material _wallMaterial;
        [SerializeField] private HighlightProfile _highlightProfile;
        
        [SerializeField] private RealWorldTerrainContainer _terrainContainer;
        public event Action Initialized;

        public bool IsInitialized { get; private set; }
        public MapManager MapManager { get; private set; }
        public BuildingInputManager BuildingInputManager { get; private set; }
        
        protected override void OnAwake() => Initialize();
        private void OnDestroy() => Dispose();

        #region Init&Dispose

        private void Initialize()
        {
            RealWorldTerrainOSMUtils.InitOSMServer();
            
            if (!Directory.Exists(AppData.GetSaveFolder()))
            {
                Directory.CreateDirectory(AppData.GetSaveFolder());
            }
            
            AppData.HighlightProfile = _highlightProfile;
            
            AppData.BuildingMaterials = new List<RealWorldTerrainBuildingMaterial>()
            {
                new()
                {
                    roof = _roofMaterial,
                    wall = _wallMaterial
                }
            };
            
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

        private void Update()
        {
            if (!IsInitialized) return;
            
            BuildingInputManager.Tick(Time.deltaTime);
        }

        private void InitializeEssentials()
        {
            MapManager = new MapManager();
            BuildingInputManager = new BuildingInputManager();
            
            
            MapManager.Initialize(_terrainContainer);
            BuildingInputManager.Initialize();
            
            MapManager.CreateBuildings();
        }
        
        private void DisposeEssentials()
        {
            MapManager.Dispose();
            BuildingInputManager.Dispose();
        }
    }
}
