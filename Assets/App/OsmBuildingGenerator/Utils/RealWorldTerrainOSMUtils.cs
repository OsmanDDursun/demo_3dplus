/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using App.OsmBuildingGenerator.Containers;
using App.OsmBuildingGenerator.OSM;
using UnityEngine;

namespace App.OsmBuildingGenerator.Utils
{
    public static class RealWorldTerrainOSMUtils
    {
        public static object OsmLocker;

        public static object OSMLocker
        {
            get
            {
                if (OsmLocker == null) OsmLocker = new object();
                return OsmLocker;
            }
        }

        public static string OsmURL = "https://overpass-api.de/api/interpreter?data=";

        public static MeshFilter AppendMesh(GameObject gameObject, Mesh mesh, Material material, string assetName)
        {
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            meshFilter.mesh = mesh;

            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.materials = new[] { material };
            renderer.sharedMaterials = new[] { material };

            return meshFilter;
        }

        public static void GenerateCompressedFile(byte[] data, ref Dictionary<string, RealWorldTerrainOSMNode> nodes, ref Dictionary<string, RealWorldTerrainOSMWay> ways, ref List<RealWorldTerrainOSMRelation> relations, string compressedFilename)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(System.Text.Encoding.UTF8.GetString(data));

            if (nodes == null) nodes = new Dictionary<string, RealWorldTerrainOSMNode>();
            if (ways == null) ways = new Dictionary<string, RealWorldTerrainOSMWay>();
            if (relations == null) relations = new List<RealWorldTerrainOSMRelation>();

            if (document.DocumentElement == null) return;
            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                if (node.Name == "node")
                {
                    RealWorldTerrainOSMNode n = new RealWorldTerrainOSMNode(node);
                    if (!nodes.ContainsKey(n.id)) nodes.Add(n.id, n);
                }
                else if (node.Name == "way")
                {
                    RealWorldTerrainOSMWay way = new RealWorldTerrainOSMWay(node);
                    if (!ways.ContainsKey(way.id)) ways.Add(way.id, way);
                }
                else if (node.Name == "relation") relations.Add(new RealWorldTerrainOSMRelation(node));
            }

            SaveOSM(compressedFilename, nodes, ways, relations);

