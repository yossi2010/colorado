/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class TxNative
{
    #region Native Structs

    public enum ShapeType { NONE, TRUSS, MESH, TERRAIN, CONVEX }

    #endregion

    #region Native Functions

#if UNITY_IOS
    const string TRUSS_DLL = "__Internal";
#else
    const string TRUSS_DLL = "TrussPhysics";
#endif

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateWorld(ref int _worldID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldExists(int _worldID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyWorld(int _worldID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldSetSimulationStep(int _worldID, float _step);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldGetSimulationStep(int _worldID, ref float _step);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldSetSimulationSubstepPower(int _worldID, int _power);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldGetSimulationSubstepPower(int _worldID, ref int _power);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldSetSolverIterations(int _worldID, int _iterations);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldGetSolverIterations(int _worldID, ref int _iterations);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldSetGlobalGravity(int _worldID, ref Vector3 _gravity);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldGetGlobalGravity(int _worldID, ref Vector3 _gravity);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldSetGlobalPressure(int _worldID, float _pressure);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldGetGlobalPressure(int _worldID, ref float _pressure);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldCreateGroup(int _worldID, ref int _groupID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldGroupExists(int _worldID, int _groupID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldDestroyGroup(int _worldID, int _groupID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldGroupSetColliding(int _worldID, int _groupID, int _layerA, int _layerB, int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldGroupGetColliding(int _worldID, int _groupID, int _layerA, int _layerB, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldCreateObject(int _worldID, ref Matrix4x4 _transform, ref int _objectID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectExists(int _worldID, int _objectID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldDestroyObject(int _worldID, int _transformID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectSetGroup(int _worldID, int _objectID, int _groupID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectGetGroup(int _worldID, int _objectID, ref int _groupID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectAttachTruss(int _worldID, int _objectID, int _trussID, ref int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectDetachTruss(int _worldID, int _objectID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectAttachRigid(int _worldID, int _objectID, int _rigidID, ref int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectDetachRigid(int _worldID, int _objectID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectAttachShape(int _worldID, int _objectID, int _shapeID, ref int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectDetachShape(int _worldID, int _objectID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectAttachSkin(int _worldID, int _objectID, int _skinID, ref int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectDetachSkin(int _worldID, int _objectID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectAttachJoint(int _worldID, int _objectID, int _baseID, int _jointID, ref int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectDetachJoint(int _worldID, int _objectID, int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectAttachMotor(int _worldID, int _objectID, int _baseID, int _motorID, ref int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectDetachMotor(int _worldID, int _objectID, int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectSetTransform(int _worldID, int _objectID, ref Matrix4x4 _transform);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectGetTransform(int _worldID, int _objectID, ref Matrix4x4 _transform);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectGetInterpolatedTransform(int _worldID, int _objectID, ref Matrix4x4 _transform);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectGetLinearVelocity(int _worldID, int _objectID, ref Vector3 _linearVelocity);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectGetAngularVelocity(int _worldID, int _objectID, ref Vector3 _angularVelocity);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectComputeSkinning(int _worldID, int _objectID, ref Vector3 _positions, ref Vector3 _normals, ref Vector4 _tangents, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectSetWorldLayer(int _worldID, int _objectID, int _layer);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectGetWorldLayer(int _worldID, int _objectID, ref int _layer);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectSetGroupLayer(int _worldID, int _objectID, int _layer);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectGetGroupLayer(int _worldID, int _objectID, ref int _layer);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectSetEnabled(int _worldID, int _objectID, int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectGetEnabled(int _worldID, int _objectID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectSetActive(int _worldID, int _objectID, int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectGetActive(int _worldID, int _objectID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectIsActive(int _worldID, int _objectID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectFirstContact(int _worldID, int _objectID, ref int _contactID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldObjectNextContact(int _worldID, int _objectID, int _contactID, ref int _nextID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldContactExists(int _worldID, int _contactID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldContactGetInfo(int _worldID, int _contactID, ref int _empty, ref int _objectA, ref int _objectB, ref Vector3 _avePosition, ref Vector3 _aveNormal, ref float _minDistance, ref Vector3 _totalImpulse, ref Vector3 _relVelocity);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldAdvance(int _worldID);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldInterpolate(int _worldID, float _t);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldStep(int _worldID, float _deltaTime);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldRayCast(int _worldID, ref Vector3 _origin, ref Vector3 _direction, float _distance, int _skipObjectID, ref int _hit, ref int _hitObjectID, ref Vector3 _hitPoint, ref Vector3 _hitNormal, ref int _hitFace);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldSetColliding(int _worldID, int _layerA, int _layerB, int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxWorldGetColliding(int _worldID, int _layerA, int _layerB, ref int _yes);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateTruss(ref int _worldID);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussExists(int _trussID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyTruss(int _trussID);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussSetNodeCount(int _trussID, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussGetNodeCount(int _trussID, ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussSetNodes(int _trussID, ref Vector3 _position, ref float _mass, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussGetNodes(int _trussID, ref Vector3 _position, ref float _mass, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussSetLinkCount(int _trussID, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussGetLinkCount(int _trussID, ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussSetLinks(int _trussID, ref int _nodes, ref float _length, ref float _stiffness, ref float _damping, ref float _elastic, ref float _stretching, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussGetLinks(int _trussID, ref int _nodes, ref float _length, ref float _stiffness, ref float _damping, ref float _elastic, ref float _stretching, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussSetFaceCount(int _trussID, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussGetFaceCount(int _trussID, ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussSetFaces(int _trussID, ref int _nodes, ref int _matter, ref float _envelop, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussGetFaces(int _trussID, ref int _nodes, ref int _matter, ref float _envelop, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussSetReferenceNodes(int _trussID, ref int _nodes);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceGetNodes(int _worldID, int _instanceID, ref Vector3 _position, ref Vector3 _velocity, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceSetLinks(int _worldID, int _instanceID, ref float _plastic, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceGetLinks(int _worldID, int _instanceID, ref float _plastic, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceActivate(int _worldID, int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceSetInternalPressure(int _worldID, int _instanceID, float _pressure);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceGetInternalPressure(int _worldID, int _instanceID, ref float _pressure);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceSetAdiabaticIndex(int _worldID, int _instanceID, float _adiabat);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceGetAdiabaticIndex(int _worldID, int _instanceID, ref float _adiabat);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceSetDeactivationSpeed(int _worldID, int _instanceID, float _speed);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceGetDeactivationSpeed(int _worldID, int _instanceID, ref float _speed);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceSetDeactivationTime(int _worldID, int _instanceID, float _time);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceGetDeactivationTime(int _worldID, int _instanceID, ref float _time);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceApplyImpulse(int _worldID, int _instanceID, ref Vector3 _impulse);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceApplyNodeImpulse(int _worldID, int _instanceID, ref Vector3 _impulse, int _node);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceApplyFaceImpulse(int _worldID, int _instanceID, ref Vector3 _impulse, ref Vector3 _point, int _face);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceSetMassScale(int _worldID, int _instanceID, float _massScale);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceGetMassScale(int _worldID, int _instanceID, ref float _massScale);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceAdjustMass(int _worldID, int _instanceID, float _mass);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceSetFastRotation(int _worldID, int _instanceID, int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxTrussInstanceGetFastRotation(int _worldID, int _instanceID, ref int _yes);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateRigid(ref int _rigidID);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidExists(int _rigidID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyRigid(int _rigidID);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidSetType(int _rigidID, int _type);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidGetType(int _rigidID, ref int _type);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidSetMass(int _rigidID, float _mass, ref float _com, ref float _inertia);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidGetMass(int _rigidID, ref float _mass, ref float _com, ref float _inertia);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceActivate(int _worldID, int _instanceID);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceSetLocation(int _worldID, int _instanceID, ref Vector3 _position, ref Quaternion _rotation);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceGetLocation(int _worldID, int _instanceID, ref Vector3 _position, ref Quaternion _rotation);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceSetVelocity(int _worldID, int _instanceID, ref Vector3 _linear, ref Vector3 _angular);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceGetVelocity(int _worldID, int _instanceID, ref Vector3 _linear, ref Vector3 _angular);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceExtractForces(int _worldID, int _instanceID, ref Vector3 _force, ref Vector3 _torque);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceSetDeactivationSpeed(int _worldID, int _instanceID, float _speed);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceGetDeactivationSpeed(int _worldID, int _instanceID, ref float _speed);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceSetDeactivationTime(int _worldID, int _instanceID, float _time);
    [DllImport(TRUSS_DLL)]
    static extern int TxRigidInstanceGetDeactivationTime(int _worldID, int _instanceID, ref float _time);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateSkin(ref int _skinID);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinExists(int _skinID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroySkin(int _skinID);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinSetBoneCount(int _skinID, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinGetBoneCount(int _skinID, ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinSetBones(int _skinID, ref int _face, ref float _width, ref float _height, ref float _skew, ref Matrix4x4 _binding, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinGetBones(int _skinID, ref int _face, ref float _width, ref float _height, ref float _skew, ref Matrix4x4 _binding, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinSetVertexCount(int _skinID, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinGetVertexCount(int _skinID, ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinSetVertices(int _skinID, ref int _bones, ref float _weights, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinGetVertices(int _skinID, ref int _bones, ref float _weights, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxSkinSetup(int _skinID, int _trussID, int _meshID, int _build);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateMatter(ref int _matterID);
    [DllImport(TRUSS_DLL)]
    static extern int TxMatterExists(int _matterID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyMatter(int _matterID);
    [DllImport(TRUSS_DLL)]
    static extern int TxMatterSetStaticFriction(int _matterID, float _friction);
    [DllImport(TRUSS_DLL)]
    static extern int TxMatterGetStaticFriction(int _matterID, ref float _friction);
    [DllImport(TRUSS_DLL)]
    static extern int TxMatterSetSlidingFriction(int _matterID, float _friction);
    [DllImport(TRUSS_DLL)]
    static extern int TxMatterGetSlidingFriction(int _matterID, ref float _friction);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateShape(ref int _shapeID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeExists(int _shapeID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyShape(int _shapeID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeGetType(int _shapeID, ref int _type);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeSetTruss(int _shapeID, int _trussID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeGetTruss(int _shapeID, ref int _trussID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeSetMesh(int _shapeID, int _meshID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeGetMesh(int _shapeID, ref int _meshID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeSetTerrain(int _shapeID, int _terrainID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeGetTerrain(int _shapeID, ref int _terrainID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeSetConvex(int _shapeID, int _convexID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeGetConvex(int _shapeID, ref int _convexID);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeSetMargin(int _shapeID, float _margin);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeGetMargin(int _shapeID, ref float _margin);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeSetMatterCount(int _shapeID, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeGetMatterCount(int _shapeID, ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeSetMatters(int _shapeID, ref int _matter, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxShapeGetMatters(int _shapeID, ref int _matter, int _offset, int _count);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateJoint(ref int _jointID);
    [DllImport(TRUSS_DLL)]
    static extern int TxJointExists(int _jointID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyJoint(int _jointID);
    [DllImport(TRUSS_DLL)]
    static extern int TxJointSetFlags(int _jointID, int _flags);
    [DllImport(TRUSS_DLL)]
    static extern int TxJointGetFlags(int _jointID, ref int _flags);
    [DllImport(TRUSS_DLL)]
    static extern int TxJointSnapToPoint(int _jointID, int _node, float _minLimit, float _maxLimit, float _strength, int _flags, ref Vector3 _position);
    [DllImport(TRUSS_DLL)]
    static extern int TxJointSnapToNode(int _jointID, int _node, float _minLimit, float _maxLimit, float _strength, int _flags, int _nodeB);
    [DllImport(TRUSS_DLL)]
    static extern int TxJointSnapToEdge(int _jointID, int _node, float _minLimit, float _maxLimit, float _strength, int _flags, int _nodeB0, int _nodeB1);
    [DllImport(TRUSS_DLL)]
    static extern int TxJointInstanceIsBroken(int _worldID, int _instanceID, ref int yes);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateMotor(ref int _motorID);
    [DllImport(TRUSS_DLL)]
    static extern int TxMotorExists(int _motorID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyMotor(int _motorID);
    [DllImport(TRUSS_DLL)]
    static extern int TxMotorSetAxis(int _motorID, int _node0, int _node1);
    [DllImport(TRUSS_DLL)]
    static extern int TxMotorGetAxis(int _motorID, ref int _node0, ref int _node1);
    [DllImport(TRUSS_DLL)]
    static extern int TxMotorInstanceSetRate(int _worldID, int _instanceID, float _rate);
    [DllImport(TRUSS_DLL)]
    static extern int TxMotorInstanceGetRate(int _worldID, int _instanceID, ref float _rate);
    [DllImport(TRUSS_DLL)]
    static extern int TxMotorInstanceSetTorque(int _worldID, int _instanceID, float _torque);
    [DllImport(TRUSS_DLL)]
    static extern int TxMotorInstanceGetTorque(int _worldID, int _instanceID, ref float _torque);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateMesh(ref int _meshID);
    [DllImport(TRUSS_DLL)]
    static extern int TxMeshExists(int _meshID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyMesh(int _meshID);
    [DllImport(TRUSS_DLL)]
    static extern int TxMeshSetVertexCount(int _meshID, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxMeshGetVertexCount(int _meshID, ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxMeshSetVertices(int _meshID, ref Vector3 _positions, ref Vector3 _normals, ref Vector4 _tangents, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxMeshGetVertices(int _meshID, ref Vector3 _positions, ref Vector3 _normals, ref Vector4 _tangents, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxMeshSetFaceCount(int _meshID, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxMeshGetFaceCount(int _meshID, ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxMeshSetFaces(int _meshID, ref int _vertices, ref int _matter, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxMeshGetFaces(int _meshID, ref int _vertices, ref int _matter, int _offset, int _count);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateTerrain(ref int _terrainID);
    [DllImport(TRUSS_DLL)]
    static extern int TxTerrainExists(int _terrainID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxTerrainSetHeightmapSize(int _terrainID, int _sizeX, int _sizeY);
    [DllImport(TRUSS_DLL)]
    static extern int TxTerrainGetHeightmapSize(int _terrainID, ref int _sizeX, ref int _sizeY);
    [DllImport(TRUSS_DLL)]
    static extern int TxTerrainSetHeightmap(int _terrainID, ref float _heightmap, int _offsetX, int _offsetY, int _sizeX, int _sizeY);
    [DllImport(TRUSS_DLL)]
    static extern int TxTerrainGetHeightmap(int _terrainID, ref float _heightmap, int _offsetX, int _offsetY, int _sizeX, int _sizeY);
    [DllImport(TRUSS_DLL)]
    static extern int TxTerrainSetExtents(int _terrainID, ref Vector3 _extents);
    [DllImport(TRUSS_DLL)]
    static extern int TxTerrainGetExtents(int _terrainID, ref Vector3 _extents);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyTerrain(int _terrainID);

    [DllImport(TRUSS_DLL)]
    static extern int TxCreateConvex(ref int _convexID);
    [DllImport(TRUSS_DLL)]
    static extern int TxConvexExists(int _convexID, ref int _yes);
    [DllImport(TRUSS_DLL)]
    static extern int TxDestroyConvex(int _convexID);
    [DllImport(TRUSS_DLL)]
    static extern int TxConvexSetVertexCount(int _convexID, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxConvexGetVertexCount(int _convexID, ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxConvexSetVertices(int _convexID, ref Vector3 _position, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxConvexGetVertices(int _convexID, ref Vector3 _position, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxConvexSetMargin(int _convexID, float _margin);
    [DllImport(TRUSS_DLL)]
    static extern int TxConvexGetMargin(int _convexID, ref float _margin);

    [DllImport(TRUSS_DLL)]
    static extern int TxThreadsStartWorkers(int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxThreadsStopWorkers();

    [DllImport(TRUSS_DLL)]
    static extern int TxDebugGetLinesCount(ref int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxDebugGetLines(ref Vector3 _start, ref Vector3 _end, ref Color _color, int _offset, int _count);
    [DllImport(TRUSS_DLL)]
    static extern int TxDebugClear();
    [DllImport(TRUSS_DLL)]
    static extern int TxClear();
    [DllImport(TRUSS_DLL)]
    static extern int TxComposeMatrix(ref Vector3 _translation, ref Quaternion _rotation, ref Vector3 _scale, ref Matrix4x4 _matrix);
    [DllImport(TRUSS_DLL)]
    static extern int TxDecomposeMatrix(ref Matrix4x4 _matrix, ref Vector3 _translation, ref Quaternion _rotation, ref Vector3 _scale);
    [DllImport(TRUSS_DLL)]
    static extern IntPtr TxErrorGetDescription(int _error);

#endregion

#region Public Methods

    public static int CreateWorld()
    {
        int worldID = -1;
        CheckError(TxCreateWorld(ref worldID));
        return worldID;
    }
    public static bool WorldExists(int _worldID)
    {
        int yes = 0;
        CheckError(TxWorldExists(_worldID, ref yes));
        return yes != 0;
    }
    public static int WorldCreateObject(int _worldID, Matrix4x4 _transform)
    {
        int objectID = -1;
        CheckError(TxWorldCreateObject(_worldID, ref _transform, ref objectID));
        return objectID;
    }
    public static bool WorldObjectExists(int _worldID, int _objectID)
    {
        int yes = 0;
        CheckError(TxWorldObjectExists(_worldID, _objectID, ref yes));
        return yes != 0;
    }
    public static void WorldDestroyObject(int _worldID, int _objectID)
    {
        CheckError(TxWorldDestroyObject(_worldID, _objectID));
    }
    public static void WorldObjectSetGroup(int _worldID, int _objectID, int _groupID)
    {
        CheckError(TxWorldObjectSetGroup(_worldID, _objectID, _groupID));
    }
    public static int WorldObjectGetGroup(int _worldID, int _objectID)
    {
        int groupID = -1;
        CheckError(TxWorldObjectGetGroup(_worldID, _objectID, ref groupID));
        return groupID;
    }
    public static void WorldObjectSetTransform(int _worldID, int _objectID, Matrix4x4 _transform)
    {
        CheckError(TxWorldObjectSetTransform(_worldID, _objectID, ref _transform));
    }
    public static Matrix4x4 WorldObjectGetTransform(int _worldID, int _objectID)
    {
        Matrix4x4 transform = new Matrix4x4();
        CheckError(TxWorldObjectGetTransform(_worldID, _objectID, ref transform));
        return transform;
    }
    public static void WorldObjectGetInterpolatedTransform(int _worldID, int _objectID, ref Matrix4x4 _transform)
    {
        CheckError(TxWorldObjectGetInterpolatedTransform(_worldID, _objectID, ref _transform));
    }
    public static Vector3 WorldObjectGetLinearVelocity(int _worldID, int _objectID)
    {
        Vector3 linearVelocity = Vector3.zero;
        CheckError(TxWorldObjectGetLinearVelocity(_worldID, _objectID, ref linearVelocity));
        return linearVelocity;
    }
    public static Vector3 WorldObjectGetAngularVelocity(int _worldID, int _objectID)
    {
        Vector3 angularVelocity = Vector3.zero;
        CheckError(TxWorldObjectGetAngularVelocity(_worldID, _objectID, ref angularVelocity));
        return angularVelocity;
    }
    public static int WorldObjectAttachTruss(int _worldID, int _objectID, int _trussID)
    {
        int instanceID = -1;
        CheckError(TxWorldObjectAttachTruss(_worldID, _objectID, _trussID, ref instanceID));
        return instanceID;
    }
    public static int WorldObjectAttachRigid(int _worldID, int _objectID, int _rigidID)
    {
        int instanceID = -1;
        CheckError(TxWorldObjectAttachRigid(_worldID, _objectID, _rigidID, ref instanceID));
        return instanceID;
    }
    public static int WorldObjectAttachShape(int _worldID, int _objectID, int _shapeID)
    {
        int instanceID = -1;
        CheckError(TxWorldObjectAttachShape(_worldID, _objectID, _shapeID, ref instanceID));
        return instanceID;
    }
    public static int WorldObjectAttachSkin(int _worldID, int _objectID, int _skinID)
    {
        int instanceID = -1;
        CheckError(TxWorldObjectAttachSkin(_worldID, _objectID, _skinID, ref instanceID));
        return instanceID;
    }
    public static void WorldObjectComputeSkinning(int _worldID, int _objectID, Vector3[] _positions, Vector3[] _normals, Vector4[] _tangents, int _offset, int _count)
    {
        Vector4[] tangents = _tangents.Length == _count ? _tangents : new Vector4[] { Vector4.zero };
        CheckError(TxWorldObjectComputeSkinning(_worldID, _objectID, ref _positions[0], ref _normals[0], ref tangents[0], _offset, _count));
    }

    public static void WorldObjectSetEnabled(int _worldID, int _objectID, bool _yes)
    {
        CheckError(TxWorldObjectSetEnabled(_worldID, _objectID, _yes ? 1 : 0));
    }
    public static bool WorldObjectGetEnabled(int _worldID, int _objectID)
    {
        int yes = 0;
        CheckError(TxWorldObjectGetEnabled(_worldID, _objectID, ref yes));
        return yes != 0;
    }
    public static void WorldObjectSetActive(int _worldID, int _objectID, bool _yes)
    {
        CheckError(TxWorldObjectSetActive(_worldID, _objectID, _yes ? 1 : 0));
    }
    public static bool WorldObjectGetActive(int _worldID, int _objectID)
    {
        int yes = 0;
        CheckError(TxWorldObjectGetActive(_worldID, _objectID, ref yes));
        return yes != 0;
    }
    public static bool WorldObjectIsActive(int _worldID, int _objectID)
    {
        int yes = 0;
        CheckError(TxWorldObjectIsActive(_worldID, _objectID, ref yes));
        return yes != 0;
    }
    public static void WorldObjectSetWorldLayer(int _worldID, int _objectID, int _layer)
    {
        CheckError(TxWorldObjectSetWorldLayer(_worldID, _objectID, _layer));
    }
    public static int WorldObjectGetWorldLayer(int _worldID, int _objectID)
    {
        int layer = 0;
        CheckError(TxWorldObjectGetWorldLayer(_worldID, _objectID, ref layer));
        return layer;
    }
    public static void WorldObjectSetGroupLayer(int _worldID, int _objectID, int _layer)
    {
        CheckError(TxWorldObjectSetGroupLayer(_worldID, _objectID, _layer));
    }
    public static int WorldObjectGetGroupLayer(int _worldID, int _objectID)
    {
        int layer = 0;
        CheckError(TxWorldObjectGetGroupLayer(_worldID, _objectID, ref layer));
        return layer;
    }
    public static int WorldObjectAttachJoint(int _worldID, int _objectID, int _baseID, int _jointID)
    {
        int instanceID = -1;
        CheckError(TxWorldObjectAttachJoint(_worldID, _objectID, _baseID, _jointID, ref instanceID));
        return instanceID;
    }
    public static void WorldObjectDetachJoint(int _worldID, int _objectID, int _instanceID)
    {
        CheckError(TxWorldObjectDetachJoint(_worldID, _objectID, _instanceID));
    }
    public static int WorldObjectAttachMotor(int _worldID, int _objectID, int _baseID, int _motorID)
    {
        int instanceID = -1;
        CheckError(TxWorldObjectAttachMotor(_worldID, _objectID, _baseID, _motorID, ref instanceID));
        return instanceID;
    }
    public static void WorldObjectDetachMotor(int _worldID, int _objectID, int _instanceID)
    {
        CheckError(TxWorldObjectDetachMotor(_worldID, _objectID, _instanceID));
    }
    public static void MotorInstanceSetRate(int _worldID, int _instanceID, float _rate)
    {
        CheckError(TxMotorInstanceSetRate(_worldID, _instanceID, _rate));
    }
    public static float MotorInstanceGetRate(int _worldID, int _instanceID)
    {
        float rate = 0;
        CheckError(TxMotorInstanceGetRate(_worldID, _instanceID, ref rate));
        return rate;
    }
    public static void MotorInstanceSetTorque(int _worldID, int _instanceID, float _torque)
    {
        CheckError(TxMotorInstanceSetTorque(_worldID, _instanceID, _torque));
    }
    public static float MotorInstanceGetTorque(int _worldID, int _instanceID)
    {
        float torque = 0;
        CheckError(TxMotorInstanceGetTorque(_worldID, _instanceID, ref torque));
        return torque;
    }
    public static int WorldObjectFirstContact(int _worldID, int _objectID)
    {
        int contactID = -1;
        CheckError(TxWorldObjectFirstContact(_worldID, _objectID, ref contactID));
        return contactID;
    }
    public static int WorldObjectNextContact(int _worldID, int _objectID, int _contactID)
    {
        int nextID = -1;
        CheckError(TxWorldObjectNextContact(_worldID, _objectID, _contactID, ref nextID));
        return nextID;
    }
    public static bool WorldContactExists(int _worldID, int _contactID)
    {
        int yes = 0;
        CheckError(TxWorldContactExists(_worldID, _contactID, ref yes));
        return yes != 0;
    }
    public static bool WorldContactGetInfo(int _worldID, int _contactID, ref int _objectA, ref int _objectB, ref Vector3 _avePosition, ref Vector3 _aveNormal, ref float _minDistance, ref Vector3 _totalImpulse, ref Vector3 _relVelocity)
    {
        int empty = 0;
        CheckError(TxWorldContactGetInfo(_worldID, _contactID, ref empty, ref _objectA, ref _objectB, ref _avePosition, ref _aveNormal, ref _minDistance, ref _totalImpulse, ref _relVelocity));
        return empty == 0;
    }
    public static void WorldAdvance(int _worldID)
    {
        CheckError(TxWorldAdvance(_worldID));
    }
    public static void WorldInterpolate(int _worldID, float _t)
    {
        CheckError(TxWorldInterpolate(_worldID, _t));
    }
    public static void WorldStep(int _worldID, float _deltaTime)
    {
        CheckError(TxWorldStep(_worldID, _deltaTime));
    }
    public static bool WorldRayCast(int _worldID, Vector3 _origin, Vector3 _direction, float _distance, int _skipObjectID, ref int _hitObjectID, ref Vector3 _hitPoint, ref Vector3 _hitNormal, ref int _hitFace)
    {
        int hit = 0;
        CheckError(TxWorldRayCast(_worldID, ref _origin, ref _direction, _distance, _skipObjectID, ref hit, ref _hitObjectID, ref _hitPoint, ref _hitNormal, ref _hitFace));
        return hit != 0;
    }
    public static void WorldSetColliding(int _worldID, int _layerA, int _layerB, bool _yes)
    {
        CheckError(TxWorldSetColliding(_worldID, _layerA, _layerB, _yes ? 1 : 0));
    }
    public static bool WorldGetColliding(int _worldID, int _layerA, int _layerB)
    {
        int yes = 0;
        CheckError(TxWorldGetColliding(_worldID, _layerA, _layerB, ref yes));
        return yes != 0;
    }

    public static void DestroyWorld(int _worldID)
    {
        CheckError(TxDestroyWorld(_worldID));
    }
    public static void WorldSetSimulationStep(int _worldID, float _step)
    {
        CheckError(TxWorldSetSimulationStep(_worldID, _step));
    }
    public static float WorldGetSimulationStep(int _worldID)
    {
        float step = 0;
        CheckError(TxWorldGetSimulationStep(_worldID, ref step));
        return step;
    }
    public static void WorldSetSimulationSubstepPower(int _worldID, int _power)
    {
        CheckError(TxWorldSetSimulationSubstepPower(_worldID, _power));
    }
    public static int TxWorldGetSimulationSubstepPower(int _worldID)
    {
        int power = 0;
        CheckError(TxWorldGetSimulationSubstepPower(_worldID, ref power));
        return power;
    }
    public static void WorldSetSolverIterations(int _worldID, int _iterations)
    {
        CheckError(TxWorldSetSolverIterations(_worldID, _iterations));
    }
    public static int WorldGetSolverIterations(int _worldID, ref int _iterations)
    {
        int iterations = 0;
        CheckError(TxWorldGetSolverIterations(_worldID, ref iterations));
        return iterations;
    }
    public static void WorldSetGlobalGravity(int _worldID, Vector3 _gravity)
    {
        CheckError(TxWorldSetGlobalGravity (_worldID, ref _gravity));
    }
    public static Vector3 WorldGetGlobalGravity(int _worldID)
    {
        Vector3 gravity = Vector3.zero;
        CheckError(TxWorldGetGlobalGravity(_worldID, ref gravity));
        return gravity;
    }
    public static void WorldSetGlobalPressure(int _worldID, float _pressure)
    {
        CheckError(TxWorldSetGlobalPressure(_worldID, _pressure));
    }
    public static float WorldGetGlobalPressure(int _worldID)
    {
        float pressure = 0;
        CheckError(TxWorldGetGlobalPressure(_worldID, ref pressure));
        return pressure;
    }
    public static int WorldCreateGroup(int _worldID)
    {
        int groupID = -1;
        CheckError(TxWorldCreateGroup(_worldID, ref groupID));
        return groupID;
    }
    public static bool WorldGroupExists(int _worldID, int _groupID)
    {
        int yes = 0;
        CheckError(TxWorldGroupExists(_worldID, _groupID, ref yes));
        return yes != 0;
    }
    public static void WorldDestroyGroup(int _worldID, int _groupID)
    {
        CheckError(TxWorldDestroyGroup(_worldID, _groupID));
    }
    public static void WorldGroupSetColliding(int _worldID, int _groupID, int _layerA, int _layerB, bool _yes)
    {
        CheckError(TxWorldGroupSetColliding(_worldID, _groupID, _layerA, _layerB, _yes ? 1 : 0));
    }
    public static bool TxWorldGroupGetColliding(int _worldID, int _groupID, int _layerA, int _layerB)
    {
        int yes = 0;
        CheckError(TxWorldGroupGetColliding(_worldID, _groupID, _layerA, _layerB, ref yes));
        return yes != 0;
    }
    public static int CreateTruss()
    {
        int trussID = -1;
        CheckError(TxCreateTruss(ref trussID));
        return trussID;
    }
    public static bool TrussExists(int _trussID)
    {
        int yes = 0;
        CheckError(TxTrussExists(_trussID, ref yes));
        return yes != 0;
    }
    public static void TrussSetNodeCount(int _trussID, int _count)
    {
        CheckError(TxTrussSetNodeCount(_trussID, _count));
    }
    public static int TrussGetNodeCount(int _trussID)
    {
        int count = -1;
        CheckError(TxTrussGetNodeCount(_trussID, ref count));
        return count;
    }
    public static void TrussSetNodes(int _trussID, Vector3[] _position, float[] _mass, int _offset, int _count)
    {
        CheckError(TxTrussSetNodes(_trussID, ref _position[0], ref _mass[0], _offset, _count)); // @@@
    }
    public static void TrussGetNodes(int _trussID, Vector3[] _position, float[] _mass, int _offset, int _count)
    {
        CheckError(TxTrussGetNodes(_trussID, ref _position[0], ref _mass[0], _offset, _count)); // @@@
    }
    public static void TrussSetLinkCount(int _trussID, int _count)
    {
        CheckError(TxTrussSetLinkCount(_trussID, _count));
    }
    public static int TrussGetLinkCount(int _trussID)
    {
        int count = -1;
        CheckError(TxTrussGetLinkCount(_trussID, ref count));
        return count;
    }
    public static void TrussSetLinks(int _trussID, int[] _nodes, float[] _length, float[] _stiffness, float[] _damping, float[] _elastic, float[] _stretching, int _offset, int _count)
    {
        CheckError(TxTrussSetLinks(_trussID, ref _nodes[0], ref _length[0], ref _stiffness[0], ref _damping[0], ref _elastic[0], ref _stretching[0], _offset, _count)); // @@@
    }
    public static void TrussGetLinks(int _trussID, int[] _nodes, float[] _length, float[] _stiffness, float[] _damping, float[] _elastic, float[] _stretching, int _offset, int _count)
    {
        CheckError(TxTrussGetLinks(_trussID, ref _nodes[0], ref _length[0], ref _stiffness[0], ref _damping[0], ref _elastic[0], ref _stretching[0], _offset, _count)); // @@@
    }
    public static void TrussSetFaceCount(int _trussID, int _count)
    {
        CheckError(TxTrussSetFaceCount(_trussID, _count));
    }
    public static int TrussGetFaceCount(int _trussID)
    {
        int count = -1;
        CheckError(TxTrussGetFaceCount(_trussID, ref count));
        return count;
    }
    public static void TrussSetFaces(int _trussID, int[] _nodes, int[] _matter, float[] _envelop, int _offset, int _count)
    {
        CheckError(TxTrussSetFaces(_trussID, ref _nodes[0], ref _matter[0], ref _envelop[0], _offset, _count)); // @@@
    }
    public static void TrussGetFaces(int _trussID, int[] _nodes, int[] _matter, float[] _envelop, int _offset, int _count)
    {
        CheckError(TxTrussGetFaces(_trussID, ref _nodes[0], ref _matter[0], ref _envelop[0], _offset, _count)); // @@@
    }
    public static void TrussSetReferenceNodes(int _trussID, int[] _nodes)
    {
        CheckError(TxTrussSetReferenceNodes(_trussID, ref _nodes[0]));
    }
    public static void DestroyTruss(int _trussID)
    {
        CheckError(TxDestroyTruss(_trussID));
    }
    public static void TrussInstanceGetNodes(int _worldID, int _instanceID, Vector3[] _position, Vector3[] _velocity, int _offset)
    {
        CheckError(TxTrussInstanceGetNodes(_worldID, _instanceID, ref _position[0], ref _velocity[0], _offset, _position.Length));
    }
    public static void TrussInstanceSetLinks(int _worldID, int _instanceID, float[] _plastic, int _offset)
    {
        CheckError(TxTrussInstanceSetLinks(_worldID, _instanceID, ref _plastic[0], _offset, _plastic.Length));
    }
    public static void TrussInstanceGetLinks(int _worldID, int _instanceID, float[] _plastic, int _offset)
    {
        CheckError(TxTrussInstanceGetLinks(_worldID, _instanceID, ref _plastic[0], _offset, _plastic.Length));
    }
    public static void TrussInstanceActivate(int _worldID, int _instanceID)
    {
        CheckError(TxTrussInstanceActivate(_worldID, _instanceID));
    }

    public static void TrussInstanceSetInternalPressure(int _worldID, int _instanceID, float _pressure)
    {
        CheckError(TxTrussInstanceSetInternalPressure(_worldID, _instanceID, _pressure));
    }
    public static float TrussInstanceGetInternalPressure(int _worldID, int _instanceID)
    {
        float pressure = 0;
        CheckError(TxTrussInstanceGetInternalPressure(_worldID, _instanceID, ref pressure));
        return pressure;
    }
    public static void TrussInstanceSetAdiabaticIndex(int _worldID, int _instanceID, float _adiabat)
    {
        CheckError(TxTrussInstanceSetAdiabaticIndex(_worldID, _instanceID, _adiabat));
    }
    public static float TrussInstanceGetAdiabaticIndex(int _worldID, int _instanceID)
    {
        float adiabat = 0;
        CheckError(TxTrussInstanceGetAdiabaticIndex(_worldID, _instanceID, ref adiabat));
        return adiabat;
    }
    public static void TrussInstanceSetDeactivationSpeed(int _worldID, int _instanceID, float _speed)
    {
        CheckError(TxTrussInstanceSetDeactivationSpeed(_worldID, _instanceID, _speed));
    }
    public static float TrussInstanceGetDeactivationSpeed(int _worldID, int _instanceID)
    {
        float speed = 0;
        CheckError(TxTrussInstanceGetDeactivationSpeed(_worldID, _instanceID, ref speed));
        return speed;
    }
    public static void TrussInstanceSetDeactivationTime(int _worldID, int _instanceID, float _time)
    {
        CheckError(TxTrussInstanceSetDeactivationTime(_worldID, _instanceID, _time));
    }
    public static float TrussInstanceGetDeactivationTime(int _worldID, int _instanceID)
    {
        float time = 0;
        CheckError(TxTrussInstanceGetDeactivationTime(_worldID, _instanceID, ref time));
        return time;
    }
    public static void TrussInstanceApplyImpulse(int _worldID, int _instanceID, Vector3 _impulse)
    {
        CheckError(TxTrussInstanceApplyImpulse(_worldID, _instanceID, ref _impulse));
    }
    public static void TrussInstanceApplyImpulse(int _worldID, int _instanceID, Vector3 _impulse, int _node)
    {
        CheckError(TxTrussInstanceApplyNodeImpulse(_worldID, _instanceID, ref _impulse, _node));
    }
    public static void TrussInstanceApplyImpulse(int _worldID, int _instanceID, Vector3 _impulse, Vector3 _point, int _face)
    {
        CheckError(TxTrussInstanceApplyFaceImpulse(_worldID, _instanceID, ref _impulse, ref _point, _face));
    }
    public static void TrussInstanceSetMassScale(int _worldID, int _instanceID, float _massScale)
    {
        CheckError(TxTrussInstanceSetMassScale(_worldID, _instanceID, _massScale));
    }
    public static float TrussInstanceGetMassScale(int _worldID, int _instanceID)
    {
        float massScale = 1.0f;
        CheckError(TxTrussInstanceGetMassScale(_worldID, _instanceID, ref massScale));
        return massScale;
    }
    public static void TrussInstanceAdjustMass(int _worldID, int _instanceID, float _mass)
    {
        CheckError(TxTrussInstanceAdjustMass(_worldID, _instanceID, _mass));
    }
    public static void TrussInstanceSetFastRotation(int _worldID, int _instanceID, bool _yes)
    {
        CheckError(TxTrussInstanceSetFastRotation(_worldID, _instanceID, _yes ? 1 : 0));
    }
    public static bool TrussInstanceGetFastRotation(int _worldID, int _instanceID)
    {
        int yes = 0;
        CheckError(TxTrussInstanceGetFastRotation(_worldID, _instanceID, ref yes));
        return yes != 0;
    }

    public static int CreateRigid()
    {
        int rigidID = -1;
        CheckError(TxCreateRigid(ref rigidID));
        return rigidID;
    }
    public static bool RigidExists(int _rigidID)
    {
        int yes = 0;
        CheckError(TxRigidExists(_rigidID, ref yes));
        return yes != 0;
    }
    public static void DestroyRigid(int _rigidID)
    {
        CheckError(TxDestroyRigid(_rigidID));
    }
    public static void RigidSetType(int _rigidID, int _type)
    {
        CheckError(TxRigidSetType(_rigidID, _type));
    }
    public static int RigidGetType(int _rigidID)
    {
        int type = 0;
        CheckError(TxRigidGetType(_rigidID, ref type));
        return type;
    }
    public static void RigidSetMass(int _rigidID, float _mass, Vector3 _com, Vector3[] _inertia)
    {
        CheckError(TxRigidSetMass(_rigidID, _mass, ref _com.x, ref _inertia[0].x));
    }
    public static void RigidSetMass(int _rigidID, float _mass, Vector3 _com, Vector3 _inertia, Quaternion _rotation)
    {
        Matrix4x4 inertiaMatrix = Matrix4x4.Scale(_inertia);
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, _rotation, Vector3.one);
        inertiaMatrix = rotationMatrix * inertiaMatrix * rotationMatrix.transpose;
        Vector3[] inertia = new Vector3[3] { new Vector3(inertiaMatrix.m00, inertiaMatrix.m01, inertiaMatrix.m02),
                                             new Vector3(inertiaMatrix.m10, inertiaMatrix.m11, inertiaMatrix.m12),
                                             new Vector3(inertiaMatrix.m20, inertiaMatrix.m21, inertiaMatrix.m22) };
        RigidSetMass(_rigidID, _mass, _com, inertia);
    }
    public static void RigidGetMass(int _rigidID, ref float _mass, ref Vector3 _com, Vector3[] _inertia)
    {
        CheckError(TxRigidGetMass(_rigidID, ref _mass, ref _com.x, ref _inertia[0].x));
    }
    public static void RigidInstanceActivate(int _worldID, int _instanceID)
    {
        CheckError(TxRigidInstanceActivate(_worldID, _instanceID));
    }
    public static void RigidInstanceSetLocation(int _worldID, int _instanceID, Vector3 _position, Quaternion _rotation)
    {
        CheckError(TxRigidInstanceSetLocation(_worldID, _instanceID, ref _position, ref _rotation));
    }
    public static void RigidInstanceGetLocation(int _worldID, int _instanceID, ref Vector3 _position, ref Quaternion _rotation)
    {
        CheckError(TxRigidInstanceGetLocation(_worldID, _instanceID, ref _position, ref _rotation));
    }
    public static void RigidInstanceSetVelocity(int _worldID, int _instanceID, Vector3 _linear, Vector3 _angular)
    {
        CheckError(TxRigidInstanceSetVelocity(_worldID, _instanceID, ref _linear, ref _angular));
    }
    public static void RigidInstanceGetVelocity(int _worldID, int _instanceID, ref Vector3 _linear, ref Vector3 _angular)
    {
        CheckError(TxRigidInstanceGetVelocity(_worldID, _instanceID, ref _linear, ref _angular));
    }
    public static Vector3 RigidInstanceGetLinearVelocity(int _worldID, int _instanceID)
    {
        Vector3 linear = Vector3.zero, angular = Vector3.zero;
        RigidInstanceGetVelocity(_worldID, _instanceID, ref linear, ref angular);
        return linear;
    }
    public static Vector3 RigidInstanceGetAngularVelocity(int _worldID, int _instanceID)
    {
        Vector3 linear = Vector3.zero, angular = Vector3.zero;
        RigidInstanceGetVelocity(_worldID, _instanceID, ref linear, ref angular);
        return angular;
    }
    public static void RigidInstanceExtractForces(int _worldID, int _instanceID, ref Vector3 _force, ref Vector3 _torque)
    {
        CheckError(TxRigidInstanceExtractForces(_worldID, _instanceID, ref _force, ref _torque));
    }
    public static void RigidInstanceSetDeactivationSpeed(int _worldID, int _instanceID, float _speed)
    {
        CheckError(TxRigidInstanceSetDeactivationSpeed(_worldID, _instanceID, _speed));
    }
    public static float RigidInstanceGetDeactivationSpeed(int _worldID, int _instanceID)
    {
        float speed = 0;
        CheckError(TxRigidInstanceGetDeactivationSpeed(_worldID, _instanceID, ref speed));
        return speed;
    }
    public static void RigidInstanceSetDeactivationTime(int _worldID, int _instanceID, float _time)
    {
        CheckError(TxRigidInstanceSetDeactivationTime(_worldID, _instanceID, _time));
    }
    public static float RigidInstanceGetDeactivationTime(int _worldID, int _instanceID)
    {
        float time = 0;
        CheckError(TxRigidInstanceGetDeactivationTime(_worldID, _instanceID, ref time));
        return time;
    }

    public static int CreateSkin()
    {
        int skinID = -1;
        CheckError(TxCreateSkin(ref skinID));
        return skinID;
    }
    public static bool SkinExists(int _skinID)
    {
        int yes = 0;
        CheckError(TxSkinExists(_skinID, ref yes));
        return yes != 0;
    }
    public static void SkinSetBoneCount(int _skinID, int _count)
    {
        CheckError(TxSkinSetBoneCount(_skinID, _count));
    }
    public static void SkinGetBoneCount(int _skinID, ref int _count)
    {
        CheckError(TxSkinGetBoneCount(_skinID, ref _count));
    }
    public static void SkinSetBones(int _skinID, ref int _face, ref float _width, ref float _height, ref float _skew, ref Matrix4x4 _binding, int _offset, int _count)
    {
        CheckError(TxSkinSetBones(_skinID, ref _face, ref _width, ref _height, ref _skew, ref _binding, _offset, _count));
    }
    public static void SkinGetBones(int _skinID, ref int _face, ref float _width, ref float _height, ref float _skew, ref Matrix4x4 _binding, int _offset, int _count)
    {
        CheckError(TxSkinGetBones(_skinID, ref _face, ref _width, ref _height, ref _skew, ref _binding, _offset, _count));
    }
    public static void SkinSetVertexCount(int _skinID, int _count)
    {
        CheckError(TxSkinSetVertexCount(_skinID, _count));
    }
    public static void SkinGetVertexCount(int _skinID, ref int _count)
    {
        CheckError(TxSkinGetVertexCount(_skinID, ref _count));
    }
    public static void SkinSetVertices(int _skinID, ref int _bones, ref float _weights, int _offset, int _count)
    {
        CheckError(TxSkinSetVertices(_skinID, ref _bones, ref _weights, _offset, _count));
    }
    public static void SkinGetVertices(int _skinID, ref int _bones, ref float _weights, int _offset, int _count)
    {
        CheckError(TxSkinGetVertices(_skinID, ref _bones, ref _weights, _offset, _count));
    }
    public static void SkinSetup(int _skinID, int _trussID, int _meshID, bool _build)
    {
        CheckError(TxSkinSetup(_skinID, _trussID, _meshID, _build ? 1 : 0));
    }
    public static void DestroySkin(int _skinID)
    {
        CheckError(TxDestroySkin(_skinID));
    }

    public static int CreateMatter()
    {
        int matterID = -1;
        CheckError(TxCreateMatter(ref matterID));
        return matterID;
    }
    public static bool MatterExists(int _matterID)
    {
        int yes = 0;
        CheckError(TxMatterExists(_matterID, ref yes));
        return yes != 0;
    }
    public static void MatterSetStaticFriction(int _matterID, float _friction)
    {
        CheckError(TxMatterSetStaticFriction(_matterID, _friction));
    }
    public static void MatterGetStaticFriction(int _matterID, ref float _friction)
    {
        CheckError(TxMatterGetStaticFriction(_matterID, ref _friction));
    }
    public static void MatterSetSlidingFriction(int _matterID, float _friction)
    {
        CheckError(TxMatterSetSlidingFriction(_matterID, _friction));
    }
    public static void MatterGetSlidingFriction(int _matterID, ref float _friction)
    {
        CheckError(TxMatterGetSlidingFriction(_matterID, ref _friction));
    }
    public static void DestroyMatter(int _matterID)
    {
        CheckError(TxDestroyMatter(_matterID));
    }

    public static int CreateShape()
    {
        int shapeID = -1;
        CheckError(TxCreateShape(ref shapeID));
        return shapeID;
    }
    public static bool ShapeExists(int _shapeID)
    {
        int yes = 0;
        CheckError(TxShapeExists(_shapeID, ref yes));
        return yes != 0;
    }
    public static void ShapeGetType(int _shapeID, ref ShapeType _type)
    {
        int type = 0;
        CheckError(TxShapeGetType(_shapeID, ref type));
        _type = (ShapeType)type;
    }
    public static void ShapeSetTruss(int _shapeID, int _trussID)
    {
        CheckError(TxShapeSetTruss(_shapeID, _trussID));
    }
    public static void ShapeGetTruss(int _shapeID, ref int _trussID)
    {
        CheckError(TxShapeGetTruss(_shapeID, ref _trussID));
    }
    public static void ShapeSetMesh(int _shapeID, int _meshID)
    {
        CheckError(TxShapeSetMesh(_shapeID, _meshID));
    }
    public static void ShapeGetMesh(int _shapeID, ref int _meshID)
    {
        CheckError(TxShapeGetMesh(_shapeID, ref _meshID));
    }
    public static void ShapeSetTerrain(int _shapeID, int _terrainID)
    {
        CheckError(TxShapeSetTerrain(_shapeID, _terrainID));
    }
    public static void ShapeGetTerrain(int _shapeID, ref int _terrainID)
    {
        CheckError(TxShapeGetTerrain(_shapeID, ref _terrainID));
    }
    public static void ShapeSetConvex(int _shapeID, int _convexID)
    {
        CheckError(TxShapeSetConvex(_shapeID, _convexID));
    }
    public static void ShapeGetConvex(int _shapeID, ref int _convexID)
    {
        CheckError(TxShapeGetConvex(_shapeID, ref _convexID));
    }
    public static void ShapeSetMargin(int _shapeID, float _margin)
    {
        CheckError(TxShapeSetMargin(_shapeID, _margin));
    }
    public static float ShapeGetMargin(int _shapeID)
    {
        float margin = 0;
        CheckError(TxShapeGetMargin(_shapeID, ref margin));
        return margin;
    }
    public static void ShapeSetMatterCount(int _shapeID, int _count)
    {
        CheckError(TxShapeSetMatterCount(_shapeID, _count));
    }
    public static void ShapeGetMatterCount(int _shapeID, ref int _count)
    {
        CheckError(TxShapeGetMatterCount(_shapeID, ref _count));
    }
    public static void ShapeSetMatters(int _shapeID, int[] _matter, int _offset, int _count)
    {
        CheckError(TxShapeSetMatters(_shapeID, ref _matter[0], _offset, _count));
    }
    public static void ShapeGetMatters(int _shapeID, int[] _matter, int _offset, int _count)
    {
        CheckError(TxShapeGetMatters(_shapeID, ref _matter[0], _offset, _count));
    }
    public static void DestroyShape(int _shapeID)
    {
        CheckError(TxDestroyShape(_shapeID));
    }
    public static int CreateJoint()
    {
        int jointID = -1;
        CheckError(TxCreateJoint(ref jointID));
        return jointID;
    }
    public static bool JointExists(int _jointID)
    {
        int yes = 0;
        CheckError(TxJointExists(_jointID, ref yes));
        return yes != 0;
    }
    public static void JointSetFlags(int _jointID, int _flags)
    {
        CheckError(TxJointSetFlags(_jointID, _flags));
    }
    public static void JointGetFlags(int _jointID, ref int _flags)
    {
        CheckError(TxJointGetFlags(_jointID, ref _flags));
    }
    public static void JointSnapToPoint(int _jointID, int _node, float _minLimit, float _maxLimit, float _strength, int _flags, Vector3 _position)
    {
        CheckError(TxJointSnapToPoint(_jointID, _node, _minLimit, _maxLimit, _strength, _flags, ref _position));
    }
    public static void JointSnapToNode(int _jointID, int _node, float _minLimit, float _maxLimit, float _strength, int _flags, int _nodeB)
    {
        CheckError(TxJointSnapToNode(_jointID, _node, _minLimit, _maxLimit, _strength, _flags, _nodeB));
    }
    public static void JointSnapToEdge(int _jointID, int _node, float _minLimit, float _maxLimit, float _strength, int _flags, int _nodeB0, int _nodeB1)
    {
        CheckError(TxJointSnapToEdge(_jointID, _node, _minLimit, _maxLimit, _strength, _flags, _nodeB0, _nodeB1));
    }
    public static bool JointInstanceIsBroken(int _worldID, int _instanceID)
    {
        int yes = 0;
        CheckError(TxJointInstanceIsBroken(_worldID, _instanceID, ref yes));
        return yes != 0;
    }
    public static void DestroyJoint(int _jointID)
    {
        CheckError(TxDestroyJoint(_jointID));
    }
    public static int CreateMotor()
    {
        int motorID = -1;
        CheckError(TxCreateMotor(ref motorID));
        return motorID;
    }
    public static bool MotorExists(int _motorID)
    {
        int yes = 0;
        CheckError(TxMotorExists(_motorID, ref yes));
        return yes != 0;
    }
    public static void MotorSetAxis(int _motorID, int _node0, int _node1)
    {
        CheckError(TxMotorSetAxis(_motorID, _node0, _node1));
    }
    public static void MotorGetAxis(int _motorID, ref int _node0, ref int _node1)
    {
        CheckError(TxMotorGetAxis(_motorID, ref _node0, ref _node1));
    }
    public static void DestroyMotor(int _motorID)
    {
        CheckError(TxDestroyMotor(_motorID));
    }
    public static int CreateMesh()
    {
        int meshID = -1;
        CheckError(TxCreateMesh(ref meshID));
        return meshID;
    }
    public static bool MeshExists(int _meshID)
    {
        int yes = 0;
        CheckError(TxMeshExists(_meshID, ref yes));
        return yes != 0;
    }
    static void MeshSetVertexCount(int _meshID, int _count)
    {
        CheckError(TxMeshSetVertexCount(_meshID, _count));
    }
    static void MeshGetVertexCount(int _meshID, ref int _count)
    {
        CheckError(TxMeshGetVertexCount(_meshID, ref _count));
    }
    static void MeshSetVertices(int _meshID, Vector3[] _positions, Vector3[] _normals, Vector4[] _tangents, int _offset, int _count)
    {
        Vector4[] tangents = _tangents.Length == _count ? _tangents : new Vector4[] { Vector4.zero };
        CheckError(TxMeshSetVertices(_meshID, ref _positions[0], ref _normals[0], ref tangents[0], _offset, _count));
    }
    static void MeshGetVertices(int _meshID, Vector3[] _positions, Vector3[] _normals, Vector4[] _tangents, int _offset, int _count)
    {
        Vector4[] tangents = _tangents.Length == _count ? _tangents : new Vector4[] { Vector4.zero };
        CheckError(TxMeshGetVertices(_meshID, ref _positions[0], ref _normals[0], ref tangents[0], _offset, _count));
    }
    static void MeshSetFaceCount(int _meshID, int _count)
    {
        CheckError(TxMeshSetFaceCount(_meshID, _count));
    }
    static void MeshGetIndexCount(int _meshID, ref int _count)
    {
        CheckError(TxMeshGetFaceCount(_meshID, ref _count));
    }
    static void MeshSetFaces(int _meshID, int[] _vertices, int[] _matter, int _offset, int _count)
    {
        CheckError(TxMeshSetFaces(_meshID, ref _vertices[0], ref _matter[0], _offset, _count));
    }
    static void MeshGetFaces(int _meshID, int[] _vertices, int[] _matter, int _offset, int _count)
    {
        CheckError(TxMeshGetFaces(_meshID, ref _vertices[0], ref _matter[0], _offset, _count));
    }
    public static void DestroyMesh(int _meshID)
    {
        CheckError(TxDestroyMesh(_meshID));
    }
    public static int CreateTerrain()
    {
        int terrainID = -1;
        CheckError(TxCreateTerrain(ref terrainID));
        return terrainID;
    }
    public static bool TerrainExists(int _terrainID)
    {
        int yes = 0;
        CheckError(TxTerrainExists(_terrainID, ref yes));
        return yes != 0;
    }
    public static void TerrainSetHeightmapSize(int _terrainID, int _sizeX, int _sizeY)
    {
        CheckError(TxTerrainSetHeightmapSize(_terrainID, _sizeX, _sizeY));
    }
    public static void TerrainGetHeightmapSize(int _terrainID, ref int _sizeX, ref int _sizeY)
    {
        CheckError(TxTerrainGetHeightmapSize(_terrainID, ref _sizeX, ref _sizeY));
    }
    public static void TerrainSetHeightmap(int _terrainID, float[] _heightmap, int _offsetX, int _offsetY, int _sizeX, int _sizeY)
    {
        CheckError(TxTerrainSetHeightmap(_terrainID, ref _heightmap[0], _offsetX, _offsetY, _sizeX, _sizeY));
    }
    public static void TerrainGetHeightmap(int _terrainID, float[] _heightmap, int _offsetX, int _offsetY, int _sizeX, int _sizeY)
    {
        CheckError(TxTerrainGetHeightmap(_terrainID, ref _heightmap[0], _offsetX, _offsetY, _sizeX, _sizeY));
    }
    public static void TerrainSetExtents(int _terrainID, Vector3 _extents)
    {
        CheckError(TxTerrainSetExtents(_terrainID, ref _extents));
    }
    public static void TerrainGetExtents(int _terrainID, ref Vector3 _extents)
    {
        CheckError(TxTerrainGetExtents(_terrainID, ref _extents));
    }
    public static void DestroyTerrain(int _terrainID)
    {
        CheckError(TxDestroyTerrain(_terrainID));
    }
    public static int CreateConvex()
    {
        int convexID = -1;
        CheckError(TxCreateConvex(ref convexID));
        return convexID;
    }
    public static bool ConvexExists(int _convexID)
    {
        int yes = 0;
        CheckError(TxConvexExists(_convexID, ref yes));
        return yes != 0;
    }
    public static void DestroyConvex(int _convexID)
    {
        CheckError(TxDestroyConvex(_convexID));
    }
    public static void ConvexSetVertexCount(int _convexID, int _count)
    {
        CheckError(TxConvexSetVertexCount(_convexID, _count));
    }
    public static int ConvexGetVertexCount(int _convexID)
    {
        int count = 0;
        CheckError(TxConvexExists(_convexID, ref count));
        return count;
    }
    public static void ConvexSetVertices(int _convexID, Vector3[] _position, int _offset, int _count)
    {
        CheckError(TxConvexSetVertices(_convexID, ref _position[0], _offset, _count));
    }
    public static void ConvexGetVertices(int _convexID, Vector3[] _position, int _offset, int _count)
    {
        CheckError(TxConvexGetVertices(_convexID, ref _position[0], _offset, _count));
    }
    public static void ConvexSetMargin(int _convexID, float _margin)
    {
        CheckError(TxConvexSetMargin(_convexID, _margin));
    }
    public static float ConvexGetMargin(int _convexID)
    {
        float margin = 0;
        CheckError(TxConvexGetMargin(_convexID, ref margin));
        return margin;
    }
    public static void ThreadsStartWorkers(int _count)
    {
        CheckError(TxThreadsStartWorkers(_count));
    }
    public static void ThreadsStopWorkers()
    {
        CheckError(TxThreadsStopWorkers());
    }
    public static int DebugGetLinesCount()
    {
        int count = -1;
        CheckError(TxDebugGetLinesCount(ref count));
        return count;
    }
    public static void DebugGetLines(Vector3[] _start, Vector3[] _end, Color[] _color, int _offset, int _count)
    {
        CheckError(TxDebugGetLines(ref _start[0], ref _end[0], ref _color[0], _offset, _count)); // @@@
    }
    public static void DebugClear()
    {
        CheckError(TxDebugClear());
    }
    public static void Clear()
    {
        CheckError(TxClear());
    }
    public static void ComposeMatrix(Vector3 _translation, Quaternion _rotation, Vector3 _scale, ref Matrix4x4 _matrix)
    {
        CheckError(TxComposeMatrix(ref _translation, ref _rotation, ref _scale, ref _matrix));
    }
    public static void DecomposeMatrix(Matrix4x4 _matrix, ref Vector3 _translation, ref Quaternion _rotation, ref Vector3 _scale)
    {
        CheckError(TxDecomposeMatrix(ref _matrix, ref _translation, ref _rotation, ref _scale));
    }

#endregion

#region More Convenient Methods

    public static void WorldObjectSetTransform(int _worldID, int _transformID, Transform _transform)
    {
        Matrix4x4 transform = Matrix4x4.identity;
        ComposeMatrix(_transform.position, _transform.rotation, _transform.localScale, ref transform);
        WorldObjectSetTransform(_worldID, _transformID, transform);
    }

    public static void WorldObjectGetTransform(int _worldID, int _objectID, Transform _transform)
    {
        Matrix4x4 transform = WorldObjectGetTransform(_worldID, _objectID);
        Vector3 translation = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = Vector3.one;
        DecomposeMatrix(transform, ref translation, ref rotation, ref scale);
        _transform.position = translation;
        _transform.rotation = rotation;
        _transform.localScale = scale;
    }

    public static void WorldObjectGetInterpolatedTransform(int _worldID, int _objectID, Transform _transform)
    {
        Matrix4x4 transform = Matrix4x4.identity;
        WorldObjectGetInterpolatedTransform(_worldID, _objectID, ref transform);
        Vector3 translation = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = Vector3.one;
        DecomposeMatrix(transform, ref translation, ref rotation, ref scale);
        _transform.position = translation;
        _transform.rotation = rotation;
        _transform.localScale = scale;
    }

    public static float TrussGetLinkLength(int _trussID, int _linkIndex)
    {
        int[] nodes = new int[2];
        float[] length = new float[1], stiffness = new float[1], damping = new float[1], elastic = new float[1], stretching = new float[1];
        TrussGetLinks(_trussID, nodes, length, stiffness, damping, elastic, stretching, _linkIndex, 1);
        return length[0];
    }

    public static Vector3 TrussInstanceGetNodePosition(int _worldID, int _instanceID, int _nodeIndex)
    {
        Vector3[] position = new Vector3[1];
        Vector3[] velocity = new Vector3[1];
        TrussInstanceGetNodes(_worldID, _instanceID, position, velocity, _nodeIndex);
        return position[0];
    }
    public static Vector3 TrussInstanceGetNodeVelocity(int _worldID, int _instanceID, int _nodeIndex)
    {
        Vector3[] position = new Vector3[1];
        Vector3[] velocity = new Vector3[1];
        TrussInstanceGetNodes(_worldID, _instanceID, position, velocity, _nodeIndex);
        return velocity[0];
    }
    public static void TrussInstanceSetLinkPlastic(int _worldID, int _instanceID, int _linkIndex, float _plastic)
    {
        float[] plastic = { _plastic };
        TrussInstanceSetLinks(_worldID, _instanceID, plastic, _linkIndex);
    }
    public static float TrussInstanceGetLinkPlastic(int _worldID, int _instanceID, int _linkIndex)
    {
        float[] plastic = new float[1];
        TrussInstanceGetLinks(_worldID, _instanceID, plastic, _linkIndex);
        return plastic[0];
    }

    public static ShapeType ShapeGetType(int _shapeID)
    {
        ShapeType type = ShapeType.NONE;
        ShapeGetType(_shapeID, ref type);
        return type;
    }

    public static void MeshSetVertices(int _meshID, Vector3[] _positions, Vector3[] _normals, Vector4[] _tangents)
    {
        MeshSetVertexCount(_meshID, _positions.Length);
        MeshSetVertices(_meshID, _positions, _normals, _tangents, 0, _positions.Length);
    }
    public static void MeshSetFaces(int _meshID, int[] _vertices, int[] _matter)
    {
        MeshSetFaceCount(_meshID, _matter.Length);
        MeshSetFaces(_meshID, _vertices, _matter, 0, _matter.Length);
    }

    public static int CreateTruss(TxTruss _truss)
    {
        TxTruss data = _truss;
        int hashCode = (data != null ? data.GetHashCode() : -1);
        SharedResource resource = sm_sharedTrusses.Find(x => x.hashCode == hashCode);
        if (resource == null)
        {
            resource = new SharedResource();
            resource.hashCode = hashCode;
            resource.ID = CreateTruss();
            if (data != null && data.nodeCount > 0 && data.linkCount > 0 && data.faceCount > 0)
            {
                TrussSetNodeCount(resource.ID, data.nodeCount);
                TrussSetNodes(resource.ID, data.nodePosition, data.nodeMass, 0, data.nodeCount);
                TrussSetLinkCount(resource.ID, data.linkCount);
                TrussSetLinks(resource.ID, data.linkNodes, data.linkLength, data.linkStiffness, data.linkDamping, data.linkElastic, data.linkStretching, 0, data.linkCount);
                TrussSetFaceCount(resource.ID, data.faceCount);
                TrussSetFaces(resource.ID, data.faceNodes, data.faceMatter, data.faceEnvelope, 0, data.faceCount);
                int[] refNodes = data.FindNodeSet("Reference");
                if (refNodes != null)
                {
                    if (refNodes.Length == 3) TrussSetReferenceNodes(resource.ID, refNodes);
                    else Debug.LogWarning("WARNING: Truss Data '" + _truss.name + "' - Node set 'Reference' should contain 3 nodes.");
                }
            }
            sm_sharedTrusses.Add(resource);
        }
        resource.refCount++;
        return resource.ID;
    }

    public static void ReleaseTruss(int _trussID)
    {
        SharedResource resource = sm_sharedTrusses.Find(x => x.ID == _trussID);
        if (resource != null)
        {
            resource.refCount--;
            if (resource.refCount == 0)
            {
                DestroyTruss(_trussID);
                sm_sharedTrusses.Remove(resource);
            }
        }
    }

    public static int CreateMesh(Mesh _mesh)
    {
        int hashCode = _mesh != null ? _mesh.GetHashCode() : 0;
        SharedResource resource = sm_sharedMeshes.Find(x => x.hashCode == hashCode);
        if (resource == null)
        {
            resource = new SharedResource();
            resource.hashCode = hashCode;
            resource.ID = CreateMesh();
            MeshSetVertices(resource.ID, _mesh.vertices, _mesh.normals, _mesh.tangents);
            List<int> vertices = new List<int>();
            List<int> matter = new List<int>();
            for (int i = 0, s = _mesh.subMeshCount; i < s; ++i)
            {
                int[] indices = _mesh.GetTriangles(i);
                for (int j = 0; j < indices.Length / 3; ++j)
                {
                    vertices.Add(indices[j * 3 + 0]);
                    vertices.Add(indices[j * 3 + 1]);
                    vertices.Add(indices[j * 3 + 2]);
                    matter.Add(i);
                }
            }
            MeshSetFaces(resource.ID, vertices.ToArray(), matter.ToArray());
            sm_sharedMeshes.Add(resource);
        }
        resource.refCount++;
        return resource.ID;
    }

    public static void ReleaseMesh(int _meshID)
    {
        SharedResource resource = sm_sharedMeshes.Find(x => x.ID == _meshID);
        if (resource != null)
        {
            resource.refCount--;
            if (resource.refCount == 0)
            {
                DestroyMesh(_meshID);
                sm_sharedMeshes.Remove(resource);
            }
        }
    }

    public static int CreateTerrain(TerrainData _terrainData)
    {
        int hashCode = _terrainData.GetHashCode();
        SharedResource resource = sm_sharedTerrains.Find(x => x.hashCode == hashCode);
        if (resource == null)
        {
            resource = new SharedResource();
            resource.hashCode = hashCode;
            resource.ID = CreateTerrain();
            int resolution = _terrainData.heightmapResolution;
            float[] heightmap = new float[resolution * resolution];
            System.Buffer.BlockCopy(_terrainData.GetHeights(0, 0, resolution, resolution), 0, heightmap, 0, resolution * resolution * sizeof(float));
            TerrainSetHeightmapSize(resource.ID, resolution, resolution);
            TerrainSetHeightmap(resource.ID, heightmap, 0, 0, resolution, resolution);
            TerrainSetExtents(resource.ID, _terrainData.size);
            sm_sharedTerrains.Add(resource);
        }
        resource.refCount++;
        return resource.ID;
    }

    public static void ReleaseTerrain(int _terrainID)
    {
        SharedResource resource = sm_sharedTerrains.Find(x => x.ID == _terrainID);
        if (resource != null)
        {
            resource.refCount--;
            if (resource.refCount == 0)
            {
                DestroyTerrain(_terrainID);
                sm_sharedTerrains.Remove(resource);
            }
        }
    }

    public static int CreateConvex(Mesh _mesh)
    {
        int hashCode = _mesh != null ? _mesh.GetHashCode() : 0;
        SharedResource resource = sm_sharedConvexes.Find(x => x.hashCode == hashCode);
        if (resource == null)
        {
            resource = new SharedResource();
            resource.hashCode = hashCode;
            resource.ID = CreateConvex();
            List<Vector3> uniqueVertices = new List<Vector3>();
            foreach (Vector3 v in _mesh.vertices)
            {
                if (uniqueVertices.FindIndex(x => Vector3.Distance(x, v) < float.Epsilon) == -1)
                    uniqueVertices.Add(v);
            }
            ConvexSetVertexCount(resource.ID, uniqueVertices.Count);
            ConvexSetVertices(resource.ID, uniqueVertices.ToArray(), 0, uniqueVertices.Count);
            sm_sharedMeshes.Add(resource);
        }
        resource.refCount++;
        return resource.ID;
    }

    public static void ReleaseConvex(int _convexID)
    {
        SharedResource resource = sm_sharedConvexes.Find(x => x.ID == _convexID);
        if (resource != null)
        {
            resource.refCount--;
            if (resource.refCount == 0)
            {
                DestroyConvex(_convexID);
                sm_sharedConvexes.Remove(resource);
            }
        }
    }

    public static int CreateConvex(Vector3 _boxCenter, Vector3 _boxSize)
    {
        int convexID = CreateConvex();
        Vector3[] vertices =
        {
            new Vector3(_boxCenter.x + _boxSize.x * 0.5f, _boxCenter.y + _boxSize.y * 0.5f, _boxCenter.z + _boxSize.z * 0.5f),
            new Vector3(_boxCenter.x - _boxSize.x * 0.5f, _boxCenter.y + _boxSize.y * 0.5f, _boxCenter.z + _boxSize.z * 0.5f),
            new Vector3(_boxCenter.x + _boxSize.x * 0.5f, _boxCenter.y - _boxSize.y * 0.5f, _boxCenter.z + _boxSize.z * 0.5f),
            new Vector3(_boxCenter.x - _boxSize.x * 0.5f, _boxCenter.y - _boxSize.y * 0.5f, _boxCenter.z + _boxSize.z * 0.5f),
            new Vector3(_boxCenter.x + _boxSize.x * 0.5f, _boxCenter.y + _boxSize.y * 0.5f, _boxCenter.z - _boxSize.z * 0.5f),
            new Vector3(_boxCenter.x - _boxSize.x * 0.5f, _boxCenter.y + _boxSize.y * 0.5f, _boxCenter.z - _boxSize.z * 0.5f),
            new Vector3(_boxCenter.x + _boxSize.x * 0.5f, _boxCenter.y - _boxSize.y * 0.5f, _boxCenter.z - _boxSize.z * 0.5f),
            new Vector3(_boxCenter.x - _boxSize.x * 0.5f, _boxCenter.y - _boxSize.y * 0.5f, _boxCenter.z - _boxSize.z * 0.5f),
        };
        List<Vector3> uniqueVertices = new List<Vector3>();
        foreach (Vector3 v in vertices)
        {
            if (uniqueVertices.FindIndex(x => Vector3.Distance(x, v) < float.Epsilon) == -1)
                uniqueVertices.Add(v);
        }
        ConvexSetVertexCount(convexID, uniqueVertices.Count);
        ConvexSetVertices(convexID, uniqueVertices.ToArray(), 0, uniqueVertices.Count);
        return convexID;
    }

    public static int CreateConvex(Vector3 _capsuleCenter, float _capsuleRadius, float _capsuleHeight, int _capsuleDirection)
    {
        int convexID = CreateConvex();
        Vector3 direction = new Vector3(1, 0, 0);
        if (_capsuleDirection == 1) direction = new Vector3(0, 1, 0);
        else if (_capsuleDirection == 0) direction = new Vector3(0, 0, 1);
        float halfLength = Mathf.Max(0.0f, 0.5f * _capsuleHeight - _capsuleRadius);
        Vector3[] vertices =
        {
            _capsuleCenter + direction * halfLength,
            _capsuleCenter - direction * halfLength,
        };
        List<Vector3> uniqueVertices = new List<Vector3>();
        foreach (Vector3 v in vertices)
        {
            if (uniqueVertices.FindIndex(x => Vector3.Distance(x, v) < float.Epsilon) == -1)
                uniqueVertices.Add(v);
        }
        ConvexSetVertexCount(convexID, uniqueVertices.Count);
        ConvexSetVertices(convexID, uniqueVertices.ToArray(), 0, uniqueVertices.Count);
        ConvexSetMargin(convexID, _capsuleRadius);
        return convexID;
    }

    public static int CreateConvex(Vector3 _sphereCenter, float _sphereRadius)
    {
        int convexID = CreateConvex();
        Vector3[] vertices =
        {
            _sphereCenter,
        };
        ConvexSetVertexCount(convexID, vertices.Length);
        ConvexSetVertices(convexID, vertices, 0, vertices.Length);
        ConvexSetMargin(convexID, _sphereRadius);
        return convexID;
    }

    public static int CreateMatter(TxMatter _matter)
    {
        int hashCode = _matter != null ? _matter.GetHashCode() : 0;
        SharedResource resource = sm_sharedMatters.Find(x => x.hashCode == hashCode);
        if (resource == null)
        {
            resource = new SharedResource();
            resource.hashCode = hashCode;
            resource.ID = CreateMatter();
            if (_matter != null)
            {
                _matter.matterID = resource.ID;
                MatterSetStaticFriction(resource.ID, _matter.staticFriction);
                MatterSetSlidingFriction(resource.ID, Mathf.Min(_matter.slidingFriction, _matter.staticFriction));
            }
            sm_sharedMatters.Add(resource);
        }
        resource.refCount++;
        return resource.ID;
    }

    public static int FindMatter(TxMatter _matter)
    {
        int hashCode = _matter != null ? _matter.GetHashCode() : 0;
        SharedResource resource = sm_sharedMatters.Find(x => x.hashCode == hashCode);
        if (resource != null) return resource.ID;
        return -1;
    }

    public static void ReleaseMatter(int _matterID)
    {
        SharedResource resource = sm_sharedMatters.Find(x => x.ID == _matterID);
        if (resource != null)
        {
            resource.refCount--;
            if (resource.refCount == 0)
            {
                DestroyMatter(_matterID);
                sm_sharedMatters.Remove(resource);
            }
        }
    }

    public static void DebugDraw()
    {
        int linesCount = DebugGetLinesCount();
        if (linesCount > 0)
        {
            Vector3[] start = new Vector3[linesCount];
            Vector3[] end = new Vector3[linesCount];
            Color[] color = new Color[linesCount];
            DebugGetLines(start, end, color, 0, linesCount);
            for (int i = 0; i < linesCount; ++i)
            {
                Debug.DrawLine(start[i], end[i], color[i]);
            }
        }
    }

#endregion

#region Private

    static string ErrorGetDescription(int _error)
    {
        IntPtr ptr = TxErrorGetDescription(_error);
        return Marshal.PtrToStringAnsi(ptr);
    }

    static void CheckError(int _error)
    {
        if (_error > 0)
        {
            Debug.LogError(ErrorGetDescription(_error));
            Debug.Break();
        }
    }

    class SharedResource { public int hashCode, ID, refCount = 0; }
    static List<SharedResource> sm_sharedTrusses = new List<SharedResource>();
    static List<SharedResource> sm_sharedMeshes = new List<SharedResource>();
    static List<SharedResource> sm_sharedTerrains = new List<SharedResource>();
    static List<SharedResource> sm_sharedConvexes = new List<SharedResource>();
    static List<SharedResource> sm_sharedMatters = new List<SharedResource>();

#endregion
}
