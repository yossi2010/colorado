/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TxSoftBody))]
public class TxTrussDesigner : MonoBehaviour
{
    #region Fields

    public int subObjects = 0;
    public TxNodeHandle[] nodes = new TxNodeHandle[0];
    public TxLinkHandle[] links = new TxLinkHandle[0];
    public TxFaceHandle[] faces = new TxFaceHandle[0];

    #endregion

    #region Properties

    public TxTruss trussData { get { return GetComponent<TxSoftBody>() ? GetComponent<TxSoftBody>().truss : null; } }

    public TxNodeHandle[] selectedNodes
    {
        get { return System.Array.FindAll(nodes, x => x.selected); }
    }

    public TxLinkHandle[] selectedLinks
    {
        get { return System.Array.FindAll(links, x => x.selected); }
    }

    public TxNodeHandle[] selectedLinksNodes
    {
        get
        {
            List<TxNodeHandle> nodes = new List<TxNodeHandle>();
            foreach (TxLinkHandle l in links)
            {
                if (!l.selected) continue;
                TxNodeHandle node0 = this.nodes[l.node0];
                if (nodes.IndexOf(node0) == -1) nodes.Add(node0);
                TxNodeHandle node1 = this.nodes[l.node1];
                if (nodes.IndexOf(node1) == -1) nodes.Add(node1);
            }
            return nodes.ToArray();
        }
    }

    public TxFaceHandle[] selectedFaces
    {
        get { return System.Array.FindAll(faces, x => x.selected); }
    }

    public TxNodeHandle[] selectedFacesNodes
    {
        get
        {
            List<TxNodeHandle> nodes = new List<TxNodeHandle>();
            foreach (TxFaceHandle f in faces)
            {
                if (!f.selected) continue;
                TxNodeHandle node0 = this.nodes[f.node0];
                if (nodes.IndexOf(node0) == -1) nodes.Add(node0);
                TxNodeHandle node1 = this.nodes[f.node1];
                if (nodes.IndexOf(node1) == -1) nodes.Add(node1);
                TxNodeHandle node2 = this.nodes[f.node2];
                if (nodes.IndexOf(node2) == -1) nodes.Add(node2);
            }
            return nodes.ToArray();
        }
    }

    #endregion

    void Reset()
    {
        CreateHandles();
        TxDestroy.IfNotUnique(this);
    }

    void Start()
    {
        if (Application.isPlaying) DestroyImmediate(this);
    }

    void Update()
    {
        UpdateHandles();
    }

    void OnDestroy()
    {
        DestroyHandles();
    }

    void CreateHandles()
    {
        TxTruss data = trussData;
        if (data == null) return;

        nodes = new TxNodeHandle[data.nodeCount];
        for (int i = 0; i < data.nodeCount; ++i)
        {
            TxNodeHandle node = TxNodeHandle.CreateInstance();
            node.index = i;
            node.name = string.Format("N({0})", i);
            node.position = data.nodePosition[i];
            node.mass = data.nodeMass[i];
            nodes[i] = node;
        }

        links = new TxLinkHandle[data.linkCount];
        for (int i = 0; i < data.linkCount; ++i)
        {
            TxLinkHandle link = TxLinkHandle.CreateInstance();
            link.index = i;
            link.node0 = data.linkNodes[i * 2 + 0];
            link.node1 = data.linkNodes[i * 2 + 1];
            link.name = string.Format("L({0},{1})", link.node0, link.node1);
            link.stiffness = data.linkStiffness[i];
            link.damping = data.linkDamping[i];
            link.elastic = data.linkElastic[i];
            link.breaking = data.linkBreaking[i];
            link.stretching = data.linkStretching.Length > 0 ? data.linkStretching[i] : 1.0f;
            link.resist = (TxLinkHandle.Resist)(data.linkFlags[i] & 0x3);
            links[i] = link;
        }

        faces = new TxFaceHandle[data.faceCount];
        for (int i = 0; i < data.faceCount; ++i)
        {
            TxFaceHandle face = TxFaceHandle.CreateInstance();
            face.index = i;
            face.node0 = data.faceNodes[i * 3 + 0];
            face.node1 = data.faceNodes[i * 3 + 1];
            face.node2 = data.faceNodes[i * 3 + 2];
            face.name = string.Format("F({0},{1},{2})", face.node0, face.node1, face.node2);
            face.collision = (data.faceFlags[i] & (1<<0)) != 0;
            face.skinning = (data.faceFlags[i] & (1<<1)) != 0;
            face.matter = data.faceMatter.Length > 0 ? data.faceMatter[i] : 0;
            face.envelope = data.faceEnvelope[i];
            faces[i] = face;
        }
    }

