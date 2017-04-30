/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(TxStaticBody))]
public class TxStaticBodyEditor : TxBodyEditor
{
    TxStaticBody[] m_targets;

    SerializedProperty collision;
    SerializedProperty mesh;
    SerializedProperty terrain;
    SerializedProperty shapeCenter;
    SerializedProperty shapeSize;
    SerializedProperty capsuleDirection;
    SerializedProperty margin;
    SerializedProperty matters;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_targets = new TxStaticBody[targets.Length];
        for (int i = 0; i < targets.Length; ++i) m_targets[i] = (TxStaticBody)targets[i];

        collision = serializedObject.FindProperty("m_collision");
        mesh = serializedObject.FindProperty("m_mesh");
        terrain = serializedObject.FindProperty("m_terrain");
        shapeCenter = serializedObject.FindProperty("m_shapeCenter");
        shapeSize = serializedObject.FindProperty("m_shapeSize");
        capsuleDirection = serializedObject.FindProperty("m_capsuleDirection");
        margin = serializedObject.FindProperty("m_margin");
        matters = serializedObject.FindProperty("m_matters");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TxEditor.LookLikeControls();

        bool isPlaying = Application.isPlaying;

        GUI.enabled = true && !isPlaying;
        EditorGUILayout.PropertyField(collision);
        EditorGUI.indentLevel++;
        switch (collision.enumValueIndex)
        {
            case 1:
                EditorGUILayout.PropertyField(mesh);
                break;
            case 2:
                EditorGUILayout.PropertyField(terrain);
                break;
            case 3:
                EditorGUILayout.PropertyField(mesh);
                break;
            case 4:
                EditorGUILayout.PropertyField(shapeCenter, new GUIContent("Box Center"));
                EditorGUILayout.PropertyField(shapeSize, new GUIContent("Box Size"));
                break;
            case 5:
                EditorGUILayout.PropertyField(shapeCenter, new GUIContent("Capsule Center"));
                EditorGUILayout.PropertyField(shapeSize.FindPropertyRelative("x"), new GUIContent("Capsule Radius"));
                EditorGUILayout.PropertyField(shapeSize.FindPropertyRelative("y"), new GUIContent("Capsule Height"));
                EditorGUILayout.IntPopup(capsuleDirection, new GUIContent[] { new GUIContent("X-Axis"), new GUIContent("Y-Axis"), new GUIContent("Z-Axis") }, new int[] { 0, 1, 2 });
                break;
            case 6:
                EditorGUILayout.PropertyField(shapeCenter, new GUIContent("Sphere Center"));
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
        BodyGroupUI();

        if (GUI.changed) serializedObject.ApplyModifiedProperties();
    }
}
