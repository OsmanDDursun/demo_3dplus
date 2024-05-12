using System;
using System.Collections.Generic;
using System.IO;
using App.OsmBuildingGenerator;
using App.OsmBuildingGenerator.Containers;
using App.OsmBuildingGenerator.Utils;
using App.Scripts.Helpers;
using UnityEngine;

namespace App.Scripts.Managers
{
    public class AppManager : SingletonBehaviour<AppManager>
    {
        [SerializeField] private Material _roofMaterial;
        [SerializeField] private Material _wallMaterial;
        
        [SerializeField] private RealWorldTerrainContainer _terrainContainer;
        public event Action Initialized;

        public MapManager MapManager { get; private set; }
        
        protected override void OnAwake() => Initialize();
        private void OnDestroy() => Dispose();

        #region Init&Dispose

        private void Initialize()
        {
            if (!Directory.Exists(AppData.GetSaveFolder()))
            {
                Directory.CreateDirectory(AppData.GetSaveFolder());
            }
            
            AppData.BuildingMaterials = new List<RealWorldTerrainBuildingMaterial>()
            {
                new()
                {
                    roof = _roofMaterial,
                    wall = _wallMaterial
                }
            };
            
            InitializeEssentials();
            Initialized?.Invoke();
        }
        
        private void Dispose()
        {
            DisposeEssentials();
        }

        #endregion Init&Dispose

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                MapManager.CreateBuilding();
            }
        }

        private void InitializeEssentials()
        {
            MapManager = new MapManager();
            MapManager.Initialize(_terrainContainer);
        }
        
        private void DisposeEssentials()
        {
            MapManager.Dispose();
        }
    }
}