    void UpdateHandles()
    {
        TxTruss data = trussData;
        if (data == null) return;

        data.nodeCount = nodes.Length;
        foreach (TxNodeHandle n in nodes)
        {
            data.nodePosition[n.index] = n.position;
            data.nodeMass[n.index] = n.mass;
        }

        data.linkCount = links.Length;
        foreach (TxLinkHandle l in links)
        {
            data.linkNodes[l.index * 2 + 0] = l.node0;
            data.linkNodes[l.index * 2 + 1] = l.node1;
            data.linkLength[l.index] = Vector3.Distance(data.nodePosition[l.node0], data.nodePosition[l.node1]);
            data.linkStiffness[l.index] = l.stiffness;
            data.linkDamping[l.index] = l.damping;
            data.linkElastic[l.index] = l.elastic;
            data.linkBreaking[l.index] = l.breaking;
            data.linkStretching[l.index] = l.stretching;
            data.linkFlags[l.index] = (int)l.resist;
        }

        data.faceCount = faces.Length;
        foreach (TxFaceHandle f in faces)
        {
            data.faceNodes[f.index * 3 + 0] = f.node0;
            data.faceNodes[f.index * 3 + 1] = f.node1;
            data.faceNodes[f.index * 3 + 2] = f.node2;
            data.faceFlags[f.index] = (f.collision ? (1<<0) : 0) | (f.skinning ? (1<<1) : 0);
            data.faceMatter[f.index] = f.matter;
            data.faceEnvelope[f.index] = f.envelope;
        }
    }

    void DestroyHandles()
    {
        foreach (TxNodeHandle n in nodes) DestroyImmediate(n);
        foreach (TxLinkHandle l in links) DestroyImmediate(l);
        foreach (TxFaceHandle f in faces) DestroyImmediate(f);
    }

    public TxNodeHandle CreateNode(Vector3 _position, float _mass)
    {
        TxNodeHandle handle = TxNodeHandle.CreateInstance();
        handle.index = nodes.Length;
        handle.name = string.Format("N({0})", handle.index);
        handle.position = _position;
        handle.mass = _mass;
        System.Array.Resize(ref nodes, nodes.Length + 1);
        nodes[handle.index] = handle;
        return handle;
    }

    public delegate void DestroyFn(UnityEngine.Object o);

    public void DeleteNode(int _index, DestroyFn destroyFn = null)
    {
        if (destroyFn != null) destroyFn(nodes[_index]);
        else DestroyImmediate(nodes[_index]);
        for (int i = _index + 1; i < nodes.Length; ++i)
        {
            TxNodeHandle node = nodes[i];
            nodes[--node.index] = node;
        }
        System.Array.Resize(ref nodes, nodes.Length - 1);
        foreach (TxTruss.NamedSet s in trussData.nodesSet)
        {
            List<int> newIndices = new List<int>();
            for (int i = 0; i < s.indices.Length; ++i)
            {
                if (s.indices[i] < _index) newIndices.Add(s.indices[i]);
                else if (s.indices[i] > _index) newIndices.Add(s.indices[i] - 1);
            }
            s.indices = newIndices.ToArray();
        }
        for (int i = links.Length - 1; i >= 0; --i)
        {
            TxLinkHandle link = links[i];
            if (link.node0 == _index || link.node1 == _index) DeleteLink(i, destroyFn);
            else
            {
                if (link.node0 > _index) --link.node0;
                if (link.node1 > _index) --link.node1;
            }
        }
        for (int i = faces.Length - 1; i >= 0; --i)
        {
            TxFaceHandle face = faces[i];
            if (face.node0 == _index || face.node1 == _index || face.node2 == _index) DeleteFace(i, destroyFn);
            else
            {
                if (face.node0 > _index) --face.node0;
                if (face.node1 > _index) --face.node1;
                if (face.node2 > _index) --face.node2;
            }
        }
    }

