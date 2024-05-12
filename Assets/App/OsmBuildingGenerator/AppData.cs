using System;
using System.Collections.Generic;
using App.OsmBuildingGenerator.Utils;

namespace App.OsmBuildingGenerator
{
    public static class AppData
    {
        public static RealWorldTerrainOSMOverpassServer osmServer = RealWorldTerrainOSMOverpassServer.main;
        public static RealWorldTerrainResultType resultType = RealWorldTerrainResultType.terrain;
        public static RealWorldTerrainBuildingBottomMode buildingBottomMode = RealWorldTerrainBuildingBottomMode.followRealWorldData;
        
        public static double leftLongitude = 32.6589202880859;
        public static double rightLongitude = 32.7071571350098;
        public static double topLatitude = 39.8802146911621;
        public static double bottomLatitude = 39.8453178405762;

        public static bool useAnchor;
        public static double anchorLatitude;
        public static double anchorLongitude;

        public static bool generateBuildings = true;
        public static bool generateGrass;
        public static bool generateRivers;
        public static bool generateRoads;
        public static bool generateTextures = false;
        public static bool generateTrees;
        public static RealWorldTerrainVector2i terrainCount = RealWorldTerrainVector2i.one;
        public static bool buildingUseColorTags;
        public static List<RealWorldTerrainBuildingMaterial> buildingMaterials;
        public static bool dynamicBuildings = true;
        public static bool buildingSingleRequest = true;
        
        public static float buildingBasementDepth = 0;
        public static float buildingFloorHeight = 3.5f;
        public static RealWorldTerrainRangeI buildingFloorLimits = new RealWorldTerrainRangeI(5, 7, 1, 50);
        public static float nodataValue = 0;
        public static bool isCapturing = true;
        
        public static string GetSaveFolder()
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
            var dataPath = "/NetCadDemo/";
            return documentPath + dataPath;
        }
    }
}