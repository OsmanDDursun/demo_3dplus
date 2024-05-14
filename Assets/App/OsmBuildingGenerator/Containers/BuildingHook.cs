using System;
using System.Collections.Generic;
using App.OsmBuildingGenerator.OSM;
using App.Scripts.CommonModels;
using HighlightPlus;
using UnityEngine;

namespace App.OsmBuildingGenerator.Containers
{
    public class BuildingHook : MonoBehaviour
    {
        public int Index { get; private set; }
        public BuildingId BuildingId { get; private set; }
        
        private RealWorldTerrainDynamicBuilding _dynamicBuilding;
        private RealWorldTerrainOSMMeta _meta;
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propertyBlock;
        private Material _outlineMaterial;
        private HighlightEffect _highlightEffect;
        private MeshFilter _meshFilter;
        
        #region Init&Dispose

        public void Initialize(int index, BuildingId id)
        {
            gameObject.AddComponent<MeshCollider>();
            Index = index;
            BuildingId = id;
            _meshFilter = GetComponent<MeshFilter>();
            _propertyBlock = new MaterialPropertyBlock();
            _meshRenderer = GetComponent<MeshRenderer>();
            _dynamicBuilding = GetComponent<RealWorldTerrainDynamicBuilding>();
            _meta = GetComponent<RealWorldTerrainOSMMeta>();
            _highlightEffect = gameObject.AddComponent<HighlightEffect>();
            _highlightEffect.ProfileLoad(AppData.HighlightProfile);
            SetLayer();
        }
        
        #endregion
        
        public float GetHeight()
        {
            return _dynamicBuilding.baseHeight;
        }
        
        public void ChangeColor(Color color)
        {
            for (var i = 0; i < _meshRenderer.materials.Length; i++)
            {
                _meshRenderer.GetPropertyBlock(_propertyBlock, i);
                _propertyBlock.SetColor("_BaseColor", color);
                _meshRenderer.SetPropertyBlock(_propertyBlock, i);
            }
        }
        
        public void ToggleHighlight(bool toggle)
        {
            _highlightEffect.overlay = toggle ? 1 : 0;
        }
        
        public void ToggleOutline(bool toggle)
        {
            _highlightEffect.outline = toggle ? 1 : 0;
            _highlightEffect.highlighted = true;
        }

        private void SetLayer()
        {
            gameObject.layer = LayerMask.NameToLayer("Building");
        }
        
        public RealWorldTerrainOSMMetaTag[] GetMetaTags()
        {
            return _meta.metaInfo;
        }
        
        public void SetHeight(float height)
        {
            _dynamicBuilding.baseHeight = height;
            _dynamicBuilding.Generate();
            _highlightEffect.Refresh(true);
        }
        
        public bool TryGetMetaTagValue(string tag, out string value)
        {
            value = null;
            if (_meta == null || _meta.metaInfo == null) return false;

            foreach (var metaTag in _meta.metaInfo)
            {
                if (metaTag.CompareKeyOrValue(tag, true, false))
                {
                    value = metaTag.info;
                    return true;
                }
            }

            return false;
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red;
        //     var max = _meshRenderer.bounds.max;
        //     var min = _meshRenderer.bounds.min;
        //     
        //     Gizmos.DrawWireSphere(max, 0.1f);
        //     Gizmos.DrawWireSphere(min, 0.1f);
        //     
        //     Gizmos.DrawLine(max, min);
        //     
        //     foreach (var meshVertex in _meshFilter.mesh.vertices)
        //     {
        //         var worldVertex = transform.TransformPoint(meshVertex);
        //         Gizmos.DrawSphere(worldVertex, 0.3f);
        //     }
        // }
    }
}