    public TxLinkHandle CreateLink(int _node0, int _node1)
    {
        TxLinkHandle handle = TxLinkHandle.CreateInstance();
        handle.index = links.Length;
        handle.node0 = _node0;
        handle.node1 = _node1;
        handle.name = string.Format("L({0},{1})", _node0, _node1);
        System.Array.Resize(ref links, links.Length + 1);
        links[handle.index] = handle;
        return handle;
    }

    public void DeleteLink(int _index, DestroyFn destroyFn = null)
    {
        if (destroyFn != null) destroyFn(links[_index]);
        else DestroyImmediate(links[_index]);
        for (int i = _index + 1; i < links.Length; ++i)
        {
            TxLinkHandle link = links[i];
            links[--link.index] = link;
        }
        System.Array.Resize(ref links, links.Length - 1);
        foreach (TxTruss.NamedSet s in trussData.linksSet)
        {
            List<int> newIndices = new List<int>();
            for (int i = 0; i < s.indices.Length; ++i)
            {
                if (s.indices[i] < _index) newIndices.Add(s.indices[i]);
                else if (s.indices[i] > _index) newIndices.Add(s.indices[i] - 1);
            }
            s.indices = newIndices.ToArray();
        }
    }

    public TxFaceHandle CreateFace(int _node0, int _node1, int _node2)
    {
        TxFaceHandle handle = TxFaceHandle.CreateInstance();
        handle.index = faces.Length;
        handle.node0 = _node0;
        handle.node1 = _node1;
        handle.node2 = _node2;
        handle.name = string.Format("F({0},{1},{2})", _node0, _node1, _node2);
        System.Array.Resize(ref faces, faces.Length + 1);
        faces[handle.index] = handle;
        return handle;
    }

    public void DeleteFace(int _index, DestroyFn destroyFn = null)
    {
        if (destroyFn != null) destroyFn(faces[_index]);
        else DestroyImmediate(faces[_index]);
        for (int i = _index + 1; i < faces.Length; ++i)
        {
            TxFaceHandle face = faces[i];
            faces[--face.index] = face;
        }
        System.Array.Resize(ref faces, faces.Length - 1);
        foreach (TxTruss.NamedSet s in trussData.facesSet)
        {
            List<int> newIndices = new List<int>();
            for (int i = 0; i < s.indices.Length; ++i)
            {
                if (s.indices[i] < _index) newIndices.Add(s.indices[i]);
                else if (s.indices[i] > _index) newIndices.Add(s.indices[i] - 1);
            }
            s.indices = newIndices.ToArray();
        }
    }

    public void Optimize()
    {
        int[] remapLinks = RemapLinks();
        List<int> remapNodes = new List<int>();
        for (int i = 0; i < remapLinks.Length; ++i)
        {
            TxLinkHandle link = links[remapLinks[i]];
            link.index = i;
            int node0 = remapNodes.IndexOf(link.node0);
            if (node0 == -1) { node0 = remapNodes.Count; remapNodes.Add(link.node0); }
            link.node0 = node0;
            int node1 = remapNodes.IndexOf(link.node1);
            if (node1 == -1) { node1 = remapNodes.Count; remapNodes.Add(link.node1); }
            link.node1 = node1;
        }
        foreach (TxNodeHandle n in nodes)
        {
            int index = remapNodes.IndexOf(n.index);
            if (index == -1) { index = remapNodes.Count; remapNodes.Add(n.index); }
            n.index = index;
        }
        foreach (TxFaceHandle f in faces)
        {
            f.node0 = nodes[f.node0].index;
            f.node1 = nodes[f.node1].index;
            f.node2 = nodes[f.node2].index;
        }
        TxTruss data = trussData;
        foreach (TxTruss.NamedSet s in data.nodesSet)
        {
            for (int i = 0; i < s.indices.Length; ++i)
            {
                s.indices[i] = nodes[s.indices[i]].index;
            }
        }
        foreach (TxTruss.NamedSet s in data.linksSet)
        {
            for (int i = 0; i < s.indices.Length; ++i)
            {
                s.indices[i] = links[s.indices[i]].index;
            }
        }
        UpdateHandles();
        DestroyHandles();
        CreateHandles();
    }

