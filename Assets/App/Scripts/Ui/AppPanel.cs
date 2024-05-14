using App.Scripts.Managers;
using App.Scripts.Ui.ActionMenu;
using App.Scripts.Ui.BuildingInfoPanel;
using App.Scripts.Ui.Components;
using App.Scripts.Ui.ModeSelectionPanel;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scripts.Ui
{
    public class AppPanel : MonoBehaviour
    {
        [SerializeField] private BuildingInfoPanelView _inspectorPanel;
        [SerializeField] private CoordinateView _pointerCoordinateView;
        [SerializeField] private SearchView _searchView;
        [SerializeField] private ActionMenuViewController _actionMenuViewController;
        [SerializeField] private ModeSelectionView _modeSelectionView;

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
            _searchView.Initialize();
            _actionMenuViewController.Initialize();
            _modeSelectionView.Initialize();
            
        }

        private void Dispose()
        {
            _inspectorPanel.Dispose();
            _searchView.Dispose();
            _actionMenuViewController.Dispose();
            _modeSelectionView.Dispose();
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
        
        public void OnQuitButtonClicked()
        {
            AppManager.Instance.QuitApplication();
        }
    }
}