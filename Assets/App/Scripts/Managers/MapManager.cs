using App.OsmBuildingGenerator;
using App.OsmBuildingGenerator.Containers;
using App.OsmBuildingGenerator.Net;
using App.OsmBuildingGenerator.Utils;

namespace App.Scripts.Managers
{
    public class MapManager
    {
        #region Init&Dispose

        public void Initialize(RealWorldTerrainContainer container)
        {
            RealWorldTerrainOSMUtils.InitOSMServer();
            RealWorldTerrainBuildingGenerator.Download((() =>
            {
                RealWorldTerrainBuildingGenerator.Generate(container);
            }));
            RealWorldTerrainDownloadManager.Start();
        }

        public void Dispose()
        {
        }

        #endregion Init&Dispose

        public void CreateBuilding()
        {
        }
    }
}