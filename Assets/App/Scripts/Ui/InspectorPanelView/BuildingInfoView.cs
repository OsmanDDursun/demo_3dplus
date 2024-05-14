using App.OsmBuildingGenerator.OSM;
using App.Scripts.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Scripts.Ui.InspectorPanelView
{
    public class BuildingInfoView : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _metaTagViewContainer;
        [SerializeField] private TextMeshProUGUI _buildingIdText;
        [SerializeField] private TextMeshProUGUI _buildingIndexText;
        [SerializeField] private MetaTagView _metaTagViewPrefab;

        private ComponentPool<MetaTagView> _metaTagViewPool;

        #region Init&Dispose

        public void Initialize(BuildingInfoViewData data)
        {
            _metaTagViewPool ??= new ComponentPool<MetaTagView>(_metaTagViewPrefab, 5, _metaTagViewContainer);
            _buildingIdText.text = data.BuildingId.Value.ToString();
            _buildingIndexText.text = data.BuildingIndex.Value.ToString();
            Clear();
            CreateMetaTagViews(data.MetaTags);
        }

        #endregion

        private void Clear()
        {
            _metaTagViewPool.ReleaseAll();
        }
        
        private void CreateMetaTagViews(RealWorldTerrainOSMMetaTag[] metaTags)
        {
            if (metaTags == null) return;
            
            foreach (var metaTag in metaTags)
            {
                var metaTagView = _metaTagViewPool.Get();
                metaTagView.transform.SetAsLastSibling();
                var title = FilterMetaTagTitle(metaTag.title);
                metaTagView.Initialize(title, metaTag.info);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_metaTagViewContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        }
        
        private string FilterMetaTagTitle(string title)
        {
            return title.Replace("_", " ");
        }
    }
}