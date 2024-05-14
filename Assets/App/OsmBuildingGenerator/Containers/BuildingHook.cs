using System;
using System.Collections.Generic;
using App.OsmBuildingGenerator.OSM;
using App.OsmBuildingGenerator.Utils;
using App.Scripts.CommonModels;
using HighlightPlus;
using UnityEngine;

namespace App.OsmBuildingGenerator.Containers
{
    public class BuildingHook : MonoBehaviour
    {
        public BuildingIndexId Index { get; private set; }
        public BuildingId BuildingId { get; private set; }
        
        private RealWorldTerrainDynamicBuilding _dynamicBuilding;
        private RealWorldTerrainOSMMeta _meta;
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propertyBlock;
        private Material _outlineMaterial;
        private HighlightEffect _highlightEffect;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        
        #region Init&Dispose

        public void Initialize(BuildingIndexId index, BuildingId id)
        {
            _meshCollider = gameObject.AddComponent<MeshCollider>();
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
            _meshCollider.sharedMesh = _meshFilter.mesh;
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3[] vertices = _meshFilter.mesh.vertices;

            // Bounds'u başlat
            Bounds bounds = new Bounds(_dynamicBuilding.baseVertices[0], Vector3.zero);

            // Tüm vertexleri dolaşarak bounds'u genişlet
            foreach (Vector3 vertex in _dynamicBuilding.baseVertices)
            {
                bounds.Encapsulate(vertex);
            }
            
            var center = transform.TransformPoint(bounds.center);
            var size = bounds.size;
            var min = bounds.min;
            var max = bounds.max;
            
            var xDir = (max - new Vector3(max.x, max.y, min.z)).normalized;
            var yDir = Vector3.up;
            var zDir = (max - new Vector3(min.x, max.y, max.z)).normalized;
            
            //draw directions
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(center, center + xDir * 10);
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(center, center + yDir * 10);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(center, center + zDir * 10);
            
            
        }

        public void ConvertToSize(Vector3 size)
        {
            _dynamicBuilding.baseVertices = new Vector3[]
            {
                new Vector3(-size.x / 2, 0, -size.z / 2),
                new Vector3(-size.x / 2, 0, size.z / 2),
                new Vector3(size.x / 2, 0, size.z / 2),
                new Vector3(size.x / 2, 0, -size.z / 2),
            };
            
            _dynamicBuilding.baseHeight = size.y;
            _dynamicBuilding.startHeight = -AppData.BuildingBasementDepth;
            _dynamicBuilding.roofHeight = 0;
            _dynamicBuilding.roofType = RealWorldTerrainRoofType.flat;
            
            _dynamicBuilding.Generate(true);
            _highlightEffect.Refresh(true);
            _meshCollider.sharedMesh = _meshFilter.mesh;
        }
    }
}