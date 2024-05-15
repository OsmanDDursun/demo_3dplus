using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Scripts.Configs
{
    public static class AppConfig
    {
        private const string DataPath = "/NetCadDemo/";

        private static readonly Dictionary<float, Color> BuildingHeightColorByMinHeight = new()
        {
            {0, new Color(0, 0.5f, 1, 1)}, //Light blue
            {20, new Color(0.5f, 1, 0, 1)}, //light green
            {40, new Color(.8f, .8f, 0, 1)}, //Yellow
            {80, new Color(1, 0.5f, 0, 1)}, //Orange
            {160, new Color(0.5f, 0.25f, 0, 1)}, //Brown
        };
        
        public static string GetSaveDataFolder()
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
            return documentPath + DataPath;
        }
        
        public static Color GetBuildingColorForHeight(float height)
        {
            var buildingColor = Color.white;
            foreach (var (minHeight, color) in BuildingHeightColorByMinHeight)
            {
                if (height > minHeight)
                {
                    buildingColor = color;
                }
            }

            return buildingColor;
        }
    }
}