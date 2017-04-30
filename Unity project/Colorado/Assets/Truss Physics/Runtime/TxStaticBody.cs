/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

[ExecuteInEditMode]
public class TxStaticBody : TxBody
{
    #region Constants

    enum CollisionType
    {
        None,
        Mesh,
        Terrain,
        Convex,
        Box,
        Capsule,
        Sphere,
        Inherit,
    }

    enum CapsuleDirection
    {
        X_Axis, Y_Axis, Z_Axis
    }

    #endregion

    #region Properties

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

    #endregion

    #region Unity

    public override void Create()
    {
        CreateStaticBody();
        base.Create();
    }

    public override void Destroy()
    {
        DestroyStaticBody();
        base.Destroy();
    }

    protected override void Reset()
    {
        base.Reset();
        if (GetComponent<Collider>() != null)
        {
            m_collision = CollisionType.Inherit;
        }
        else if (GetComponent<MeshFilter>() != null)
        {
            m_collision = CollisionType.Mesh;
            m_mesh = GetComponent<MeshFilter>().sharedMesh;
        }
        else if (GetComponent<Terrain>() != null)
        {
            m_collision = CollisionType.Terrain;
            m_terrain = GetComponent<Terrain>().terrainData;
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        m_margin = Mathf.Max(m_margin, 0.001f);
        if (m_collision == CollisionType.Capsule)
        {
            m_shapeSize.y = Mathf.Max(m_shapeSize.y, 2.0f * m_shapeSize.x);
        }
    }

    #endregion

    #region Protected

    protected override void OnBeforeStep()
    {
        TxNative.WorldObjectSetTransform(worldID, objectID, transform);
        base.OnBeforeStep();
    }

    #endregion

    #region Private

    void CreateStaticBody()
    {
        CreateBody();
        if (m_collision != CollisionType.None)
        {
            m_matterIDs = new int[m_matters.Length];
            for (int i = 0; i < m_matterIDs.Length; ++i)
            {
                m_matterIDs[i] = TxNative.CreateMatter(m_matters[i]);
            }
            m_shapeID = TxNative.CreateShape();
            Mesh shapeMesh; TerrainData shapeTerrain; Vector3 shapeCenter, shapeSize; int capsuleDirection;
            switch (DetectCollisionType(out shapeMesh, out shapeTerrain, out shapeCenter, out shapeSize, out capsuleDirection))
            {
                case CollisionType.Mesh:
                    {
                        if (shapeMesh != null)
                        {
                            m_colliderID = TxNative.CreateMesh(shapeMesh);
                            TxNative.ShapeSetMesh(m_shapeID, m_colliderID);
                        }
                        break;
                    }
                case CollisionType.Terrain:
                    {
                        if (shapeTerrain != null)
                        {
                            m_colliderID = TxNative.CreateTerrain(shapeTerrain);
                            TxNative.ShapeSetTerrain(m_shapeID, m_colliderID);
                        }
                        break;
                    }
                case CollisionType.Convex:
                    {
                        if (shapeMesh != null)
                        {
                            m_colliderID = TxNative.CreateConvex(shapeMesh);
                            TxNative.ShapeSetConvex(m_shapeID, m_colliderID);
                        }
                        break;
                    }
                case CollisionType.Box:
                    {
                        m_colliderID = TxNative.CreateConvex(shapeCenter, shapeSize);
                        TxNative.ShapeSetConvex(m_shapeID, m_colliderID);
                        break;
                    }
                case CollisionType.Capsule:
                    {
                        m_colliderID = TxNative.CreateConvex(shapeCenter, shapeSize.x, shapeSize.y, capsuleDirection);
                        TxNative.ShapeSetConvex(m_shapeID, m_colliderID);
                        break;
                    }
                case CollisionType.Sphere:
                    {
                        m_colliderID = TxNative.CreateConvex(shapeCenter, shapeSize.x);
                        TxNative.ShapeSetConvex(m_shapeID, m_colliderID);
                        break;
                    }
            }
            TxNative.ShapeSetMatterCount(m_shapeID, m_matterIDs.Length);
            TxNative.ShapeSetMatters(m_shapeID, m_matterIDs, 0, m_matterIDs.Length);
            TxNative.ShapeSetMargin(m_shapeID, m_margin);
            TxNative.WorldObjectAttachShape(worldID, objectID, m_shapeID);
        }
    }

    void DestroyStaticBody()
    {
        if (TxNative.ShapeExists(m_shapeID))
        {
            TxNative.DestroyShape(m_shapeID);
        }
        Mesh shapeMesh; TerrainData shapeTerrain; Vector3 shapeCenter, shapeSize; int capsuleDirection;
        switch (DetectCollisionType(out shapeMesh, out shapeTerrain, out shapeCenter, out shapeSize, out capsuleDirection))
        {
            case CollisionType.Mesh:
                if (TxNative.MeshExists(m_colliderID)) TxNative.ReleaseMesh(m_colliderID);
                break;
            case CollisionType.Terrain:
                if (TxNative.TerrainExists(m_colliderID)) TxNative.ReleaseTerrain(m_colliderID);
                break;
            case CollisionType.Convex:
                if (TxNative.ConvexExists(m_colliderID)) TxNative.ReleaseConvex(m_colliderID);
                break;
            case CollisionType.Box:
            case CollisionType.Capsule:
            case CollisionType.Sphere:
                if (TxNative.ConvexExists(m_colliderID)) TxNative.DestroyConvex(m_colliderID);
                break;
        }
        for (int i = 0; i < m_matterIDs.Length; ++i)
        {
            TxNative.ReleaseMatter(m_matterIDs[i]);
        }
        DestroyBody();
    }

    CollisionType DetectCollisionType(out Mesh _shapeMesh, out TerrainData _shapeTerrain, out Vector3 _shapeCenter, out Vector3 _shapeSize, out int _capsuleDirection)
    {
        _shapeMesh = null;
        _shapeTerrain = null;
        _shapeCenter = m_shapeCenter;
        _shapeSize = m_shapeSize;
        _capsuleDirection = (int)m_capsuleDirection;
        switch (m_collision)
        {
            case CollisionType.Mesh:
            case CollisionType.Terrain:
            case CollisionType.Convex:
            case CollisionType.Box:
            case CollisionType.Capsule:
            case CollisionType.Sphere:
                {
                    _shapeMesh = m_mesh;
                    _shapeTerrain = m_terrain;
                    _shapeCenter = m_shapeCenter;
                    _shapeSize = m_shapeSize;
                    _capsuleDirection = (int)m_capsuleDirection;
                    return m_collision;
                }
            case CollisionType.Inherit:
                {
                    Collider bodyCollider = GetComponent<Collider>();
                    if (bodyCollider != null)
                    {
                        if (bodyCollider is MeshCollider)
                        {
                            MeshCollider meshCollider = bodyCollider as MeshCollider;
                            _shapeMesh = meshCollider.sharedMesh;
                            if (meshCollider.convex) return CollisionType.Convex;
                            else return CollisionType.Mesh;
                        }
                        else if (bodyCollider is TerrainCollider)
                        {
                            TerrainCollider terrainCollider = bodyCollider as TerrainCollider;
                            _shapeTerrain = terrainCollider.terrainData;
                            return CollisionType.Terrain;
                        }
                        else if (bodyCollider is BoxCollider)
                        {
                            BoxCollider boxCollider = bodyCollider as BoxCollider;
                            _shapeCenter = boxCollider.center;
                            _shapeSize = boxCollider.size;
                            return CollisionType.Box;
                        }
                        else if (bodyCollider is CapsuleCollider)
                        {
                            CapsuleCollider capsuleCollider = bodyCollider as CapsuleCollider;
                            _shapeCenter = capsuleCollider.center;
                            _shapeSize = new Vector3(capsuleCollider.radius, capsuleCollider.height, capsuleCollider.radius);
                            _capsuleDirection = capsuleCollider.direction;
                            return CollisionType.Capsule;
                        }
                        else if (bodyCollider is SphereCollider)
                        {
                            SphereCollider sphereCollider = bodyCollider as SphereCollider;
                            _shapeCenter = sphereCollider.center;
                            _shapeSize = new Vector3(sphereCollider.radius, sphereCollider.radius, sphereCollider.radius);
                            return CollisionType.Sphere;
                        }
                    }
                    return CollisionType.None;
                }
        }
        return CollisionType.None;
    }

    [SerializeField]
    CollisionType m_collision = CollisionType.None;
    [SerializeField]
    Mesh m_mesh = null;
    [SerializeField]
    TerrainData m_terrain = null;
    [SerializeField]
    Vector3 m_shapeCenter = Vector3.zero;
    [SerializeField]
    Vector3 m_shapeSize = Vector3.one;
    [SerializeField]
    CapsuleDirection m_capsuleDirection = CapsuleDirection.Y_Axis;
    [SerializeField]
    float m_margin = 0.01f;
    [SerializeField]
    TxMatter[] m_matters = new TxMatter[1];

    int m_colliderID = -1;
    int m_shapeID = -1;
    int[] m_matterIDs = new int[0];

    #endregion
}
