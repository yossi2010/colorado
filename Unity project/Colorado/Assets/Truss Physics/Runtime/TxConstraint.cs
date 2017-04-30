/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TxSoftBody))]
public class TxConstraint : TxComponent
{
    #region Classes

    public enum SnapType { Point, Node, Edge }

    [System.Serializable]
    public class Snap
    {
        public SnapType type;
        public string node;
        public string featureB;
        public float minLimit = 0;
        public float maxLimit = 0;
        public float strength = Mathf.Infinity;
        public bool master = true;
        public bool show = false;

        public int flags
        {
            get { return (master ? (1 << 0) : 0); }
        }
    }

    #endregion

    #region Properties

    public bool enableMotor
    {
        get { return m_enableMotor; }
    }

    public string motorAxis
    {
        get { return m_axisNodeSet; }
    }

    public float motorRate
    {
        get { return m_targetRate; }
        set
        {
            m_targetRate = value;
            TxSoftBody softBody = m_attachedBody; // @@@
            if (softBody.exists && TxNative.MotorExists(m_motorID))
                TxNative.MotorInstanceSetRate(softBody.worldID, m_motorInstanceID, m_targetRate);
        }
    }

    public float motorTorque
    {
        get { return m_maxTorque; }
        set
        {
            m_maxTorque = value;
            TxSoftBody softBody = m_attachedBody; // @@@
            if (softBody.exists && TxNative.MotorExists(m_motorID))
                TxNative.MotorInstanceSetTorque(softBody.worldID, m_motorInstanceID, m_maxTorque);
        }
    }

    public bool isBroken
    {
        get
        {
            TxSoftBody softBody = m_attachedBody; // @@@
            return !softBody.exists || !TxNative.JointExists(m_jointID) || TxNative.JointInstanceIsBroken(softBody.worldID, m_jointInstanceID);
        }
    }

    public TxSoftBody attachedBody
    {
        get { return m_attachedBody; }
    }

    public TxBody baseBody
    {
        get { return m_baseBody; }
    }

    public Snap[] snaps
    {
        get { return m_snaps; }
    }

    public Matrix4x4 startMatrix
    {
        get { return m_startMatrix; }
    }

    #endregion

    #region Methods

    public override void Create()
    {
        CreateConstraint();
        base.Create();
    }

    public override void Destroy()
    {
        DestroyConstraint();
        base.Destroy();
    }

    #endregion

    #region Events

    public delegate void OnBreakFn();
    public event OnBreakFn onBreak;

    #endregion

    #region Unity

    void OnValidate()
    {
        foreach (var s in m_snaps)
        {
            s.minLimit = Mathf.Max(0.0f, Mathf.Min(s.minLimit, s.minLimit));
            s.maxLimit = Mathf.Max(0.0f, Mathf.Max(s.minLimit, s.maxLimit));
        }
        if (Application.isPlaying && TxNative.MotorExists(m_motorID))
        {
            int worldID = TxWorld.instance.worldID;
            TxNative.MotorInstanceSetRate(worldID, m_motorInstanceID, m_targetRate);
            TxNative.MotorInstanceSetTorque(worldID, m_motorInstanceID, m_maxTorque);
        }
    }

    #endregion

    #region Protected

    protected override TxComponent GetComponentMaster()
    {
        return GetComponent<TxSoftBody>();
    }

    protected override void RegisterDependencies()
    {
        TxSoftBody softBody = GetComponent<TxSoftBody>();
        if (softBody) AddDependency(softBody);
        if (m_baseBody) AddDependency(m_baseBody);
    }

    protected override void OnAfterStep()
    {
        if (!m_wasBroken && isBroken)
        {
            if (onBreak != null) onBreak();
            m_wasBroken = true;
        }
        base.OnAfterStep();
    }

    #endregion

    #region Private

