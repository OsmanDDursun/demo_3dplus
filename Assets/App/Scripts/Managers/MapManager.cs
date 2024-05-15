using System.Collections.Generic;
using App.OsmBuildingGenerator;
using App.OsmBuildingGenerator.Containers;
using App.OsmBuildingGenerator.Net;
using App.Scripts.CommonModels;
using App.Scripts.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App.Scripts.Managers
{
    public class MapManager
    {
        private readonly Dictionary<BuildingId, BuildingHook> _buildingHooksByBuildingId = new();
        private RealWorldTerrainContainer _terrainContainer;
        private List<BuildingId> _highlightedBuildingIds = new List<BuildingId>();
        
        #region Init&Dispose

        public void Initialize(RealWorldTerrainContainer container)
        {
            AppData.BottomLatitude = container.bottomLatitude;
            AppData.LeftLongitude = container.leftLongitude;
            AppData.TopLatitude = container.topLatitude;
            AppData.RightLongitude = container.rightLongitude;
            
            _terrainContainer = container;
            RegisterEvents();
        }

        public void Dispose()
        {
            _buildingHooksByBuildingId.Clear();
            UnregisterEvents();
        }

        #endregion Init&Dispose
        
        public void CreateBuildings()
        {
            RealWorldTerrainBuildingGenerator.Download((() =>
            {
                RealWorldTerrainBuildingGenerator.Generate(_terrainContainer);
            }));
            RealWorldTerrainDownloadManager.Start();
        }
        
        private void OnBuildingCreated(BuildingId buildingId, BuildingHook hook)
        {
            _buildingHooksByBuildingId.Add(buildingId, hook);
        }
        
        public void HighlightBuilding(BuildingId buildingId)
        {
            if (_highlightedBuildingIds.Contains(buildingId)) return;
            if (!_buildingHooksByBuildingId.TryGetValue(buildingId, out var buildingHook)) return;

            buildingHook.ToggleHighlight(true);
            _highlightedBuildingIds.Add(buildingId);
        }
        
        public int HighlightBuildingsForInfo(string info)
        {
            var count = 0;
            foreach (var (buildingId, hook) in _buildingHooksByBuildingId)
            {
                var metaTags = hook.GetMetaTags();
                if (metaTags == null) continue;
                foreach (var metaTag in metaTags)
                {
                    var lowerInfo = info.ToLower();
                    var lowerMetaInfo = metaTag.info.ToLower();
                    if (lowerMetaInfo.Contains(lowerInfo))
                    {
                        count++;
                        HighlightBuilding(buildingId);
                    }
                }
            }

            return count;
        }
        
        public void DisableAllHighlights()
        {
            foreach (var buildingId in _highlightedBuildingIds)
            {
                if (!_buildingHooksByBuildingId.TryGetValue(buildingId, out var buildingHook)) continue;
                buildingHook.ToggleHighlight(false);
            }
            
            _highlightedBuildingIds.Clear();
        }
        
        public void AssignColorToBuilding(BuildingId buildingId, Color color)
        {
            if (!_buildingHooksByBuildingId.TryGetValue(buildingId, out var buildingHook)) return;

            buildingHook.ChangeColor(color);
        }
        
        public BuildingHook GetBuildingHookForBuildingId(BuildingId buildingId)
        {
            return _buildingHooksByBuildingId[buildingId];
        }
        
        public bool TryGetBuildingHookForBuildingId(BuildingId buildingId, out BuildingHook hook)
        {
            return _buildingHooksByBuildingId.TryGetValue(buildingId, out hook);
        }
        
        public void ChangeBuildingHeight(BuildingId buildingId, float value)
        {
            if (!_buildingHooksByBuildingId.TryGetValue(buildingId, out var buildingHook)) return;
            buildingHook.SetHeight(value);
        }

        public bool GetCoordinatesByWorldPosition(Vector3 worldPosition, out double longitude, out double latitude, out double altitude)
        {
            return _terrainContainer.GetCoordinatesByWorldPosition(worldPosition, out longitude, out latitude, out altitude);
        }
        
        public void CreateBuildingAt(Vector3 size, Vector3 position)
        {
            RealWorldTerrainBuildingGenerator.CreateHouse(size.x, size.y, size.z, position, _terrainContainer);
        }
        
        public void RemoveBuilding(BuildingId buildingId)
        {
            if (!_buildingHooksByBuildingId.TryGetValue(buildingId, out var buildingHook)) return;
            Object.Destroy(buildingHook.gameObject);
            _buildingHooksByBuildingId.Remove(buildingId);
        }
        
        public void ConvertBuildingToSize(BuildingId buildingId, Vector3 size)
        {
            if (!_buildingHooksByBuildingId.TryGetValue(buildingId, out var buildingHook)) return;
            buildingHook.ConvertToSize(size);
        }

        #region Events

        private void RegisterEvents()
        {
            RealWorldTerrainBuildingGenerator.BuildingCreated += OnBuildingCreated;
        }

        private void UnregisterEvents()
        {
            RealWorldTerrainBuildingGenerator.BuildingCreated -= OnBuildingCreated;
        }

        #endregion

        public float GetProgress()
        {
            return (float)RealWorldTerrainDownloadManager.progress;
        }

        public bool TryGetBuildingHookForBuildingIndexId(BuildingIndexId buildingIndexId, out BuildingHook buildingHook)
        {
            foreach (var hook in _buildingHooksByBuildingId.Values)
            {
                if (hook.Index == buildingIndexId)
                {
                    buildingHook = hook;
                    return true;
                }
            }
            
            buildingHook = null;
            return false;
        }
    }
}