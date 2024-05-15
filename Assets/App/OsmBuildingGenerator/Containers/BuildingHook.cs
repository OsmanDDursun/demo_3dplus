using System.Collections.Generic;
using App.OsmBuildingGenerator.OSM;
using App.OsmBuildingGenerator.Utils;
using App.Scripts.CommonModels;
using App.Scripts.Configs;
using App.Scripts.Data;
using App.Scripts.Managers;
using HighlightPlus.Runtime.Scripts;
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
        private readonly List<Vector3> _surfaceVertices = new();
        
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
            _highlightEffect.ProfileLoad(ResourcesManager.Instance.GetHighlightProfile());
            gameObject.layer = LayerMask.NameToLayer("Building");
            
            UpdateBuildingColor();
        }
        
        #endregion
        
        private void UpdateBuildingColor()
        {
            var color = AppConfig.GetBuildingColorForHeight(_dynamicBuilding.baseHeight);
            ChangeColor(color);
        }

        public bool TryGetSurfaceVertices(Vector3 pointOnSurface, out List<Vector3> surfaceVertices, out Vector3 worldCenterPoint)
        {
            pointOnSurface = transform.InverseTransformPoint(pointOnSurface);
            _surfaceVertices.Clear();
            surfaceVertices = _surfaceVertices;
            
            var vertices = _meshFilter.mesh.vertices;
            var triangles = _meshFilter.mesh.triangles;
            float tolerance = 0.01f;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                var v0 = vertices[triangles[i]];
                var v1 = vertices[triangles[i + 1]];
                var v2 = vertices[triangles[i + 2]];

                var triangleNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                var v0ToPoint = pointOnSurface - v0;

                if (Mathf.Abs(Vector3.Dot(triangleNormal, v0ToPoint)) < tolerance)
                {
                    if(!_surfaceVertices.Contains(v0))
                        _surfaceVertices.Add(v0);
                    if(!_surfaceVertices.Contains(v1))
                        _surfaceVertices.Add(v1);
                    if(!_surfaceVertices.Contains(v2))
                        _surfaceVertices.Add(v2);
                }
            }
            
            worldCenterPoint = Vector3.zero;
            foreach (var vertex in _surfaceVertices)
            {
                worldCenterPoint += vertex;
            }
            
            if (_surfaceVertices.Count > 0)
                worldCenterPoint /= _surfaceVertices.Count;
            
            worldCenterPoint = transform.TransformPoint(worldCenterPoint);
            
            return _surfaceVertices.Count > 0;
        }
        
        public void ExtendInDirection(List<Vector3> vertices, Vector3 direction)
        {
            var localDirection = transform.InverseTransformDirection(direction);
            
            var baseVertices = _dynamicBuilding.baseVertices;

            for (int i = 0; i < vertices.Count; i++)
            {
                var vertex = vertices[i];
                
                for (int j = 0; j < baseVertices.Length; j++)
                {
                    var baseVertex = baseVertices[j];
                    var distance = Vector3.Distance(vertex, baseVertex);
                    if (distance < 0.1f)
                    {
                        baseVertices[j] += localDirection;
                        break;
                    }
                }
            }
            
            _dynamicBuilding.baseVertices = baseVertices;
            _dynamicBuilding.Generate();
            _highlightEffect.Refresh(true);
            _meshCollider.sharedMesh = _meshFilter.mesh;
            UpdateBuildingColor();
        }
        
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
            UpdateBuildingColor();
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
            UpdateBuildingColor();
        }

        private void OnDrawGizmos()
        {
            // if (_surfaceVertices == null) return;
            // foreach (var faceVertex in _surfaceVertices)
            // {
            //     var worldPosition = transform.TransformPoint(faceVertex);
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawSphere(worldPosition, 1f);
            // }
        }
    }
}