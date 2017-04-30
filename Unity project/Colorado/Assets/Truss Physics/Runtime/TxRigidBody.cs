/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class TxRigidBody : TxBody
{
    #region Constants

    enum CollisionType
    {
        None,
        Mesh,
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

    enum InteractionType
    {
        Kinematic,
        Heavy,
        Light,
    }

    #endregion

    #region Properties

    public float massScale
    {
        get { return m_massScale; }
        set { m_massScale = value; }
    }

    public float mass
    {
        get
        {
            if (m_rigidBody) return m_rigidBody.mass * m_massScale;
            return Mathf.Infinity;
        }
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

    #endregion

    #region Unity

    public override void Create()
    {
        CreateRigidBody();
        base.Create();
    }

    public override void Destroy()
    {
        DestroyRigidBody();
        base.Destroy();
    }

    protected override void Reset()
    {
        base.Reset();
        if (GetComponent<Collider>() != null) m_collision = CollisionType.Inherit;
        else m_collision = CollisionType.None;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        m_massScale = Mathf.Max(0.01f, m_massScale);
        m_shapeSize.x = Mathf.Max(0.0f, m_shapeSize.x);
        if (m_collision == CollisionType.Capsule) m_shapeSize.y = Mathf.Max(2.0f * m_shapeSize.x, m_shapeSize.y);
        else m_shapeSize.y = Mathf.Max(0.0f, m_shapeSize.y);
        m_shapeSize.z = Mathf.Max(0.0f, m_shapeSize.z);
    }

    #endregion

    #region Protected

    protected override void OnBeforePhysX()
    {
        if (m_rigidBody)
        {
            Rigidbody rb = m_rigidBody;
            m_prevPosition = rb.position;
            m_prevCenterOfMass = rb.worldCenterOfMass;
            m_prevRotation = rb.rotation;
        }
        base.OnBeforePhysX();
    }

    protected override void OnBeforeStep()
    {
        if (m_rigidBody)
        {
            Rigidbody rb = m_rigidBody;
            TxNative.RigidInstanceSetLocation(worldID, m_rigidInstanceID, m_prevPosition, m_prevRotation);
            Vector3 linearVelocity = (rb.worldCenterOfMass - m_prevCenterOfMass) / Time.fixedDeltaTime;
            float angle; Vector3 axis; (rb.rotation * Quaternion.Inverse(m_prevRotation)).ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = axis * angle * Mathf.Deg2Rad / Time.fixedDeltaTime;
            TxNative.RigidInstanceSetVelocity(worldID, m_rigidInstanceID, linearVelocity, angularVelocity);
            if (TxNative.WorldObjectGetEnabled(worldID, objectID))
            {
                if (rb.IsSleeping() == TxNative.WorldObjectGetActive(worldID, objectID))
                    TxNative.WorldObjectSetActive(worldID, objectID, !rb.IsSleeping());
            }
        }
        else
        {
            TxNative.WorldObjectSetTransform(worldID, objectID, transform);
            TxNative.RigidInstanceSetVelocity(worldID, m_rigidInstanceID, Vector3.zero, Vector3.zero);
        }
        base.OnBeforeStep();
    }

    protected override void OnAfterStep()
    {
        if (m_rigidBody)
        {
            Rigidbody rb = m_rigidBody;
            if (TxNative.WorldObjectGetActive(worldID, objectID))
            {
                switch (m_interaction)
                {
                    case InteractionType.Heavy:
                        {
                            Vector3 force = Vector3.zero, torque = Vector3.zero;
                            TxNative.RigidInstanceExtractForces(worldID, m_rigidInstanceID, ref force, ref torque);
                            if (force != Vector3.zero) rb.AddForce(force / m_massScale);
                            if (torque != Vector3.zero) rb.AddTorque(torque / m_massScale);
                        }
                        break;
                    case InteractionType.Light:
                        {
                            Vector3 position = Vector3.zero; Quaternion rotation = Quaternion.identity;
                            TxNative.RigidInstanceGetLocation(worldID, m_rigidInstanceID, ref position, ref rotation);
                            rb.MovePosition(position);
                            rb.velocity = (position - m_prevPosition) / Time.fixedDeltaTime;
                            rb.MoveRotation(rotation);
                            float angle; Vector3 axis; (rotation * Quaternion.Inverse(m_prevRotation)).ToAngleAxis(out angle, out axis);
                            rb.angularVelocity = axis * angle * Mathf.Deg2Rad / Time.fixedDeltaTime;
                        }
                        break;
                }
            }
            else
            {
                rb.Sleep();
            }
        }
        base.OnAfterStep();
    }

    #endregion

    #region Private

    void CreateRigidBody()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        Rigidbody rb = m_rigidBody;
        m_rigidID = TxNative.CreateRigid();
        TxNative.RigidSetType(m_rigidID, (int)m_interaction);
        TxNative.RigidSetMass(m_rigidID, rb.mass * m_massScale, rb.centerOfMass, rb.inertiaTensor * m_massScale, rb.inertiaTensorRotation); // Unity 4 crash on object duplicate
        CreateBody();
        m_rigidInstanceID = TxNative.WorldObjectAttachRigid(worldID, objectID, m_rigidID);
        if (m_collision != CollisionType.None)
        {
            m_matterIDs = new int[m_matters.Length];
            for (int i = 0; i < m_matterIDs.Length; ++i)
            {
                m_matterIDs[i] = TxNative.CreateMatter(m_matters[i]);
            }
            m_shapeID = TxNative.CreateShape();
            Mesh shapeMesh; Vector3 shapeCenter, shapeSize; int capsuleDirection;
            switch (DetectCollisionType(out shapeMesh, out shapeCenter, out shapeSize, out capsuleDirection))
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
        if (m_spawnActive) TxNative.WorldObjectSetActive(worldID, objectID, true);
        else rb.Sleep();
        if (m_deactivation)
        {
            TxNative.RigidInstanceSetDeactivationSpeed(worldID, m_rigidInstanceID, m_deactivationSpeed);
            TxNative.RigidInstanceSetDeactivationTime(worldID, m_rigidInstanceID, m_deactivationTime);
        }
        else
        {
            TxNative.RigidInstanceSetDeactivationSpeed(worldID, m_rigidInstanceID, 0);
            TxNative.RigidInstanceSetDeactivationTime(worldID, m_rigidInstanceID, float.MaxValue);
        }
    }

    void DestroyRigidBody()
    {
        if (TxNative.ShapeExists(m_shapeID))
        {
            TxNative.DestroyShape(m_shapeID);
        }
        for (int i = 0; i < m_matterIDs.Length; ++i)
        {
            TxNative.ReleaseMatter(m_matterIDs[i]);
        }
        Mesh shapeMesh; Vector3 shapeCenter, shapeSize; int capsuleDirection;
        switch (DetectCollisionType(out shapeMesh, out shapeCenter, out shapeSize, out capsuleDirection))
        {
            case CollisionType.Mesh:
                if (TxNative.MeshExists(m_colliderID)) TxNative.ReleaseMesh(m_colliderID);
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
        DestroyBody();
        if (TxNative.RigidExists(m_rigidID))
        {
            TxNative.DestroyRigid(m_rigidID);
        }
    }

    CollisionType DetectCollisionType(out Mesh _shapeMesh, out Vector3 _shapeCenter, out Vector3 _shapeSize, out int _capsuleDirection)
    {
        _shapeMesh = null;
        _shapeCenter = m_shapeCenter;
        _shapeSize = m_shapeSize;
        _capsuleDirection = (int)m_capsuleDirection;
        switch (m_collision)
        {
            case CollisionType.Mesh:
            case CollisionType.Convex:
            case CollisionType.Box:
            case CollisionType.Capsule:
            case CollisionType.Sphere:
                {
                    _shapeMesh = m_mesh;
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

    int m_rigidID;
    int m_rigidInstanceID;
    Vector3 m_prevPosition = Vector3.zero;
    Vector3 m_prevCenterOfMass = Vector3.zero;
    Quaternion m_prevRotation = Quaternion.identity;
    Rigidbody m_rigidBody = null;

    [SerializeField]
    float m_massScale = 1.0f;
    [SerializeField]
    CollisionType m_collision = CollisionType.None;
    [SerializeField]
    Mesh m_mesh = null;
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
    [SerializeField]
    InteractionType m_interaction = InteractionType.Light;
    [SerializeField]
    bool m_deactivation = true;
    [SerializeField]
    float m_deactivationSpeed = 0.2f;
    [SerializeField]
    float m_deactivationTime = 2.0f;
    [SerializeField]
    bool m_spawnActive = true;

    int m_colliderID = -1;
    int m_shapeID = -1;
    int[] m_matterIDs = new int[0];

    #endregion
}
