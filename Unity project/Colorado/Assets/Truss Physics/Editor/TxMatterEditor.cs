/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(TxMatter))]
public class TxMatterEditor : Editor
{
    TxMatter[] m_targets;

    SerializedProperty staticFriction;
    SerializedProperty slidingFriction;

    protected virtual void OnEnable()
    {
        m_targets = new TxMatter[targets.Length];
        for (int i = 0; i < targets.Length; ++i) m_targets[i] = (TxMatter)targets[i];

        staticFriction = serializedObject.FindProperty("m_staticFriction");
        slidingFriction = serializedObject.FindProperty("m_slidingFriction");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TxEditor.LookLikeControls();

        EditorGUILayout.PropertyField(staticFriction);
        EditorGUILayout.PropertyField(slidingFriction);

        if (GUI.changed) serializedObject.ApplyModifiedProperties();
    }
}
