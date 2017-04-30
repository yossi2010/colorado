/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(TxSceneSettings))]
public class TxSceneSettingsEditor : Editor
{
    TxSceneSettings[] m_targets;

    //SerializedProperty simulationStep;
    SerializedProperty substepPower;
    SerializedProperty solverIterations;
    SerializedProperty globalGravity;
    SerializedProperty globalPressure;
    SerializedProperty workerThreads;

    protected virtual void OnEnable()
    {
        m_targets = new TxSceneSettings[targets.Length];
        for (int i = 0; i < targets.Length; ++i) m_targets[i] = (TxSceneSettings)targets[i];

        //simulationStep = serializedObject.FindProperty("m_simulationStep");
        substepPower = serializedObject.FindProperty("m_substepPower");
        solverIterations = serializedObject.FindProperty("m_solverIterations");
        globalGravity = serializedObject.FindProperty("m_globalGravity");
        globalPressure = serializedObject.FindProperty("m_globalPressure");
        workerThreads = serializedObject.FindProperty("m_workerThreads");

        TxEditor.HideHandles(true);
    }

    void OnDisable()
    {
        TxEditor.HideHandles(false);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TxEditor.LookLikeControls();

        EditorGUILayout.LabelField("Simulation");

        EditorGUI.indentLevel++;
        //EditorGUILayout.PropertyField(simulationStep);
        EditorGUILayout.PropertyField(substepPower);
        GUI.enabled = false;
        EditorGUILayout.IntField("Substep Count", 1 << substepPower.intValue);
        EditorGUILayout.FloatField("Substep Size", Time.fixedDeltaTime / (1 << substepPower.intValue));
        GUI.enabled = true;
        EditorGUILayout.PropertyField(solverIterations);
        EditorGUI.indentLevel--;

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Environment");

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(globalGravity);
        EditorGUILayout.PropertyField(globalPressure);
        EditorGUI.indentLevel--;

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Optimization");

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(workerThreads);
        EditorGUI.indentLevel--;

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (GUILayout.Button("Remove Scene Settings"))
        {
            EditorApplication.delayCall += () => Undo.DestroyObjectImmediate(m_targets[0].gameObject);
        }

        if (GUI.changed) serializedObject.ApplyModifiedProperties();
    }
}
