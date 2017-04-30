/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(TxConstraint))]
public class TxConstraintEditor : Editor
{
    TxConstraint[] m_targets;

    SerializedProperty baseBody;
    SerializedProperty disableCollision;
    SerializedProperty showSnaps;
    SerializedProperty snaps;
    SerializedProperty enableMotor;
    SerializedProperty axisNodeSet;
    SerializedProperty targetRate;
    SerializedProperty maxTorque;

    //static bool showSnaps = false;
    //bool[] showSnap = null;

    protected virtual void OnEnable()
    {
        m_targets = new TxConstraint[targets.Length];
        for (int i = 0; i < targets.Length; ++i) m_targets[i] = (TxConstraint)targets[i];

        baseBody = serializedObject.FindProperty("m_baseBody");
        disableCollision = serializedObject.FindProperty("m_disableCollision");
        showSnaps = serializedObject.FindProperty("m_showSnaps");
        snaps = serializedObject.FindProperty("m_snaps");
        enableMotor = serializedObject.FindProperty("m_enableMotor");
        axisNodeSet = serializedObject.FindProperty("m_axisNodeSet");
        targetRate = serializedObject.FindProperty("m_targetRate");
        maxTorque = serializedObject.FindProperty("m_maxTorque");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TxEditor.LookLikeControls();

        bool isPlaying = Application.isPlaying;

        GUI.enabled = true && !isPlaying;
        EditorGUILayout.PropertyField(baseBody);
        EditorGUILayout.PropertyField(disableCollision);

        if (m_targets.Length > 1)
        {
            GUI.enabled = false;
            EditorGUILayout.LabelField("Select single body to edit snaps");
            GUI.enabled = true;
        }
        else
        {
            TxSoftBody softBodyA = m_targets[0].GetComponent<TxSoftBody>();
            TxSoftBody softBodyB = baseBody.objectReferenceValue as TxSoftBody;
            for (int i = snaps.arraySize - 1; i >= 0; --i)
            {
                SerializedProperty snap = snaps.GetArrayElementAtIndex(i);
                SerializedProperty snapType = snap.FindPropertyRelative("type");
                if ((snapType.enumValueIndex == 0 && softBodyB != null) || (snapType.enumValueIndex != 0 && softBodyB == null))
                {
                    snaps.DeleteArrayElementAtIndex(i);
                    showSnaps.boolValue = false;
                }
            }
            //
            GUI.enabled = true && !isPlaying;
            EditorGUILayout.BeginHorizontal();
            showSnaps.boolValue = EditorGUILayout.Foldout(showSnaps.boolValue, "Snap List (" + snaps.arraySize + " Item" + (snaps.arraySize == 1 ? "" : "s") + ")");
            if (showSnaps.boolValue)
            {
                if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.MaxWidth(50)))
                {
                    snaps.InsertArrayElementAtIndex(0);
                    // Initialize new snap
                    snaps.GetArrayElementAtIndex(0).FindPropertyRelative("type").enumValueIndex = softBodyB != null ? 1 : 0;
                    snaps.GetArrayElementAtIndex(0).FindPropertyRelative("node").stringValue = "";
                    snaps.GetArrayElementAtIndex(0).FindPropertyRelative("featureB").stringValue = "";
                    snaps.GetArrayElementAtIndex(0).FindPropertyRelative("minLimit").floatValue = 0;
                    snaps.GetArrayElementAtIndex(0).FindPropertyRelative("maxLimit").floatValue = 0;
                    snaps.GetArrayElementAtIndex(0).FindPropertyRelative("strength").floatValue = Mathf.Infinity;
                    snaps.GetArrayElementAtIndex(0).FindPropertyRelative("master").boolValue = true;
                    snaps.GetArrayElementAtIndex(0).FindPropertyRelative("show").boolValue = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (showSnaps.boolValue)
            {
                EditorGUI.indentLevel++;
                if (softBodyB != null)
                {
                    for (int i = 0; i < snaps.arraySize; ++i)
                    {
                        SerializedProperty snap = snaps.GetArrayElementAtIndex(i);
                        SerializedProperty showSnap = snap.FindPropertyRelative("show");
                        if (showSnap.boolValue)
                        {
                            EditorGUILayout.BeginHorizontal();
                            showSnap.boolValue = EditorGUILayout.Foldout(showSnap.boolValue, "Snap " + (i + 1));
                            if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.MaxWidth(50)))
                            {
                                snaps.DeleteArrayElementAtIndex(i);
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel++;
                            SerializedProperty node = snap.FindPropertyRelative("node");
                            string[] nodes = GetNodes(softBodyA);
                            int index = System.Array.IndexOf(nodes, node.stringValue);
                            index = EditorGUILayout.Popup("Node", index, nodes);
                            if (index > -1 && index < nodes.Length) node.stringValue = nodes[index];
                            SerializedProperty snapType = snap.FindPropertyRelative("type");
                            int selectType = EditorGUILayout.Popup("Snap To", snapType.enumValueIndex - 1, new string[] { "Node", "Edge" });
                            //EditorGUILayout.PropertyField(snapType, new GUIContent("Snap To"));
                            snapType.enumValueIndex = selectType + 1;
                            switch (snapType.enumValueIndex)
                            {
                                case 1:
                                    {
                                        SerializedProperty nodeB = snap.FindPropertyRelative("featureB");
                                        string[] nodesB = GetNodes(softBodyB);
                                        index = System.Array.IndexOf(nodesB, nodeB.stringValue);
                                        index = EditorGUILayout.Popup("Node B", index, nodesB);
                                        if (index > -1 && index < nodesB.Length) nodeB.stringValue = nodesB[index];
                                    }
                                    break;
                                case 2:
                                    {
                                        SerializedProperty nodeB = snap.FindPropertyRelative("featureB");
                                        string[] nodesB = GetNodes(softBodyB, 2, 2);
                                        index = System.Array.IndexOf(nodesB, nodeB.stringValue);
                                        index = EditorGUILayout.Popup("Nodes B", index, nodesB);
                                        if (index > -1 && index < nodesB.Length) nodeB.stringValue = nodesB[index];
                                    }
                                    break;
                            }
                            SerializedProperty snapMinLimit = snap.FindPropertyRelative("minLimit");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(snapMinLimit);
                            if (GUILayout.Button("Calc", TxEditor.MiniUnpressedStyle(), GUILayout.MaxWidth(40)))
                            {
                                snapMinLimit.floatValue = CalcSnapDistance(node.stringValue, snap.FindPropertyRelative("featureB").stringValue);
                            }
                            EditorGUILayout.EndHorizontal();
                            SerializedProperty snapMaxLimit = snap.FindPropertyRelative("maxLimit");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(snapMaxLimit);
                            if (GUILayout.Button("Calc", TxEditor.MiniUnpressedStyle(), GUILayout.MaxWidth(40)))
                            {
                                snapMaxLimit.floatValue = CalcSnapDistance(node.stringValue, snap.FindPropertyRelative("featureB").stringValue);
                            }
                            EditorGUILayout.EndHorizontal();
                            SerializedProperty snapStrength = snap.FindPropertyRelative("strength");
                            EditorGUILayout.PropertyField(snapStrength);
                            SerializedProperty snapMaster = snap.FindPropertyRelative("master");
                            EditorGUILayout.PropertyField(snapMaster);
                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            SerializedProperty node = snap.FindPropertyRelative("node");
                            string nodeName = node.stringValue;
                            string targetName = "Point";
                            SerializedProperty snapType = snap.FindPropertyRelative("type");
                            if (snapType.enumValueIndex > 0)
                            {
                                SerializedProperty nodeB = snap.FindPropertyRelative("featureB");
                                targetName = nodeB.stringValue;
                            }
                            showSnap.boolValue = EditorGUILayout.Foldout(showSnap.boolValue, "Snap " + (i + 1) + ": " + nodeName + " - " + targetName);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < snaps.arraySize; ++i)
                    {
                        SerializedProperty snap = snaps.GetArrayElementAtIndex(i);
                        SerializedProperty showSnap = snap.FindPropertyRelative("show");
                        if (showSnap.boolValue)
                        {
                            EditorGUILayout.BeginHorizontal();
                            showSnap.boolValue = EditorGUILayout.Foldout(showSnap.boolValue, "Snap " + (i + 1));
                            if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.MaxWidth(50)))
                            {
                                snaps.DeleteArrayElementAtIndex(i);
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel++;
                            SerializedProperty node = snap.FindPropertyRelative("node");
                            string[] nodes = GetNodes(softBodyA, 1, 100);
                            int index = System.Array.IndexOf(nodes, node.stringValue);
                            index = EditorGUILayout.Popup("Nodes", index, nodes);
                            if (index > -1 && index < nodes.Length) node.stringValue = nodes[index];
                            //SerializedProperty snapMinLimit = snap.FindPropertyRelative("minLimit");
                            //EditorGUILayout.BeginHorizontal();
                            //EditorGUILayout.PropertyField(snapMinLimit);
                            //EditorGUILayout.EndHorizontal();
                            SerializedProperty snapMaxLimit = snap.FindPropertyRelative("maxLimit");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(snapMaxLimit);
                            EditorGUILayout.EndHorizontal();
                            SerializedProperty snapStrength = snap.FindPropertyRelative("strength");
                            EditorGUILayout.PropertyField(snapStrength);
                            SerializedProperty snapMaster = snap.FindPropertyRelative("master");
                            EditorGUILayout.PropertyField(snapMaster);
                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            SerializedProperty node = snap.FindPropertyRelative("node");
                            showSnap.boolValue = EditorGUILayout.Foldout(showSnap.boolValue, "Snap " + (i + 1) + ": " + node.stringValue);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(enableMotor);

        EditorGUI.indentLevel++;
        if (m_targets.Length == 1)
        {
            GUI.enabled = true && !isPlaying;
            string[] axisNodes = GetNodes(m_targets[0].GetComponent<TxSoftBody>(), 2, 2);
            int index = System.Array.IndexOf(axisNodes, axisNodeSet.stringValue);
            index = EditorGUILayout.Popup("Axis Nodes", index, axisNodes);
            if (index > -1 && index < axisNodes.Length) axisNodeSet.stringValue = axisNodes[index];
        }
        else
        {
            GUI.enabled = false;
            string[] empty = { };
            EditorGUILayout.Popup("Axis Nodes", -1, empty);
            GUI.enabled = true;
        }
        GUI.enabled = true;
        EditorGUILayout.PropertyField(targetRate);
        EditorGUILayout.PropertyField(maxTorque);
        EditorGUI.indentLevel--;

        if (GUI.changed) serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        Color yellow = new Color(1.0f, 0.5f, 0.0f);
        Color green = new Color(0.0f, 0.5f, 0.0f);
        Color blue = new Color(0.0f, 0.5f, 1.0f);
        Color red = new Color(0.5f, 0.0f, 0.0f);
        float size = 0.1f;
        foreach (TxConstraint constraint in m_targets)
        {
            if (!constraint.enabled || constraint.isBroken) continue;
            TxSoftBody bodyA = constraint.attachedBody;
            TxTruss trussA = bodyA.truss;
            if (trussA == null) continue;
            if (constraint.enableMotor)
            {
                int[] motorAxis = trussA.FindNodeSet(constraint.motorAxis);
                if (motorAxis != null && motorAxis.Length == 2)
                {
                    Vector3 axis0 = bodyA.nodePosition[motorAxis[0]];
                    Vector3 axis1 = bodyA.nodePosition[motorAxis[1]];
                    Handles.color = red;
                    Handles.DrawLine(axis0, axis1);
                }
            }
            if (constraint.baseBody is TxSoftBody)
            {
                Handles.color = yellow;
                TxSoftBody bodyB = constraint.baseBody as TxSoftBody;
                TxTruss trussB = bodyB.truss;
                if (trussB == null) continue;
                TxConstraint.Snap[] snaps = constraint.snaps;
                foreach (var s in snaps)
                {
                    if (!s.show) continue;
                    int[] indicesA = trussA.FindNodeSet(s.node);
                    if (indicesA != null && indicesA.Length == 1)
                    {
                        Vector3 positionA = bodyA.nodePosition[indicesA[0]];
                        int[] indicesB = trussB.FindNodeSet(s.featureB);
                        if (indicesB != null)
                        {
                            switch (s.type)
                            {
                                case TxConstraint.SnapType.Node:
                                    if (indicesB.Length == 1)
                                    {
                                        Matrix4x4 matrixB = bodyB.transform.localToWorldMatrix;
                                        Vector3 positionB = bodyB.nodePosition[indicesB[0]];
                                        if (Vector3.Distance(positionA, positionB) > 0.001f)
                                        {
                                            Handles.color = yellow;
                                            size = HandleUtility.GetHandleSize(positionA) * 0.08f;
                                            Handles.SphereCap(-1, positionA, Quaternion.identity, size);
                                            Handles.color = blue;
                                            size = HandleUtility.GetHandleSize(positionB) * 0.08f;
                                            Handles.SphereCap(-1, positionB, Quaternion.identity, size);
                                            Handles.color = green;
                                            Handles.DrawLine(positionA, positionB);
                                        }
                                        else
                                        {
                                            Handles.color = green;
                                            size = HandleUtility.GetHandleSize(positionA) * 0.08f;
                                            Handles.SphereCap(-1, positionA, Quaternion.identity, size);
                                        }
                                        if (s.maxLimit > 0.001f)
                                        {
                                            if (s.minLimit < s.maxLimit - 0.001f)
                                            {
                                                Handles.color = blue;
                                                Handles.DrawWireDisc(positionB, matrixB.GetColumn(0), s.minLimit);
                                                Handles.DrawWireDisc(positionB, matrixB.GetColumn(1), s.minLimit);
                                                Handles.DrawWireDisc(positionB, matrixB.GetColumn(2), s.minLimit);
                                                Handles.color = yellow;
                                                Handles.DrawWireDisc(positionB, matrixB.GetColumn(0), s.maxLimit);
                                                Handles.DrawWireDisc(positionB, matrixB.GetColumn(1), s.maxLimit);
                                                Handles.DrawWireDisc(positionB, matrixB.GetColumn(2), s.maxLimit);
                                            }
                                            else
                                            {
                                                Handles.color = green;
                                                Handles.DrawWireDisc(positionB, matrixB.GetColumn(0), s.maxLimit);
                                                Handles.DrawWireDisc(positionB, matrixB.GetColumn(1), s.maxLimit);
                                                Handles.DrawWireDisc(positionB, matrixB.GetColumn(2), s.maxLimit);
                                            }
                                        }
                                    }
                                    break;
                                case TxConstraint.SnapType.Edge:
                                    if (indicesB.Length == 2)
                                    {
                                        Vector3 positionB0 = bodyB.nodePosition[indicesB[0]];
                                        Vector3 positionB1 = bodyB.nodePosition[indicesB[1]];
                                        Handles.color = blue;
                                        size = HandleUtility.GetHandleSize(positionB0) * 0.08f;
                                        Handles.SphereCap(-1, positionB0, Quaternion.identity, size);
                                        size = HandleUtility.GetHandleSize(positionB1) * 0.08f;
                                        Handles.SphereCap(-1, positionB1, Quaternion.identity, size);
                                        Handles.DrawLine(positionB0, positionB1);
                                        Vector3 positionB = positionB0 + (positionB1 - positionB0) * Vector3.Dot(positionB1 - positionB0, positionA - positionB0) / (positionB1 - positionB0).sqrMagnitude;
                                        if (Vector3.Distance(positionA, positionB) > 0.001f)
                                        {
                                            Handles.color = yellow;
                                            size = HandleUtility.GetHandleSize(positionA) * 0.08f;
                                            Handles.SphereCap(-1, positionA, Quaternion.identity, size);
                                            Handles.color = blue;
                                            size = HandleUtility.GetHandleSize(positionB) * 0.08f;
                                            Handles.SphereCap(-1, positionB, Quaternion.identity, size);
                                            Handles.color = green;
                                            Handles.DrawLine(positionA, positionB);
                                        }
                                        else
                                        {
                                            Handles.color = green;
                                            size = HandleUtility.GetHandleSize(positionA) * 0.08f;
                                            Handles.SphereCap(-1, positionA, Quaternion.identity, size);
                                        }
                                        if (s.maxLimit > 0.001f)
                                        {
                                            Vector3 normal = (positionB1 - positionB0).normalized;
                                            if (s.minLimit < s.maxLimit - 0.001f)
                                            {
                                                Handles.color = blue;
                                                Handles.DrawWireDisc(positionB0, normal, s.minLimit);
                                                Handles.DrawWireDisc((positionB0 + positionB1) * 0.5f, normal, s.minLimit);
                                                Handles.DrawWireDisc(positionB1, normal, s.minLimit);
                                                Handles.color = yellow;
                                                Handles.DrawWireDisc(positionB0, normal, s.maxLimit);
                                                Handles.DrawWireDisc((positionB0 + positionB1) * 0.5f, normal, s.maxLimit);
                                                Handles.DrawWireDisc(positionB1, normal, s.maxLimit);
                                            }
                                            else
                                            {
                                                Handles.color = green;
                                                Handles.DrawWireDisc(positionB0, normal, s.maxLimit);
                                                Handles.DrawWireDisc((positionB0 + positionB1) * 0.5f, normal, s.maxLimit);
                                                Handles.DrawWireDisc(positionB1, normal, s.maxLimit);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                TxBody bodyB = constraint.baseBody;
                Matrix4x4 matrixA = Application.isPlaying ? constraint.startMatrix : bodyB ? bodyB.transform.worldToLocalMatrix * bodyA.transform.localToWorldMatrix : bodyA.transform.localToWorldMatrix;
                Matrix4x4 matrixB = bodyB ? bodyB.transform.localToWorldMatrix : Matrix4x4.identity;
                TxConstraint.Snap[] snaps = constraint.snaps;
                foreach (var s in snaps)
                {
                    if (!s.show) continue;
                    int[] indices = trussA.FindNodeSet(s.node);
                    if (indices != null) foreach (var i in indices)
                    {
                        if (s.maxLimit > 0.001f)
                        {
                            Vector3 positionA = bodyA.nodePosition[i];
                            Vector3 positionB = matrixB.MultiplyPoint(matrixA.MultiplyPoint(trussA.nodePosition[i]));
                            if (Vector3.Distance(positionA, positionB) > 0.001f)
                            {
                                Handles.color = yellow;
                                size = HandleUtility.GetHandleSize(positionA) * 0.08f;
                                Handles.SphereCap(-1, positionA, Quaternion.identity, size);
                                Handles.color = blue;
                                size = HandleUtility.GetHandleSize(positionB) * 0.08f;
                                Handles.SphereCap(-1, positionB, Quaternion.identity, size);
                            }
                            else
                            {
                                Handles.color = green;
                                size = HandleUtility.GetHandleSize(positionA) * 0.08f;
                                Handles.SphereCap(-1, positionA, Quaternion.identity, size);
                            }
                            Handles.color = green;
                            Handles.DrawWireDisc(positionB, matrixB.GetColumn(0), s.maxLimit);
                            Handles.DrawWireDisc(positionB, matrixB.GetColumn(1), s.maxLimit);
                            Handles.DrawWireDisc(positionB, matrixB.GetColumn(2), s.maxLimit);
                        }
                        else
                        {
                            Vector3 position = matrixB.MultiplyPoint(matrixA.MultiplyPoint(trussA.nodePosition[i]));
                            Handles.color = green;
                            size = HandleUtility.GetHandleSize(position) * 0.08f;
                            Handles.SphereCap(-1, position, Quaternion.identity, size);
                        }
                    }
                }
            }
        }
        Handles.color = Color.white;
    }

        string[] GetNodes(TxSoftBody _body, int _minNodes = 1, int _maxNodes = 1)
    {
        List<string> nodes = new List<string>();
        if (_body != null && _body.truss != null)
        {
            foreach (TxTruss.NamedSet s in _body.truss.nodesSet)
            {
                if (s.indices.Length >= _minNodes && s.indices.Length <= _maxNodes) nodes.Add(s.name);
            }
        }
        return nodes.ToArray();
    }

    float CalcSnapDistance(string _nodeA, string _nodesB)
    {
        TxSoftBody body0 = m_targets[0].GetComponent<TxSoftBody>();
        int[] nodeIndex = body0.truss.FindNodeSet(_nodeA);
        if (nodeIndex.Length == 1)
        {
            Vector3 p0 = body0.transform.TransformPoint(body0.truss.nodePosition[nodeIndex[0]]);
            TxSoftBody body1 = (TxSoftBody)baseBody.objectReferenceValue;
            if (body1 != null)
            {
                int[] nodeIndices = body1.truss.FindNodeSet(_nodesB);
                switch (nodeIndices.Length)
                {
                    case 1:
                        {
                            Vector3 p1 = body1.transform.TransformPoint(body1.truss.nodePosition[nodeIndices[0]]);
                            return Vector3.Distance(p0, p1);
                        }
                    case 2:
                        {
                            Vector3 p1 = body1.transform.TransformPoint(body1.truss.nodePosition[nodeIndices[0]]);
                            Vector3 p2 = body1.transform.TransformPoint(body1.truss.nodePosition[nodeIndices[1]]);
                            return Vector3.Distance(p0, p1 + (p2 - p1) * Vector3.Dot(p2 - p1, p0 - p1) / (p2 - p1).sqrMagnitude);
                        }
                }
            }
        }
        return 0;
    }
}
