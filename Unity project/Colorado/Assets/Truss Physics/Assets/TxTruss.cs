/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

public class TxTruss : ScriptableObject
{
    #region Structures

    [System.Serializable]
    public class NamedSet
    {
        public string name = "";
        public int[] indices = new int[0];
    }

    #endregion

    #region Data

    // Nodes
    public Vector3[] nodePosition = new Vector3[0];
    public float[] nodeMass = new float[0];
    public NamedSet[] nodesSet = new NamedSet[0];

    // Links
    public int[] linkNodes = new int[0];
    public float[] linkLength = new float[0];
    public float[] linkStiffness = new float[0];
    public float[] linkDamping = new float[0];
    public float[] linkElastic = new float[0];
    public float[] linkBreaking = new float[0];
    public float[] linkStretching = new float[0];
    public int[] linkFlags = new int[0];
    public NamedSet[] linksSet = new NamedSet[0];

    // Faces
    public int[] faceNodes = new int[0];
    public int[] faceFlags = new int[0];
    public int[] faceMatter = new int[0];
    public float[] faceEnvelope = new float[0];
    public NamedSet[] facesSet = new NamedSet[0];

    #endregion

    #region Properties

    public int nodeCount
    {
        get { return nodePosition.Length; }
        set
        {
            System.Array.Resize(ref nodePosition, value);
            System.Array.Resize(ref nodeMass, value);
        }
    }

    public int linkCount
    {
        get { return linkNodes.Length / 2; }
        set
        {
            System.Array.Resize(ref linkNodes, value * 2);
            System.Array.Resize(ref linkLength, value);
            System.Array.Resize(ref linkStiffness, value);
            System.Array.Resize(ref linkDamping, value);
            System.Array.Resize(ref linkElastic, value);
            System.Array.Resize(ref linkBreaking, value);
            System.Array.Resize(ref linkStretching, value);
            System.Array.Resize(ref linkFlags, value);
        }
    }

    public int faceCount
    {
        get { return faceNodes.Length / 3; }
        set
        {
            System.Array.Resize(ref faceNodes, value * 3);
            System.Array.Resize(ref faceFlags, value);
            System.Array.Resize(ref faceMatter, value);
            System.Array.Resize(ref faceEnvelope, value);
        }
    }

    #endregion

    #region Methods

    public int[] FindNodeSet(string _name)
    {
        NamedSet nameSet = System.Array.Find(nodesSet, x => x.name == _name);
        return nameSet != null ? nameSet.indices : null;
    }

    public int[] FindLinkSet(string _name)
    {
        NamedSet nameSet = System.Array.Find(linksSet, x => x.name == _name);
        return nameSet != null ? nameSet.indices : null;
    }

    public int[] FindFaceSet(string _name)
    {
        NamedSet nameSet = System.Array.Find(facesSet, x => x.name == _name);
        return nameSet != null ? nameSet.indices : null;
    }

    #endregion
}
