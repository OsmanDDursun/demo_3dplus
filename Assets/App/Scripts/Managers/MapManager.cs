using System.Collections.Generic;
using App.OsmBuildingGenerator;
using App.OsmBuildingGenerator.Containers;
using App.OsmBuildingGenerator.Net;
using App.Scripts.CommonModels;
using UnityEngine;

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
                
                foreach (var metaTag in hook.GetMetaTags())
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
        
        public bool GetBuildingHookForIndex(int index, out BuildingHook buildingHook)
        {
            foreach (var (buildingId, hook) in _buildingHooksByBuildingId)
            {
                if (hook.Index != index) continue;
                
                buildingHook = hook;
                return true;
            }

            buildingHook = null;
            return false;
        }
        
        public IEnumerable<BuildingHook> GetBuildingHooksInIndexRange(int min, int max)
        {
            foreach (var (buildingId, hook) in _buildingHooksByBuildingId)
            {
                if (hook.Index >= min && hook.Index <= max)
                {
                    yield return hook;
                }
            }
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
    }
}