/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TxBodyEditor : Editor
{
    [System.NonSerialized]
    TxBody[] m_targets;

    static bool sm_showGroup = false;
    static bool sm_showLayers = false;
    static bool sm_showCollision = false;

    SerializedProperty spawnEnabled;
    SerializedProperty groupRoot;
    SerializedProperty groupLayers;
    SerializedProperty groupCollision;
    SerializedProperty groupLayer;

    protected virtual void OnEnable()
    {
        m_targets = new TxBody[targets.Length];
        for (int i = 0; i < targets.Length; ++i) m_targets[i] = (TxBody)targets[i];

        spawnEnabled = serializedObject.FindProperty("m_spawnEnabled");
        groupRoot = serializedObject.FindProperty("m_groupRoot");
        groupLayers = serializedObject.FindProperty("m_groupLayers");
        groupCollision = serializedObject.FindProperty("m_groupCollision");
        groupLayer = serializedObject.FindProperty("m_groupLayer");
    }

    protected void SpawnEnabledUI()
    {
        EditorGUILayout.PropertyField(spawnEnabled);
    }

    protected void BodyGroupUI()
    {
        bool isPlaying = Application.isPlaying;

        sm_showGroup = EditorGUILayout.Foldout(sm_showGroup, "Group");
        if (sm_showGroup)
        {
            EditorGUI.indentLevel++;

            GUI.enabled = !isPlaying;
            EditorGUILayout.PropertyField(groupRoot, new GUIContent("Root"));

            TxBody rootBody = null;
            int inGroup = InSameGroup(ref rootBody);
            switch (inGroup)
            {
                case 0:
                    GUI.enabled = false;
                    EditorGUILayout.LabelField("Different groups");
                    break;
                case -1:
                    GUI.enabled = false;
                    EditorGUILayout.LabelField("Not in a group");
                    break;
                case 1:
                    GUI.enabled = !isPlaying;
                    if (m_targets.Length == 1 && m_targets[0] == rootBody)
                    {
                        EditorGUI.indentLevel++;
                        GroupLayersUI();
                        GroupCollisionUI(rootBody);
                        EditorGUI.indentLevel--;
                    }
                    GroupLayerUI(rootBody);
                    break;
            }
            EditorGUI.indentLevel--;
        }
    }

    int InSameGroup(ref TxBody _rootBody)
    {
        foreach (var b in m_targets)
        {
            int inGroup = InGroup(b, ref _rootBody);
            if (inGroup != 1) return inGroup;
        }
        return 1;
    }

    int InGroup(TxBody _target, ref TxBody _rootBody)
    {
        Transform parent = _target.transform;
        while (parent != null)
        {
            TxBody parentBody = parent.GetComponent<TxBody>();
            if (parentBody != null && parentBody.groupRoot)
            {
                if (_rootBody == null) _rootBody = parentBody;
                else if (_rootBody != parentBody) return 0;
                return 1;
            }
            parent = parent.parent;
        }
        return -1;
    }

    void GroupLayersUI()
    {
        sm_showLayers = EditorGUILayout.Foldout(sm_showLayers, "Layers");
        if (sm_showLayers)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < groupLayers.arraySize; ++i)
            {
                SerializedProperty groupLayer = groupLayers.GetArrayElementAtIndex(i);
                GUI.enabled = (i > 0);
                EditorGUILayout.PropertyField(groupLayer, new GUIContent("Layer " + i));
            }
            EditorGUI.indentLevel--;
        }
    }

    void GroupLayerUI(TxBody _rootBody)
    {
        string[] groupLayers = _rootBody.groupLayers;
        List<GUIContent> layerNames = new List<GUIContent>();
        List<int> layerIndices = new List<int>();
        for (int i = 0; i < groupLayers.Length; ++i)
        {
            string layerName = groupLayers[i];
            if (layerName != string.Empty)
            {
                layerNames.Add(new GUIContent(layerName));
                layerIndices.Add(i);
            }
        }
        EditorGUILayout.IntPopup(groupLayer, layerNames.ToArray(), layerIndices.ToArray(), new GUIContent("Layer"));
    }

    void GroupCollisionUI(TxBody _rootBody)
    {
        string[] groupLayers = _rootBody.groupLayers;
        List<GUIContent> layerNames = new List<GUIContent>();
        List<int> layerIndices = new List<int>();
        GUIStyle lableStyle = EditorStyles.label;
        float maxLength = 0;
        for (int i = 0; i < groupLayers.Length; ++i)
        {
            string layerName = groupLayers[i];
            if (layerName != string.Empty)
            {
                layerNames.Add(new GUIContent(layerName));
                layerIndices.Add(i);
                maxLength = Mathf.Max(maxLength, lableStyle.CalcSize(new GUIContent(layerName)).x);
            }
        }
        int num = layerNames.Count;
        sm_showCollision = EditorGUILayout.Foldout(sm_showCollision, "Collision");
        if (sm_showCollision)
        {
            Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, maxLength);
            for (int j = 0; j < num; j++)
            {
                Vector3 pos = new Vector3(EditorGUIUtility.labelWidth + (num - j + 1) * 15f - 1f, rect.y - 9f, 0f);
                GUI.matrix = Matrix4x4.identity;
                GUIUtility.RotateAroundPivot(90.0f, pos);
                if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9.0"))
                    GUI.matrix *= Matrix4x4.TRS(new Vector3(-0.5f, -0.5f, 0f), Quaternion.identity, Vector3.one);
                GUI.Label(new Rect(pos.x, pos.y, maxLength, 15f), layerNames[j], "RightLabel");
            }
            GUI.matrix = Matrix4x4.identity;
            for (int k = 0; k < num; k++)
            {
                GUILayoutUtility.GetRect((float)(30 + 15 * num + 100), 15f);
                GUI.Label(new Rect(15f, rect.y + maxLength + k * 15f - 10f, EditorGUIUtility.labelWidth, 15f), layerNames[k], "RightLabel");
                for (int l = k; l < num; l++)
                {
                    GUIContent content = new GUIContent(string.Empty, layerNames[k].text + "/" + layerNames[l].text);
                    int ki = layerIndices[k], li = layerIndices[l];
                    int index = ki * 8 - ki * (ki + 1) / 2 + li;
                    SerializedProperty collision = groupCollision.GetArrayElementAtIndex(index);
                    bool flag = collision.boolValue;
                    bool flag2 = GUI.Toggle(new Rect(EditorGUIUtility.labelWidth + (num - l) * 15f, rect.y + maxLength + k * 15f - 10f, 15f, 15f), flag, content);
                    if (flag2 != flag)
                        collision.boolValue = flag2;
                }
            }
        }
    }
}
