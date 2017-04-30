/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

[ExecuteInEditMode]
public class TxSoftBody : TxBody
{
    #region Properties

    public TxTruss truss
    {
        get { return m_truss; }
    }

    public int trussID
    {
        get { return m_trussID; }
    }

    public int trussInstanceID
    {
        get { return m_trussInstanceID; }
    }

    public Vector3 linearVelocity
    {
        get { return exists ? TxNative.WorldObjectGetLinearVelocity(worldID, objectID) : Vector3.zero; }
    }

    public Vector3 angularVelocity
    {
        get { return exists ? TxNative.WorldObjectGetAngularVelocity(worldID, objectID) : Vector3.zero; }
    }

    public float massScale
    {
        get { return m_massScale; }
        set { m_massScale = value; TxNative.TrussInstanceSetMassScale(worldID, m_trussInstanceID, m_massScale); }
    }

    public int nodeCount
    {
        get { return TxNative.TrussGetNodeCount(trussID); }
    }

    public PropertyArrayRO<Vector3> nodePosition
    {
        get { return new PropertyArrayRO<Vector3>(nodeCount, _nodeIndex => TxNative.TrussInstanceGetNodePosition(worldID, trussInstanceID, _nodeIndex) + transform.position); }
    }
    public PropertyArrayRO<Vector3> nodeLocalPosition
    {
        get { return new PropertyArrayRO<Vector3>(nodeCount, _nodeIndex => TxNative.TrussInstanceGetNodePosition(worldID, trussInstanceID, _nodeIndex)); }
    }

    public PropertyArrayRO<Vector3> nodeVelocity
    {
        get { return new PropertyArrayRO<Vector3>(nodeCount, _nodeIndex => TxNative.TrussInstanceGetNodeVelocity(worldID, trussInstanceID, _nodeIndex)); }
    }

    public int linkCount
    {
        get { return TxNative.TrussGetLinkCount(trussID); }
    }

    public PropertyArrayRO<float> linkLength
    {
        get { return new PropertyArrayRO<float>(linkCount,
                                                _linkIndex => TxNative.TrussGetLinkLength(trussID, _linkIndex)); }
    }

    public PropertyArrayRW<float> linkPlastic
    {
        get { return new PropertyArrayRW<float>(linkCount,
                                                (_linkIndex) => TxNative.TrussInstanceGetLinkPlastic(worldID, trussInstanceID, _linkIndex),
                                                (_linkIndex, _value) => TxNative.TrussInstanceSetLinkPlastic(worldID, trussInstanceID, _linkIndex, _value)); }
    }

    public bool isActive
    {
        get { return TxNative.WorldObjectExists(worldID, objectID) &&  TxNative.WorldObjectGetActive(worldID, objectID); }
    }

    public PropertyArrayRW<TxMatter> matters
    {
        get
        {
            return new PropertyArrayRW<TxMatter>
                (
                    m_matterIDs.Length,
                    (_matterIndex) => m_matters[_matterIndex],
                    (_matterIndex, _value) =>
                    {
                        m_matters[_matterIndex] = _value;
                        int matterID = TxNative.CreateMatter(_value);
                        TxNative.ReleaseMatter(m_matterIDs[_matterIndex]);
                        m_matterIDs[_matterIndex] = matterID;
                        if (TxNative.ShapeExists(m_shapeID))
                        {
                            TxNative.ShapeSetMatters(m_shapeID, m_matterIDs, 0, m_matterIDs.Length);
                        }
                    }
                );
        }
    }

    public bool fastRotation
    {
        get { return m_fastRotation; }
        set
        {
            m_fastRotation = value;
            if (isValid) TxNative.TrussInstanceSetFastRotation(worldID, m_trussInstanceID, m_fastRotation);
        }
    }

    #endregion

    #region Methods

    public void Activate()
    {
        if (TxNative.WorldObjectExists(worldID, objectID))
        {
            TxNative.WorldObjectSetActive(worldID, objectID, true);
        }
    }

    public void ResetGroup()
    {
        TxNative.WorldObjectSetGroup(worldID, objectID, -1);
    }

    public void ApplyImpulse(Vector3 _impulse)
    {
        TxNative.TrussInstanceApplyImpulse(worldID, m_trussInstanceID, _impulse);
    }

    public void ApplyImpulse(Vector3 _impulse, int _node)
    {
        TxNative.TrussInstanceApplyImpulse(worldID, m_trussInstanceID, _impulse, _node);
    }

