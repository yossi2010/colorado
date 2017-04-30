/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(TxRigidBody))]
public class TxRigidBodyEditor : TxBodyEditor
{
    TxRigidBody[] m_targets;

    SerializedProperty massScale;
    SerializedProperty collision;
    SerializedProperty mesh;
    SerializedProperty shapeCenter;
    SerializedProperty shapeSize;
    SerializedProperty capsuleDirection;
    SerializedProperty margin;
    SerializedProperty matters;
    SerializedProperty interaction;
    SerializedProperty spawnActive;
    SerializedProperty deactivation;
    SerializedProperty deactivationSpeed;
    SerializedProperty deactivationTime;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_targets = new TxRigidBody[targets.Length];
        for (int i = 0; i < targets.Length; ++i) m_targets[i] = (TxRigidBody)targets[i];

        massScale = serializedObject.FindProperty("m_massScale");
        collision = serializedObject.FindProperty("m_collision");
        mesh = serializedObject.FindProperty("m_mesh");
        shapeCenter = serializedObject.FindProperty("m_shapeCenter");
        shapeSize = serializedObject.FindProperty("m_shapeSize");
        capsuleDirection = serializedObject.FindProperty("m_capsuleDirection");
        margin = serializedObject.FindProperty("m_margin");
        matters = serializedObject.FindProperty("m_matters");
        interaction = serializedObject.FindProperty("m_interaction");
        spawnActive = serializedObject.FindProperty("m_spawnActive");
        deactivation = serializedObject.FindProperty("m_deactivation");
        deactivationSpeed = serializedObject.FindProperty("m_deactivationSpeed");
        deactivationTime = serializedObject.FindProperty("m_deactivationTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TxEditor.LookLikeControls();

        bool isPlaying = Application.isPlaying;

        GUI.enabled = true && !isPlaying;
        EditorGUILayout.PropertyField(massScale);
        bool sameMass = true;
        for (int i = 1; i < m_targets.Length; ++i)
        {
            if (m_targets[i].mass != m_targets[i - 1].mass)
            {
                sameMass = false;
                break;
            }
        }
        if (sameMass)
        {
            GUI.enabled = false;
            EditorGUILayout.FloatField("Scaled Mass", m_targets[0].mass);
        }
        else
        {
            GUI.enabled = false;
            EditorGUILayout.TextField("Scaled Mass", "-");
        }
        GUI.enabled = true && !isPlaying;
        EditorGUILayout.PropertyField(collision);

        EditorGUI.indentLevel++;
        switch (collision.enumValueIndex)
        {
            case 1:
                EditorGUILayout.PropertyField(mesh);
                break;
            case 2:
                EditorGUILayout.PropertyField(mesh);
                break;
            case 3:
                EditorGUILayout.PropertyField(shapeCenter, new GUIContent("Box Center"));
                EditorGUILayout.PropertyField(shapeSize, new GUIContent("Box Size"));
                break;
            case 4:
                EditorGUILayout.PropertyField(shapeCenter);
                EditorGUILayout.PropertyField(shapeSize.FindPropertyRelative("x"), new GUIContent("Capsule Radius"));
                EditorGUILayout.PropertyField(shapeSize.FindPropertyRelative("y"), new GUIContent("Capsule Height"));
                EditorGUILayout.IntPopup(capsuleDirection, new GUIContent[] { new GUIContent("X-Axis"), new GUIContent("Y-Axis"), new GUIContent("Z-Axis") }, new int[] { 0, 1, 2 });
                break;
            case 5:
                EditorGUILayout.PropertyField(shapeCenter);
                EditorGUILayout.PropertyField(shapeSize.FindPropertyRelative("x"), new GUIContent("Sphere Radius"));
                break;
        }
        EditorGUI.indentLevel--;

        EditorGUI.indentLevel++;
        GUI.enabled = (collision.enumValueIndex > 0) && !isPlaying;
        EditorGUILayout.PropertyField(margin);
        EditorGUILayout.PropertyField(matters, true);
        EditorGUI.indentLevel--;

        SpawnEnabledUI();
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(spawnActive);
        EditorGUI.indentLevel--;
        GUI.enabled = true;
        EditorGUILayout.PropertyField(deactivation);
        EditorGUI.indentLevel++;
        GUI.enabled = deactivation.boolValue;
        EditorGUILayout.PropertyField(deactivationSpeed, new GUIContent("Speed"));
        EditorGUILayout.PropertyField(deactivationTime, new GUIContent("Time"));
        EditorGUI.indentLevel--;

        BodyGroupUI();

        GUI.enabled = true && !isPlaying;
        EditorGUILayout.PropertyField(interaction);

        if (GUI.changed) serializedObject.ApplyModifiedProperties();
    }
}