    void CreateConstraint()
    {
        m_startMatrix = m_baseBody ? m_baseBody.transform.worldToLocalMatrix * transform.localToWorldMatrix : transform.localToWorldMatrix;
        m_showSnaps = !!m_showSnaps;
        TxSoftBody softBody = m_attachedBody = GetComponent<TxSoftBody>();
        m_jointID = TxNative.CreateJoint();
        TxNative.JointSetFlags(m_jointID, (m_disableCollision ? (1 << 0) : 0));
        foreach (Snap s in m_snaps)
        {
            int[] nodes = softBody.truss.FindNodeSet(s.node);
            if (s.type == SnapType.Point && nodes != null && nodes.Length > 1) // @@@
            {
                foreach (int node in nodes)
                {
                    Vector3 position = softBody.transform.TransformPoint(softBody.truss.nodePosition[node]);
                    if (m_baseBody) position = m_baseBody.transform.InverseTransformPoint(position);
                    TxNative.JointSnapToPoint(m_jointID, node, s.minLimit, s.maxLimit, s.strength, s.flags, position);
                }
            }
            else
            {
                if (nodes != null && nodes.Length == 1)
                {
                    int node = nodes[0];
                    switch (s.type)
                    {
                        case SnapType.Point:
                            Vector3 position = softBody.transform.TransformPoint(softBody.truss.nodePosition[node]);
                            if (m_baseBody) position = m_baseBody.transform.InverseTransformPoint(position);
                            TxNative.JointSnapToPoint(m_jointID, node, s.minLimit, s.maxLimit, s.strength, s.flags, position);
                            break;
                        case SnapType.Node:
                            if (!(m_baseBody is TxSoftBody)) Debug.LogError("TRUSS PHYSICS: Soft body '" + softBody.gameObject.name + "' constraint. Base body isn't soft body.");
                            else
                            {
                                int[] nodesB = (m_baseBody as TxSoftBody).truss.FindNodeSet(s.featureB);
                                if (nodesB == null) Debug.LogError("TRUSS PHYSICS: Soft body '" + m_baseBody.gameObject.name + "'. Nodes set '" + s.featureB + "' not found.");
                                else if (nodesB.Length == 1) TxNative.JointSnapToNode(m_jointID, node, s.minLimit, s.maxLimit, s.strength, s.flags, nodesB[0]);
                                else Debug.LogError("TRUSS PHYSICS: Soft body '" + m_baseBody.gameObject.name + "'. Nodes set '" + s.featureB + "' should contain one node.");
                            }
                            break;
                        case SnapType.Edge:
                            if (!(m_baseBody is TxSoftBody)) Debug.LogError("TRUSS PHYSICS: Soft body '" + m_baseBody.gameObject.name + "' joint. Base body isn't soft body.");
                            else
                            {
                                int[] nodesB = (m_baseBody as TxSoftBody).truss.FindNodeSet(s.featureB);
                                if (nodesB == null) Debug.LogError("TRUSS PHYSICS: Soft body '" + m_baseBody.gameObject.name + "'. Nodes set '" + s.featureB + "' not found.");
                                else if (nodesB.Length == 2) TxNative.JointSnapToEdge(m_jointID, node, s.minLimit, s.maxLimit, s.strength, s.flags, nodesB[0], nodesB[1]);
                                else Debug.LogError("TRUSS PHYSICS: Soft body '" + m_baseBody.gameObject.name + "'. Nodes set '" + s.featureB + "' should contain two nodes.");
                            }
                            break;
                    }
                }
            }
        }
        m_jointInstanceID = TxNative.WorldObjectAttachJoint(softBody.worldID, softBody.objectID, m_baseBody ? m_baseBody.objectID : -1, m_jointID);
        if (m_enableMotor)
        {
            m_motorID = TxNative.CreateMotor();
            int[] axis = softBody.truss.FindNodeSet(m_axisNodeSet);
            if (axis == null) Debug.LogError("TRUSS PHYSICS: Soft body '" + softBody.gameObject.name + "'. Nodes set '" + m_axisNodeSet + "' not found.");
            else if (axis.Length == 2)
            {
                TxNative.MotorSetAxis(m_motorID, axis[0], axis[1]);
            }
            else Debug.LogError("TRUSS PHYSICS: Soft body '" + softBody.gameObject.name + "'. Nodes set '" + m_axisNodeSet + "' should contain two nodes.");
            m_motorInstanceID = TxNative.WorldObjectAttachMotor(softBody.worldID, softBody.objectID, m_baseBody ? m_baseBody.objectID : -1, m_motorID);
            TxNative.MotorInstanceSetRate(softBody.worldID, m_motorInstanceID, m_targetRate);
            TxNative.MotorInstanceSetTorque(softBody.worldID, m_motorInstanceID, m_maxTorque);
        }
    }

    void DestroyConstraint()
    {
        TxSoftBody softBody = m_attachedBody;
        if (softBody.exists)
        {
            TxNative.WorldObjectDetachJoint(softBody.worldID, softBody.objectID, m_jointInstanceID);
            if (TxNative.JointExists(m_jointID)) TxNative.DestroyJoint(m_jointID);
            TxNative.WorldObjectDetachMotor(softBody.worldID, softBody.objectID, m_motorInstanceID);
            if (TxNative.MotorExists(m_motorID)) TxNative.DestroyMotor(m_motorID);
        }
    }

    [SerializeField]
    TxBody m_baseBody = null;
    [SerializeField]
    bool m_disableCollision = true;
    [SerializeField]
    bool m_showSnaps = false;
    [SerializeField]
    Snap[] m_snaps = new Snap[0];
    [SerializeField]
    bool m_enableMotor = false;
    [SerializeField]
    string m_axisNodeSet = "";
    [SerializeField]
    float m_targetRate = 0;
    [SerializeField]
    float m_maxTorque = 0;
    
    int m_jointID = -1;
    int m_jointInstanceID = -1;
    int m_motorID = -1;
    int m_motorInstanceID = -1;
    TxSoftBody m_attachedBody = null;
    bool m_wasBroken = false;
    Matrix4x4 m_startMatrix = Matrix4x4.identity;

    #endregion
}
