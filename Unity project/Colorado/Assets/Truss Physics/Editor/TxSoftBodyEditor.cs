/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(TxSoftBody))]
public class TxSoftBodyEditor : TxBodyEditor
{
    TxSoftBody[] m_targets;

    SerializedProperty truss;
    SerializedProperty collision;
    SerializedProperty margin;
    SerializedProperty matters;
    SerializedProperty skinning;
    SerializedProperty filling;
    SerializedProperty internalPressure;
    SerializedProperty adiabaticIndex;
    SerializedProperty spawnActive;
    SerializedProperty deactivation;
    SerializedProperty deactivationSpeed;
    SerializedProperty deactivationTime;
    SerializedProperty massScale;
    SerializedProperty fastRotation;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_targets = new TxSoftBody[targets.Length];
        for (int i = 0; i < targets.Length; ++i) m_targets[i] = (TxSoftBody)targets[i];

        truss = serializedObject.FindProperty("m_truss");
        collision = serializedObject.FindProperty("m_collision");
        margin = serializedObject.FindProperty("m_margin");
        matters = serializedObject.FindProperty("m_matters");
        skinning = serializedObject.FindProperty("m_skinning");
        filling = serializedObject.FindProperty("m_filling");
        internalPressure = serializedObject.FindProperty("m_internalPressure");
        adiabaticIndex = serializedObject.FindProperty("m_adiabaticIndex");
        spawnActive = serializedObject.FindProperty("m_spawnActive");
        deactivation = serializedObject.FindProperty("m_deactivation");
        deactivationSpeed = serializedObject.FindProperty("m_deactivationSpeed");
        deactivationTime = serializedObject.FindProperty("m_deactivationTime");
        massScale = serializedObject.FindProperty("m_massScale");
        fastRotation = serializedObject.FindProperty("m_fastRotation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TxEditor.LookLikeControls();

        bool isPlaying = Application.isPlaying;

        TxTrussDesigner trussDesigner = m_targets[0].GetComponent<TxTrussDesigner>();
        GUI.enabled = (trussDesigner == null && !Application.isPlaying) && !isPlaying;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(truss);
        GUI.enabled = (truss.objectReferenceValue != null && !Application.isPlaying);
        if (GUILayout.Button("Edit", (trussDesigner != null) ? TxEditor.MiniPressedStyle() : TxEditor.MiniUnpressedStyle(), GUILayout.MaxWidth(40)))
        {
            if (trussDesigner) EditorApplication.delayCall += ()=> Undo.DestroyObjectImmediate(trussDesigner);
            else Undo.AddComponent<TxTrussDesigner>(m_targets[0].gameObject);
        }
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;
        EditorGUILayout.PropertyField(massScale);
        if (truss.objectReferenceValue == null || truss.hasMultipleDifferentValues || massScale.hasMultipleDifferentValues)
        {
            GUI.enabled = false;
            EditorGUILayout.TextField("Scaled Mass", "-");
        }
        else
        {
            TxTruss trussAsset = truss.objectReferenceValue as TxTruss;
            if (trussAsset != null)
            {
                float scaledMass = 0;
                foreach (var m in trussAsset.nodeMass) scaledMass += m;
                GUI.enabled = false;
                EditorGUILayout.FloatField("Scaled Mass", scaledMass * massScale.floatValue);
            }
        }

        GUI.enabled = true && !isPlaying;
        EditorGUILayout.PropertyField(collision);

        EditorGUI.indentLevel++;
        GUI.enabled = collision.boolValue && !isPlaying;
        EditorGUILayout.PropertyField(margin);
        EditorGUILayout.PropertyField(matters, true);
        EditorGUI.indentLevel--;

        BodyGroupUI();

        GUI.enabled = true && !isPlaying;
        EditorGUILayout.PropertyField(skinning);

        GUI.enabled = true && !isPlaying;
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

        GUI.enabled = true;
        EditorGUILayout.PropertyField(filling);
        EditorGUI.indentLevel++;
        GUI.enabled = filling.boolValue;
        EditorGUILayout.PropertyField(internalPressure);
        EditorGUILayout.PropertyField(adiabaticIndex);
        EditorGUI.indentLevel--;

        GUI.enabled = true;
        EditorGUILayout.PropertyField(fastRotation);

        if (GUI.changed) serializedObject.ApplyModifiedProperties();
    }
}
