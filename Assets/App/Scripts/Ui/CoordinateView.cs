using App.OsmBuildingGenerator.Utils;
using App.Scripts.Managers;
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
            AppManager.Instance.MapManager.GetCoordinatesByWorldPosition(unityPosition, out var longitude, out var latitude, out var altitude);
            _latitudeText.text = latitude.ToString("F7");
            _longitudeText.text = longitude.ToString("F7");
        }
    }
}