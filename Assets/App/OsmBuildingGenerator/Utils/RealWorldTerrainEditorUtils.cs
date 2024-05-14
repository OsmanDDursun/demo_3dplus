/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using App.OsmBuildingGenerator.Containers;
using App.Scripts.Data;
using UnityEngine;

namespace App.OsmBuildingGenerator.Utils
{
    public static class RealWorldTerrainEditorUtils
    {
        private static RealWorldTerrainItem _lastC2WItem;

        public static Vector3 CoordsToWorld(double mx, float y, double mz, RealWorldTerrainContainer globalContainer)
        {
            mx = (mx - globalContainer.leftMercator) / (globalContainer.rightMercator - globalContainer.leftMercator) * globalContainer.size.x;
            mz = (1 - (mz - globalContainer.topMercator) / (globalContainer.bottomMercator - globalContainer.topMercator)) * globalContainer.size.z;

            return new Vector3((float)mx, y * globalContainer.scale.y, (float)mz) + globalContainer.transform.position;
        }

        public static Vector3 CoordsToWorldWithElevation(Vector3 point, RealWorldTerrainContainer globalContainer, Vector3 offset = default(Vector3))
        {
            bool success;
            return CoordsToWorldWithElevation(point, globalContainer, offset, out success);
        }

        public static Vector3 CoordsToWorldWithElevation(Vector3 point, RealWorldTerrainContainer globalContainer, Vector3 offset, out bool success)
        {
            return CoordsToWorldWithElevation(point.x, point.z, point.y, globalContainer, offset, out success);
        }

        public static Vector3 CoordsToWorldWithElevation(double longitude, double latitude, float altitude, RealWorldTerrainContainer globalContainer, Vector3 offset, out bool success)
        {
            double mx, my;
            RealWorldTerrainUtils.LatLongToMercat(longitude, latitude, out mx, out my);
            success = false;

            // if (globalContainer.prefs.elevationType == RealWorldTerrainElevationType.realWorld)
            // {
            //     double elevation = RealWorldTerrainElevationGenerator.GetElevation(mx, my, false);
            //     success = Math.Abs(elevation - double.MinValue) > double.Epsilon;
            //     if (success) return CoordsToWorld(mx, (float) (elevation - globalContainer.minElevation), my, globalContainer) - offset;
            // }

            Vector3 v = CoordsToWorld(mx, AppData.NodataValue, my, globalContainer) - offset;

            if (_lastC2WItem == null || !_lastC2WItem.Contains(longitude, latitude))
            {
                _lastC2WItem = null;
                for (int i = 0; i < globalContainer.terrains.Length; i++)
                {
                    if (globalContainer.terrains[i].Contains(longitude, latitude))
                    {
                        _lastC2WItem = globalContainer.terrains[i];
                        break;
                    }
                }
            }

            if (_lastC2WItem != null)
            {
                v.y = _lastC2WItem.GetHeightmapValueByMercator(mx, my) + _lastC2WItem.transform.position.y;
                success = true;
            }

            return v;
        }
    }
}