using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scripts.Ui
{
    public class AppPanel : MonoBehaviour
    {
        [SerializeField] private InspectorPanel _inspectorPanel;
        [SerializeField] private CoordinateView _pointerCoordinateView;

        private LayerMask _terrainLayerMask;
        private Camera _camera;
        
        private void OnEnable() => Initialize();
        private void OnDisable() => Dispose();

        #region Init&Dispose

        private void Initialize()
        {
            _camera = Camera.main;
            _terrainLayerMask = LayerMask.GetMask("Terrain");
            _inspectorPanel.Initialize();
        }

        private void Dispose()
        {
            _inspectorPanel.Dispose();
        }

        #endregion

        private void Update()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, int.MaxValue, _terrainLayerMask))
            {
                _pointerCoordinateView.UpdateView(hit.point);
            }
        }
    }
}