/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace App.OsmBuildingGenerator.Containers
{
    [ExecuteInEditMode]
    public class RealWorldTerrainDynamicBuilding : RealWorldTerrainBuilding
    {
        private void Awake()
        {
            if (baseVertices != null) Generate();
        }
    }
}