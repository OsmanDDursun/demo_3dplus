/*         INFINITY CODE         */
/*   https://infinity-code.com   */

#if BUILDR3
using BuildRCities;
#endif

namespace App.OsmBuildingGenerator.Containers
{
    public class RealWorldTerrainBuildR3Material
    {
#if BUILDR3
        public FacadeAsset wallFacade;
        public DynamicTextureAsset roofTexture;
        public Roof.Types roofType;
#endif
    }
}