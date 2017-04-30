/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using System.Collections.Generic;
using UnityEngine;

public class TxBody : TxComponent
{
    #region Classes

    public class Contact
    {
        public TxBody bodyA;
        public TxBody bodyB;
        public Vector3 averagePosition;
        public Vector3 averageNormal;
        public float minimumDistance;
        public Vector3 totalImpulse;
        public Vector3 relativeVelocity;
    }

    #endregion

    #region Properties

    public int worldID
    {
        get { return m_worldID; }
    }

    public int objectID
    {
        get { return m_objectID; }
    }

    public bool exists
    {
        get { return TxNative.WorldExists(m_worldID) && TxNative.WorldObjectExists(m_worldID, m_objectID); }
    }

    public bool isEnabled
    {
        get { return TxNative.WorldObjectExists(worldID, objectID) && TxNative.WorldObjectGetEnabled(worldID, objectID); }
    }

    public bool groupRoot
    {
        get { return m_groupRoot; }
    }

    public string[] groupLayers
    {
        get { return m_groupLayers; }
    }

    public int groupID
    {
        get { return m_groupID; }
    }

    public Matrix4x4 currentTransform
    {
        get { return TxNative.WorldObjectGetTransform(m_worldID, m_objectID); }
    }

    #endregion

    #region Events

    public delegate void OnCollisionFn(Contact _contact);
    public event OnCollisionFn onCollision;

    #endregion

    #region Methods

    public void Enable()
    {
        if (TxNative.WorldObjectExists(worldID, objectID))
        {
            TxNative.WorldObjectSetEnabled(worldID, objectID, true);
        }
    }

    public void Disable()
    {
        if (TxNative.WorldObjectExists(worldID, objectID))
        {
            TxNative.WorldObjectSetEnabled(worldID, objectID, false);
        }
    }

    public TxBody FindParentBody()
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            TxBody body = parent.GetComponent<TxBody>();
            if (body != null) return body;
            parent = parent.parent;
        }
        return null;
    }

    static public TxBody Find(int _objectID)
    {
        TxBody body;
        if (sm_bodies.TryGetValue(_objectID, out body)) return body;
        return null;
    }

    #endregion

    #region Unity

    protected virtual void Reset()
    {
        m_groupRoot = false;
        m_groupLayer = 0;
        m_groupLayers = new string[0];
        m_groupCollision = new bool[0];
        TxDestroy.IfNotUnique(this);
    }

    protected virtual void OnValidate()
    {
        if (m_groupRoot)
        {
            if (m_groupLayers.Length != 8)
                m_groupLayers = new string[8] { "Default", "", "", "", "", "", "", "" };
            if (m_groupCollision.Length != 36)
                m_groupCollision = new bool[36];
        }
        else
        {
            m_groupLayers = new string[0];
            m_groupCollision = new bool[0];
        }
    }

    #endregion

    #region Protected

    protected override TxComponent GetComponentMaster()
    {
        TxBody parent = FindParentBody();
        return parent != null ? (TxComponent)parent : (TxComponent)TxWorld.instance;
    }

    protected override void RegisterDependencies()
    {
        TxBody parentBody = FindParentBody();
        if (parentBody) AddDependency(parentBody);
        base.RegisterDependencies();
    }

    protected override void OnAfterStep()
    {
        if (onCollision != null)
        {
            int contactID = TxNative.WorldObjectFirstContact(m_worldID, m_objectID);
            while (TxNative.WorldContactExists(m_worldID, contactID))
            {
                Contact c = new Contact();
                int objectA = -1, objectB = -1;
                if (TxNative.WorldContactGetInfo(m_worldID, contactID, ref objectA, ref objectB, ref c.averagePosition, ref c.averageNormal, ref c.minimumDistance, ref c.totalImpulse, ref c.relativeVelocity))
                {
                    c.bodyA = TxBody.Find(objectA);
                    c.bodyB = TxBody.Find(objectB);
                    onCollision(c);
                }
                contactID = TxNative.WorldObjectNextContact(m_worldID, m_objectID, contactID);
            }
        }
        base.OnAfterStep();
    }

    protected void CreateBody()
    {
        m_worldID = TxWorld.instance.worldID;
        if (!TxNative.WorldObjectExists(m_worldID, m_objectID))
        {
            m_objectID = TxNative.WorldCreateObject(m_worldID, transform.localToWorldMatrix);

            sm_bodies.Add(m_objectID, this);

            TxNative.WorldObjectSetEnabled(m_worldID, m_objectID, m_spawnEnabled);

            if (m_groupRoot)
            {
                m_groupID = TxNative.WorldCreateGroup(m_worldID);
                TxNative.WorldObjectSetGroup(m_worldID, m_objectID, m_groupID);
                for (int i = 0; i < 8; ++i)
                {
                    for (int j = i; j < 8; ++j)
                    {
                        int index = i * 8 - i * (i + 1) / 2 + j;
                        bool yes = m_groupCollision[index];
                        TxNative.WorldGroupSetColliding(m_worldID, m_groupID, i, j, yes);
                    }
                }
            }
            else
            {
                m_groupID = -1;
                Transform parent = transform;
                while (parent != null)
                {
                    TxBody parentBody = parent.GetComponent<TxBody>();
                    if (parentBody != null && parentBody.groupRoot)
                    {
                        if (!parentBody.isValid) parentBody.Create();
                        m_groupID = parentBody.groupID;
                        break;
                    }
                    parent = parent.parent;
                }
                TxNative.WorldObjectSetGroup(m_worldID, m_objectID, m_groupID);
            }

            TxNative.WorldObjectSetWorldLayer(m_worldID, m_objectID, gameObject.layer);
            TxNative.WorldObjectSetGroupLayer(m_worldID, m_objectID, m_groupLayer);
        }
    }

    protected void DestroyBody()
    {
        if (m_groupRoot && TxNative.WorldExists(m_worldID) && TxNative.WorldGroupExists(m_worldID, m_groupID))
        {
            TxNative.WorldDestroyGroup(m_worldID, m_groupID);
        }
        if (TxNative.WorldExists(m_worldID) && TxNative.WorldObjectExists(m_worldID, m_objectID))
        {
            TxNative.WorldDestroyObject(m_worldID, m_objectID);
            sm_bodies.Remove(m_objectID);
        }
    }

    #endregion

    #region Private

    [SerializeField]
    protected bool m_spawnEnabled = true;
    [SerializeField]
    protected bool m_groupRoot = false;
    [SerializeField]
    protected int m_groupLayer = 0;
    [SerializeField]
    protected string[] m_groupLayers = new string[0];
    [SerializeField]
    protected bool[] m_groupCollision = new bool[0];

    int m_worldID = -1;
    int m_objectID = -1;
    int m_groupID = -1;

    static Dictionary<int, TxBody> sm_bodies = new Dictionary<int, TxBody>();

    #endregion
}
