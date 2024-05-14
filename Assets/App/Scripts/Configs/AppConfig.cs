using System;

namespace App.Scripts.Configs
{
    public static class AppConfig
    {
        private const string DataPath = "/NetCadDemo/";
        public static string GetSaveDataFolder()
        {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
            return documentPath + DataPath;
        }
    }
}