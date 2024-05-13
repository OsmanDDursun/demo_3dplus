using App.OsmBuildingGenerator.Utils;
using TMPro;
using UnityEngine;

namespace App.Scripts.Ui
{
    public class CoordinateView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _latitudeText;
        [SerializeField] private TextMeshProUGUI _longitudeText;

        public void UpdateView(Vector3 unityPosition)
        {
            RealWorldTerrainUtils.MercatToLatLong(unityPosition.x, unityPosition.z, out var longitude, out var latitude);
            
            _latitudeText.text = latitude.ToString("F10");
            _longitudeText.text = longitude.ToString("F10");
        }
    }
}