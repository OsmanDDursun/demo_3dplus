using App.OsmBuildingGenerator.Utils;
using App.Scripts.CommonModels;

namespace App.Scripts.Data
{
    public static class AppData
    {
        public const RealWorldTerrainOSMOverpassServer OsmServer = RealWorldTerrainOSMOverpassServer.main;
        public const RealWorldTerrainResultType ResultType = RealWorldTerrainResultType.terrain;
        public const RealWorldTerrainBuildingBottomMode BuildingBottomMode = RealWorldTerrainBuildingBottomMode.followRealWorldData;

        public static double LeftLongitude = 32.7306747436523;
        public static double RightLongitude = 32.779426574707;
        public static double TopLatitude = 39.989200592041;
        public static double BottomLatitude = 39.9527816772461;
        
        public const float BuildingBasementDepth = 0;
        public const float BuildingFloorHeight = 3.5f;
        public static readonly RealWorldTerrainRangeI BuildingFloorLimits = new(5, 7, 1, 50);
        public const float NodataValue = 0;
        public static InputMode InputMode = InputMode.Selection;
    }
}