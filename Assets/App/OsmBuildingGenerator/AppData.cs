using System;
using System.Collections.Generic;
using App.OsmBuildingGenerator.Utils;

namespace App.OsmBuildingGenerator
{
    public static class AppData
    {
        public const RealWorldTerrainOSMOverpassServer OsmServer = RealWorldTerrainOSMOverpassServer.main;
        public const RealWorldTerrainResultType ResultType = RealWorldTerrainResultType.terrain;
        public const RealWorldTerrainBuildingBottomMode BuildingBottomMode = RealWorldTerrainBuildingBottomMode.followRealWorldData;

        public const double LeftLongitude = 32.7306747436523;
        public const double RightLongitude = 32.779426574707;
        public const double TopLatitude = 39.989200592041;
        public const double BottomLatitude = 39.9527816772461;

        public const bool GenerateBuildings = true;
        public const bool BuildingUseColorTags = false;
        public static List<RealWorldTerrainBuildingMaterial> BuildingMaterials;
        public const bool DynamicBuildings = true;
        public const float BuildingBasementDepth = 0;
        public const float BuildingFloorHeight = 3.5f;
        public static readonly RealWorldTerrainRangeI BuildingFloorLimits = new RealWorldTerrainRangeI(5, 7, 1, 50);
        public const float NodataValue = 0;
        public static bool IsCapturing = true;
        
        public static string GetSaveFolder()
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
            var dataPath = "/NetCadDemo/";
            return documentPath + dataPath;
        }
    }
}