            GC.Collect();
        }

        public static List<Vector3> GetGlobalPointsFromWay(RealWorldTerrainOSMWay way, Dictionary<string, RealWorldTerrainOSMNode> nodes)
        {
            List<Vector3> points = new List<Vector3>();
            if (way.nodeRefs.Count == 0) return points;

            foreach (string nodeRef in way.nodeRefs)
            {
                RealWorldTerrainOSMNode node;
                if (nodes.TryGetValue(nodeRef, out node)) points.Add(new Vector3(node.lng, 0, node.lat));
            }
            return points;
        }

        public static List<Vector3> GetGlobalPointsFromWay(RealWorldTerrainOSMWay way, Dictionary<string, RealWorldTerrainOSMNode> nodes, out float minLng, out float minLat, out float maxLng, out float maxLat)
        {
            minLng = minLat = float.MaxValue;
            maxLng = maxLat = float.MinValue;
            List<Vector3> points = new List<Vector3>();
            if (way.nodeRefs.Count == 0) return points;

            foreach (string nodeRef in way.nodeRefs)
            {
                RealWorldTerrainOSMNode node;
                if (nodes.TryGetValue(nodeRef, out node))
                {
                    if (minLng > node.lng) minLng = node.lng;
                    if (minLat > node.lat) minLat = node.lat;
                    if (maxLng < node.lng) maxLng = node.lng;
                    if (maxLat < node.lat) maxLat = node.lat;

                    points.Add(new Vector3(node.lng, 0, node.lat));
                }
            }
            return points;
        }

        public static float GetTriangleDirectionOffset(Vector2 rp, ref Vector2 vp1, ref Vector2 vp2, float angle)
        {
            rp *= 100;
            if (vp1 == Vector2.zero)
            {
                vp1 = rp;
                return 0;
            }
            if (vp2 == Vector2.zero)
            {
                vp2 = vp1;
                vp1 = rp;
                return RealWorldTerrainUtils.Angle2D(vp2, vp1);
            }

            vp2 = vp1;
            vp1 = rp;
            float a = RealWorldTerrainUtils.Angle2D(vp2, vp1);
            if (a - angle > 180) a -= 360;
            else if (angle - a > 180) a += 360;
            return a - angle;
        }

        public static List<Vector3> GetWorldPointsFromWay(RealWorldTerrainOSMWay way, Dictionary<string, RealWorldTerrainOSMNode> nodes, RealWorldTerrainContainer container)
        {
            List<Vector3> points = new List<Vector3>();
            if (way.nodeRefs.Count == 0) return points;

            foreach (string nodeRef in way.nodeRefs)
            {
                RealWorldTerrainOSMNode node;
                if (nodes.TryGetValue(nodeRef, out node)) points.Add(RealWorldTerrainEditorUtils.CoordsToWorldWithElevation(new Vector3(node.lng, 0, node.lat), container));
            }
            return points;
        }

        public static void InitOSMServer()
        {
            OsmURL = AppData.OsmServer switch
            {
                RealWorldTerrainOSMOverpassServer.main => "https://overpass-api.de/api/interpreter?data=",
                RealWorldTerrainOSMOverpassServer.main2 => "https://z.overpass-api.de/api/interpreter?data=",
                RealWorldTerrainOSMOverpassServer.french => "https://overpass.openstreetmap.fr/api/interpreter?data=",
                RealWorldTerrainOSMOverpassServer.taiwan => "https://overpass.nchc.org.tw/api/interpreter?data=",
                RealWorldTerrainOSMOverpassServer.kumiSystems => "https://overpass.kumi.systems/api/interpreter?data=",
                _ => OsmURL
            };
        }

        public static void LoadOSM(string filename, out Dictionary<string, RealWorldTerrainOSMNode> nodes, out Dictionary<string, RealWorldTerrainOSMWay> ways, out List<RealWorldTerrainOSMRelation> relations, bool moveRelationsToWays = true)
        {
            nodes = new Dictionary<string, RealWorldTerrainOSMNode>();
            relations = new List<RealWorldTerrainOSMRelation>();
            ways = new Dictionary<string, RealWorldTerrainOSMWay>();

            if (!File.Exists(filename)) return;

            FileStream fs = File.OpenRead(filename);
            BinaryReader br = new BinaryReader(fs);

            int nodesCount = br.ReadInt32();

            for (int i = 0; i < nodesCount; i++)
            {
                RealWorldTerrainOSMNode node = new RealWorldTerrainOSMNode(br);
                nodes.Add(node.id, node);
            }
            int wayCount = br.ReadInt32();
            for (int i = 0; i < wayCount; i++)
            {
                RealWorldTerrainOSMWay way = new RealWorldTerrainOSMWay(br);
                if (!ways.ContainsKey(way.id)) ways.Add(way.id, way);
            }
            int relationCount = br.ReadInt32();
            for (int i = 0; i < relationCount; i++) relations.Add(new RealWorldTerrainOSMRelation(br));

            if (moveRelationsToWays) MoveRelationsToWays(relations, ways, nodes);
        }

        private static void MoveRelationToWay(RealWorldTerrainOSMRelation relation, Dictionary<string, RealWorldTerrainOSMWay> ways, List<string> waysInRelation, Dictionary<string, RealWorldTerrainOSMNode> nodes)
        {
            if (relation.members.Count == 0) return;

            List<string> nodeRefs = new List<string>();

            List<RealWorldTerrainOSMRelationMember> members = relation.members.Where(m => m.type == "way" && m.role == "outer").ToList();
            if (members.Count == 0) return;

            RealWorldTerrainOSMWay relationWay;
            if (!ways.TryGetValue(members[0].reference, out relationWay) || relationWay == null) return;

            nodeRefs.AddRange(relationWay.nodeRefs);
            members.RemoveAt(0);

            while (members.Count > 0)
            {
                if (!MoveRelationMemberToWay(nodeRefs, members, ways)) break;
            }

            RealWorldTerrainOSMWay way = new RealWorldTerrainOSMWay();

            members = relation.members.Where(m => m.type == "way" && m.role == "inner").ToList();
            if (members.Count > 0)
            {
                way.holes = new List<RealWorldTerrainOSMWay>();

                foreach (RealWorldTerrainOSMRelationMember member in members)
                {
                    RealWorldTerrainOSMWay holeWay;
                    if (ways.TryGetValue(member.reference, out holeWay)) way.holes.Add(holeWay);
                }
            }

            waysInRelation.AddRange(relation.members.Select(m => m.reference));
            way.nodeRefs = nodeRefs;
            way.id = relation.id;
            way.tags = relation.tags;
            ways.Add(way.id, way);
        }

        private static bool MoveRelationMemberToWay(List<string> nodeRefs, List<RealWorldTerrainOSMRelationMember> members, Dictionary<string, RealWorldTerrainOSMWay> ways)
        {
            string lastRef = nodeRefs[nodeRefs.Count - 1];

            int memberIndex = -1;
            for (int i = 0; i < members.Count; i++)
            {
                RealWorldTerrainOSMRelationMember member = members[i];
                RealWorldTerrainOSMWay w = ways[member.reference];
                if (w.nodeRefs[0] == lastRef)
                {
                    nodeRefs.AddRange(w.nodeRefs.Skip(1));
                    memberIndex = i;
                    break;
                }
                if (w.nodeRefs[w.nodeRefs.Count - 1] == lastRef)
                {
                    List<string> refs = w.nodeRefs;
                    refs.Reverse();
                    nodeRefs.AddRange(refs.Skip(1));
                    memberIndex = i;
                    break;
                }
            }

            if (memberIndex != -1) members.RemoveAt(memberIndex);
            else return false;
            return true;
        }

        private static void MoveRelationsToWays(List<RealWorldTerrainOSMRelation> relations, Dictionary<string, RealWorldTerrainOSMWay> ways, Dictionary<string, RealWorldTerrainOSMNode> nodes)
        {
            List<string> waysInRelation = new List<string>();

            foreach (RealWorldTerrainOSMRelation relation in relations) MoveRelationToWay(relation, ways, waysInRelation, nodes);

            foreach (string id in waysInRelation)
            {
                if (!ways.ContainsKey(id)) continue;
                ways.Remove(id);
            }
        }

        public static void SaveOSM(string filename, Dictionary<string, RealWorldTerrainOSMNode> nodes, Dictionary<string, RealWorldTerrainOSMWay> ways, List<RealWorldTerrainOSMRelation> relations)
        {
            var fs = File.OpenWrite(filename);
            var bw = new BinaryWriter(fs);

            if (nodes != null)
            {
                bw.Write(nodes.Count);
                foreach (KeyValuePair<string, RealWorldTerrainOSMNode> pair in nodes) pair.Value.Write(bw);
            }
            else bw.Write(0);

            if (ways != null)
            {
                bw.Write(ways.Count);
                foreach (KeyValuePair<string, RealWorldTerrainOSMWay> pair in ways) pair.Value.Write(bw);
            }
            else bw.Write(0);

            if (relations != null)
            {
                relations = new List<RealWorldTerrainOSMRelation>(relations.Distinct());
                bw.Write(relations.Count);
                foreach (RealWorldTerrainOSMRelation relation in relations) relation.Write(bw);
            }
            else bw.Write(0);

            bw.Close();
        }
    }
}