    public void ApplyImpulse(Vector3 _impulse, Vector3 _point, int _face)
    {
        TxNative.TrussInstanceApplyImpulse(worldID, m_trussInstanceID, _impulse, _point, _face);
    }

    #endregion

    #region Unity

    public override void Create()
    {
        CreateSoftBody();
        base.Create();
    }

    public override void Destroy()
    {
        DestroySoftBody();
        base.Destroy();
    }

    void OnDrawGizmosSelected()
    {
        if (isValid && truss != null && GetComponent<TxTrussDesigner>() == null)
        {
            Gizmos.color = new Color(0.9f, 0.7f, 0.0f);
            int[] links = truss.linkNodes;
            if (Application.isPlaying)
            {
                for (int i = 0; i < links.Length; i += 2)
                    Gizmos.DrawLine(nodePosition[links[i + 0]], nodePosition[links[i + 1]]);
            }
            else
            {
                Vector3[] nodes = truss.nodePosition;
                Gizmos.matrix = transform.localToWorldMatrix;
                for (int i = 0; i < links.Length; i += 2)
                    Gizmos.DrawLine(nodes[links[i + 0]], nodes[links[i + 1]]);
            }
        }
    }

    protected override void Reset()
    {
        base.Reset();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        m_margin = Mathf.Max(m_margin, 0.001f);
        m_adiabaticIndex = Mathf.Clamp(m_adiabaticIndex, 1.1f, 11.1f);
        m_massScale = Mathf.Max(m_massScale, 0.01f);
        if (Application.isPlaying && TxNative.WorldExists(worldID))
        {
            TxNative.TrussInstanceSetMassScale(worldID, m_trussInstanceID, m_massScale);
            if (m_filling)
            {
                TxNative.TrussInstanceSetInternalPressure(worldID, m_trussInstanceID, m_internalPressure);
                TxNative.TrussInstanceSetAdiabaticIndex(worldID, m_trussInstanceID, m_adiabaticIndex);
            }
            else
            {
                TxNative.TrussInstanceSetInternalPressure(worldID, m_trussInstanceID, 0);
                TxNative.TrussInstanceSetAdiabaticIndex(worldID, m_trussInstanceID, 0);
            }
            if (m_deactivation)
            {
                TxNative.TrussInstanceSetDeactivationSpeed(worldID, m_trussInstanceID, m_deactivationSpeed);
                TxNative.TrussInstanceSetDeactivationTime(worldID, m_trussInstanceID, m_deactivationTime);
            }
            else
            {
                TxNative.TrussInstanceSetDeactivationSpeed(worldID, m_trussInstanceID, 0);
                TxNative.TrussInstanceSetDeactivationTime(worldID, m_trussInstanceID, float.MaxValue);
            }
            TxNative.TrussInstanceSetFastRotation(worldID, m_trussInstanceID, m_fastRotation);
        }
        m_spawnActive = m_spawnEnabled ? m_spawnActive : false;
    }

    #endregion

    #region Protected

    protected override void OnAfterUpdate()
    {
        TxNative.WorldObjectGetInterpolatedTransform(worldID, objectID, transform);
        if (m_meshFilter && TxNative.SkinExists(m_skinID) && isActive)
        {
            TxNative.WorldObjectComputeSkinning(worldID, objectID, m_positions, m_normals, m_tangents, 0, m_positions.Length);
            m_meshFilter.mesh.vertices = m_positions;
            m_meshFilter.mesh.normals = m_normals;
            m_meshFilter.mesh.tangents = m_tangents;
            m_meshFilter.mesh.RecalculateBounds();
        }
        base.OnAfterUpdate();
    }

    #endregion

    #region Private