    public int[] RemapLinks()
    {
        List<int> linkRemap = new List<int>();
        List<int> linkRest = new List<int>();
        for (int i = 0; i < links.Length; ++i) linkRest.Add(i);
        List<int> doesAffect = new List<int>();
        List<int> doesntAffect = new List<int>();
        for (int i = 0; i < links.Length; i += 4)
        {
            for (int j = 0; j < 4; ++j)
            {
                int index = FindLink(linkRest, doesAffect, doesntAffect);
                if (index == -1)
                {
                    linkRemap.AddRange(linkRest);
                    return linkRemap.ToArray();
                }
                linkRemap.Add(index);
                linkRest.Remove(index);
                TxLinkHandle link = links[index];
                doesntAffect.Add(link.node0);
                doesntAffect.Add(link.node1);
                doesAffect.Remove(link.node0);
                doesAffect.Remove(link.node1);
            }
            doesAffect.Clear();
            doesAffect.AddRange(doesntAffect);
            doesntAffect.Clear();
        }
        return linkRemap.ToArray();
    }

    int FindLink(List<int> linkRest, List<int> doesAffect, List<int> doesntAffect)
    {
        int goodIndex = -1;
        int veryGoodIndex = -1;
        for (int i = 0; i < linkRest.Count; ++i)
        {
            int index = linkRest[i];
            TxLinkHandle link = links[index];
            int node0 = link.node0;
            int node1 = link.node1;
            if (doesntAffect.IndexOf(node0) == -1 && doesntAffect.IndexOf(node1) == -1)
            {
                if (goodIndex == -1)
                {
                    goodIndex = index;
                }
                if (doesAffect.IndexOf(node0) != -1 || doesAffect.IndexOf(node1) != -1)
                {
                    if (veryGoodIndex == -1)
                    {
                        goodIndex = index;
                        veryGoodIndex = index;
                    }
                    if (doesAffect.IndexOf(node0) != -1 && doesAffect.IndexOf(node1) != -1)
                    {
                        return index;
                    }
                }
            }
        }
        return goodIndex;
    }

    public void CreateFromMesh(Mesh _mesh)
    {
        int offset = nodes.Length;
        Vector3[] vertices =  _mesh.vertices;
        List<Vector3> uniqueVertices = new List<Vector3>();
        int[] vertexMap = new int[vertices.Length];
        for (int i = 0; i < vertices.Length; ++i)
        {
            Vector3 v0 = vertices[i];
            int index = -1;
            for (int j = 0; j < uniqueVertices.Count; ++j)
            {
                Vector3 v1 = uniqueVertices[j];
                if (v0 == v1)
                {
                    index = j;
                    break;
                }
            }
            if (index == -1)
            {
                index = uniqueVertices.Count;
                uniqueVertices.Add(v0);
            }
            vertexMap[i] = index;
        }
        foreach (var v in uniqueVertices)
        {
            CreateNode(v, 1.0f);
        }
        int[] indices = _mesh.triangles;
        for (int i = 0; i < indices.Length; i += 3)
        {
            for (int j = 0; j < 3; ++j)
            {
                int i0 = vertexMap[indices[i + j]] + offset;
                int i1 = vertexMap[indices[i + (j + 1) % 3]] + offset;
                bool create = true;
                for (int k = i + 3; k < indices.Length && create; k += 3)
                {
                    for (int l = 0; l < 3 && create; ++l)
                    {
                        int i2 = vertexMap[indices[k + l]] + offset;
                        int i3 = vertexMap[indices[k + (l + 1) % 3]] + offset;
                        if (i0 == i3 && i1 == i2) create = false;
                    }
                }
                if (create)
                {
                    CreateLink(i0, i1);
                }
            }
        }
        for (int i = 0; i < indices.Length; i += 3)
        {
            int i0 = vertexMap[indices[i + 0]] + offset;
            int i1 = vertexMap[indices[i + 1]] + offset;
            int i2 = vertexMap[indices[i + 2]] + offset;
            CreateFace(i0, i1, i2);
        }
    }
}
