/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(TxTruss))]
public class TxTrussEditor : Editor
{
    TxTruss[] m_targets;

    protected virtual void OnEnable()
    {
        m_targets = new TxTruss[targets.Length];
        for (int i = 0; i < targets.Length; ++i) m_targets[i] = (TxTruss)targets[i];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TxEditor.LookLikeControls();

        EditorGUILayout.LabelField("Use Soft Body inspector Edit button to open Truss Designer");

        if (GUI.changed) serializedObject.ApplyModifiedProperties();
    }
}