    void CreateSoftBody()
    {
        CreateBody();
        m_trussID = TxNative.CreateTruss(m_truss);
        m_trussInstanceID = TxNative.WorldObjectAttachTruss(worldID, objectID, m_trussID);
        TxNative.TrussInstanceSetMassScale(worldID, m_trussInstanceID, m_massScale);
        if (m_collision)
        {
            m_matterIDs = new int[m_matters.Length];
            for (int i = 0; i < m_matterIDs.Length; ++i)
            {
                m_matterIDs[i] = TxNative.CreateMatter(m_matters[i]);
            }
            m_shapeID = TxNative.CreateShape();
            TxNative.ShapeSetTruss(m_shapeID, m_trussID);
            if (m_matterIDs.Length > 0)
            {
                TxNative.ShapeSetMatterCount(m_shapeID, m_matterIDs.Length);
                TxNative.ShapeSetMatters(m_shapeID, m_matterIDs, 0, m_matterIDs.Length);
            }
            TxNative.ShapeSetMargin(m_shapeID, m_margin);
            TxNative.WorldObjectAttachShape(worldID, objectID, m_shapeID);
        }
        if (m_skinning)
        {
            m_meshFilter = GetComponent<MeshFilter>();
            if (m_meshFilter)
            {
                m_sharedMesh = m_meshFilter.sharedMesh;
                m_meshID = TxNative.CreateMesh(m_meshFilter.sharedMesh);
                m_skinID = TxNative.CreateSkin();
                TxNative.SkinSetup(m_skinID, m_trussID, m_meshID, true);
                TxNative.WorldObjectAttachSkin(worldID, objectID, m_skinID);
                m_positions = m_sharedMesh.vertices; //new Vector3[m_meshFilter.sharedMesh.vertexCount];
                m_normals = m_sharedMesh.normals; //new Vector3[m_meshFilter.sharedMesh.vertexCount];
                m_tangents = m_sharedMesh.tangents;
            }
        }
        if (m_filling)
        {
            TxNative.TrussInstanceSetInternalPressure(worldID, m_trussInstanceID, m_internalPressure);
            TxNative.TrussInstanceSetAdiabaticIndex(worldID, m_trussInstanceID, m_adiabaticIndex);
        }
        else
        {
            TxNative.TrussInstanceSetInternalPressure(worldID, m_trussInstanceID, 0);
            TxNative.TrussInstanceSetAdiabaticIndex(worldID, m_trussInstanceID, 0);
        }
        if (m_spawnActive)
        {
            TxNative.WorldObjectSetActive(worldID, objectID, true);
        }
        if (m_deactivation)
        {
            TxNative.TrussInstanceSetDeactivationSpeed(worldID, m_trussInstanceID, m_deactivationSpeed);
            TxNative.TrussInstanceSetDeactivationTime(worldID, m_trussInstanceID, m_deactivationTime);
        }
        else
        {
            TxNative.TrussInstanceSetDeactivationSpeed(worldID, m_trussInstanceID, 0);
            TxNative.TrussInstanceSetDeactivationTime(worldID, m_trussInstanceID, float.MaxValue);
        }
        if (m_fastRotation)
        {
            TxNative.TrussInstanceSetFastRotation(worldID, m_trussInstanceID, true);
        }
    }

    void DestroySoftBody()
    {
        TxNative.ReleaseTruss(m_trussID);
        if (TxNative.ShapeExists(m_shapeID))
        {
            TxNative.DestroyShape(m_shapeID);
        }
        for (int i = 0; i < m_matterIDs.Length; ++i)
        {
            TxNative.ReleaseMatter(m_matterIDs[i]);
        }
        TxNative.ReleaseMesh(m_meshID);
        if (TxNative.SkinExists(m_skinID))
        {
            TxNative.DestroySkin(m_skinID);
        }
        if (m_sharedMesh) m_meshFilter.sharedMesh = m_sharedMesh;
        DestroyBody();
    }

    [SerializeField]
    TxTruss m_truss = null;
    [SerializeField]
    bool m_collision = true;
    [SerializeField]
    float m_margin = 0.01f;
    [SerializeField]
    TxMatter[] m_matters = new TxMatter[0];
    [SerializeField]
    bool m_skinning = true;
    [SerializeField]
    bool m_filling = false;
    [SerializeField]
    float m_internalPressure = 0.0f;
    [SerializeField]
    float m_adiabaticIndex = 1.0f;
    [SerializeField]
    bool m_deactivation = true;
    [SerializeField]
    float m_deactivationSpeed = 0.2f;
    [SerializeField]
    float m_deactivationTime = 2.0f;
    [SerializeField]
    bool m_spawnActive = true;
    [SerializeField]
    float m_massScale = 1.0f;
    [SerializeField]
    bool m_fastRotation = false;

    int m_trussID = -1;
    int m_trussInstanceID = -1;
    int m_shapeID = -1;
    int[] m_matterIDs = new int[0];
    int m_meshID = -1;
    int m_skinID = -1;
    MeshFilter m_meshFilter = null;
    Mesh m_sharedMesh = null;
    Vector3[] m_positions = new Vector3[0];
    Vector3[] m_normals = new Vector3[0];
    Vector4[] m_tangents = new Vector4[0];

    #endregion
}
