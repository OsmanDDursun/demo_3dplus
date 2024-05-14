using App.OsmBuildingGenerator.OSM;
using App.Scripts.CommonModels;

namespace App.Scripts.Ui
{
    public readonly struct BuildingInfoViewData
    {
        public BuildingId BuildingId { get; }
        public BuildingIndexId BuildingIndex { get; }
        public RealWorldTerrainOSMMetaTag[] MetaTags { get; }
        
        public BuildingInfoViewData(BuildingId buildingId, BuildingIndexId buildingIndex, RealWorldTerrainOSMMetaTag[] metaTags)
        {
            BuildingId = buildingId;
            BuildingIndex = buildingIndex;
            MetaTags = metaTags;
        }
    }
}