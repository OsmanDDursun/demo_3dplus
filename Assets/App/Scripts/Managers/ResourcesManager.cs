using System.Collections.Generic;
using App.OsmBuildingGenerator.Utils;
using App.Scripts.Helpers;
using HighlightPlus.Runtime.Scripts;
using UnityEngine;

namespace App.Scripts.Managers
{
    public class ResourcesManager : SingletonBehaviour<ResourcesManager>
    {
        [SerializeField] private List<RealWorldTerrainBuildingMaterial> _buildingMaterials;
        [SerializeField] private HighlightProfile _highlightProfile;
        
        protected override void OnAwake(){ }
        
        public List<RealWorldTerrainBuildingMaterial> GetBuildingMaterials()
        {
            return _buildingMaterials;
        }
        
        public HighlightProfile GetHighlightProfile()
        {
            return _highlightProfile;
        }
    }
}