/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TxTrussDesigner))]
public class TxTrussDesignerEditor : Editor
{
    const int TRUSS = 0;
    const int NODES = 1;
    const int LINKS = 2;
    const int FACES = 3;

    TxTrussDesigner m_target;

    SerializedProperty subObjects;

    Quaternion m_selectionRotation = Quaternion.identity;
    Vector3 m_selectionScale = Vector3.one;

    bool m_regionSelection = false;
    Vector2 m_selectionStart = Vector2.zero;
    Rect m_selectionRect = new Rect(0, 0, 0, 0);

    Vector3 m_applyTranslation = Vector3.zero;
    Vector3 m_applyRotation = Vector3.zero;
    Vector3 m_applyScale = Vector3.one;
    bool m_transformClone = false;

    public string m_dummyProp = "";
    public bool m_dummyBool = false;

    [NonSerialized]
    bool m_creatingNode = false;
    [NonSerialized]
    int[] m_creatingLink = null;
    [NonSerialized]
    int[] m_creatingFace = null;

    string m_nodesSet = "";
    string m_linksSet = "";
    string m_facesSet = "";

    int createFromMeshControlID = -1;

    protected virtual void OnEnable()
    {
        m_target = (TxTrussDesigner)target;
        subObjects = serializedObject.FindProperty("subObjects");
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

        EditorGUILayout.Separator();

        string[] tabs = { "Truss", "Nodes", "Links", "Faces" };
        subObjects.intValue = GUILayout.Toolbar(subObjects.intValue, tabs);
        TxEditor.HideHandles(subObjects.intValue != TRUSS);
        switch (subObjects.intValue)
        {
            case TRUSS:
                {
                    EditorGUILayout.Separator();

                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("Truss", m_target.trussData, typeof(TxTruss), false);
                    EditorGUILayout.IntField("Nodes", m_target.nodes.Length);
                    EditorGUILayout.IntField("Links", m_target.links.Length);
                    EditorGUILayout.IntField("Faces", m_target.faces.Length);
                    GUI.enabled = true;

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Create From Mesh..."))
                    {
                        createFromMeshControlID = UnityEngine.Random.Range(0, 1000);
                        EditorGUIUtility.ShowObjectPicker<Mesh>(null, true, "", createFromMeshControlID);
                    }
                    string commandName = Event.current.commandName;
                    if (commandName == "ObjectSelectorClosed" && createFromMeshControlID != -1 && EditorGUIUtility.GetObjectPickerControlID() == createFromMeshControlID)
                    {
                        Mesh selectedMesh = (Mesh)EditorGUIUtility.GetObjectPickerObject();
                        if (selectedMesh != null)
                        {
                            Undo.RecordObject(m_target, "Create From Mesh");
                            CreateFromMesh(selectedMesh);
                        }
                        createFromMeshControlID = -1;
                    }

                    if (GUILayout.Button("Optimize For SIMD"))
                    {
                        Undo.RecordObject(m_target, "Optimize For SIMD");
                        m_target.Optimize();
                        EditorUtility.SetDirty(m_target);
                    }

                    m_creatingNode = false;
                    m_creatingLink = null;
                    m_creatingFace = null;
                }
                break;
            case NODES:
                {
                    TxNodeHandle[] selectedNodes = m_target.selectedNodes;
                    string selectedNodesLabel = string.Format("{0} Nodes Selected", selectedNodes.Length);
                    if (selectedNodes.Length == 0) selectedNodesLabel = "No Nodes Selected";
                    if (selectedNodes.Length == 1) selectedNodesLabel = string.Format("Node {0} Selected", selectedNodes[0].index);
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.alignment = TextAnchor.MiddleRight;
                    EditorGUILayout.LabelField(selectedNodesLabel, style);
                    if (selectedNodes.Length > 0)
                    {
                        SerializedObject serializedNodes = new SerializedObject(selectedNodes);
                        SerializedProperty position = serializedNodes.FindProperty("position");
                        EditorGUILayout.PropertyField(position, new GUIContent("Local Position"));
                        SerializedProperty mass = serializedNodes.FindProperty("mass");
                        EditorGUILayout.PropertyField(mass, new GUIContent("Node Mass"));
                        float selectionMass = 0; foreach (TxNodeHandle n in selectedNodes) selectionMass += n.mass;
                        float newSelectionMass = EditorGUILayout.FloatField("Selection Mass", selectionMass);
                        if (newSelectionMass != selectionMass)
                        {
                            if (selectionMass > 0) foreach (TxNodeHandle n in selectedNodes) n.mass *= newSelectionMass / selectionMass;
                            else foreach (TxNodeHandle n in selectedNodes) n.mass = newSelectionMass / selectedNodes.Length;
                        }
                        if (GUI.changed) serializedNodes.ApplyModifiedProperties();
                    }
                    else
                    {
                        GUI.enabled = false;
                        SerializedObject serializedDummy = new SerializedObject(this);
                        SerializedProperty dummy = serializedDummy.FindProperty("m_dummyProp");
                        EditorGUILayout.PropertyField(dummy, new GUIContent("Local Position"));
                        EditorGUILayout.PropertyField(dummy, new GUIContent("Node Mass"));
                        EditorGUILayout.TextField("Selection Mass", "");
                    }

                    EditorGUILayout.Separator();

                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = true;
                    if (GUILayout.Button("Create", (m_creatingNode) ? TxEditor.PressedStyle() : TxEditor.UnpressedStyle()))
                    {
                        m_creatingNode = !m_creatingNode;
                    }
                    GUI.enabled = (selectedNodes.Length > 0);
                    if (GUILayout.Button("Hide"))
                    {
                        Undo.RecordObjects(selectedNodes, "Hide Truss Nodes");
                        HideSelection();
                        EditorUtility.SetDirty(m_target);
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button("Unhide All"))
                    {
                        Undo.RecordObjects(m_target.nodes, "Unhide Truss Nodes");
                        if (UnhideAll())
                        {
                            EditorUtility.SetDirty(m_target);
                        }
                    }
                    GUI.enabled = (selectedNodes.Length > 0);
                    if (GUILayout.Button("Delete"))
                    {
                        DeleteSelection();
                        EditorUtility.SetDirty(m_target);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Separator();

                    NamedSets(ref m_nodesSet, selectedNodes.Length > 0);

                    EditorGUILayout.Separator();

                    GUI.enabled = (selectedNodes.Length > 0);
                    EditorGUILayout.LabelField("Transform Selection");
                    EditorGUI.indentLevel++;
                    GUI.enabled = (selectedNodes.Length > 0);
                    m_applyTranslation = EditorGUILayout.Vector3Field("Translate", m_applyTranslation);
                    m_applyRotation = EditorGUILayout.Vector3Field("Rotate", m_applyRotation);
                    m_applyScale = EditorGUILayout.Vector3Field("Scale", m_applyScale);
                    m_transformClone = EditorGUILayout.Toggle("Clone", m_transformClone);
                    EditorGUI.indentLevel--;
                    if (GUILayout.Button("Apply Transformation"))
                    {
                        if (m_transformClone)
                        {
                            TxNodeHandle[] nodes = m_target.selectedNodes;
                            CloneNodes(nodes);
                            foreach (TxNodeHandle n in nodes) n.selected = false;
                        }
                        TransformSelection(Matrix4x4.TRS(m_applyTranslation, Quaternion.Euler(m_applyRotation), m_applyScale));
                    }

                    EditorGUILayout.Separator();

                    m_creatingLink = null;
                    m_creatingFace = null;
                }
                break;
            case LINKS:
                {
                    TxLinkHandle[] selectedLinks = m_target.selectedLinks;
                    string selectedLinksLabel = string.Format("{0} Links Selected", selectedLinks.Length);
                    if (selectedLinks.Length == 0) selectedLinksLabel = "No Links Selected";
                    if (selectedLinks.Length == 1) selectedLinksLabel = string.Format("Link {0} Selected", selectedLinks[0].index);
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.alignment = TextAnchor.MiddleRight;
                    EditorGUILayout.LabelField(selectedLinksLabel, style);
                    if (selectedLinks.Length > 0)
                    {
                        SerializedObject serializedLinks = new SerializedObject(selectedLinks);
                        SerializedProperty stiffness = serializedLinks.FindProperty("stiffness");
                        EditorGUILayout.PropertyField(stiffness, new GUIContent("Spring Stiffness"));
                        SerializedProperty damping = serializedLinks.FindProperty("damping");
                        EditorGUILayout.PropertyField(damping, new GUIContent("Spring Damping"));
                        SerializedProperty elastic = serializedLinks.FindProperty("elastic");
                        EditorGUILayout.PropertyField(elastic, new GUIContent("Elastic Deformation"));
                        SerializedProperty stretching = serializedLinks.FindProperty("stretching");
                        EditorGUILayout.PropertyField(stretching, new GUIContent("Stretching Limit"));
                        if (GUI.changed) serializedLinks.ApplyModifiedProperties();
                    }
                    else
                    {
                        GUI.enabled = false;
                        SerializedObject serializedDummy = new SerializedObject(this);
                        SerializedProperty dummy = serializedDummy.FindProperty("m_dummyProp");
                        EditorGUILayout.PropertyField(dummy, new GUIContent("Spring Stiffness"));
                        EditorGUILayout.PropertyField(dummy, new GUIContent("Spring Damping"));
                        EditorGUILayout.PropertyField(dummy, new GUIContent("Elastic Deformation"));
                        EditorGUILayout.PropertyField(dummy, new GUIContent("Stretching Limit"));
                    }

                    EditorGUILayout.Separator();

                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = true;
                    if (GUILayout.Button("Create", (m_creatingLink != null) ? TxEditor.PressedStyle() : TxEditor.UnpressedStyle()))
                    {
                        if (m_creatingLink != null) m_creatingLink = null;
                        else m_creatingLink = new int[0];
                        EditorUtility.SetDirty(m_target);
                    }
                    GUI.enabled = (selectedLinks.Length > 0);
                    if (GUILayout.Button("Hide"))
                    {
                        Undo.RecordObjects(selectedLinks, "Hide Truss Links");
                        HideSelection();
                        EditorUtility.SetDirty(m_target);
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button("Unhide All"))
                    {
                        Undo.RecordObjects(m_target.nodes, "Unhide Truss Links");
                        if (UnhideAll())
                        {
                            EditorUtility.SetDirty(m_target);
                        }
                    }
                    GUI.enabled = (selectedLinks.Length > 0);
                    if (GUILayout.Button("Delete"))
                    {
                        DeleteSelection();
                        EditorUtility.SetDirty(m_target);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Separator();

                    NamedSets(ref m_linksSet, selectedLinks.Length > 0);

                    EditorGUILayout.Separator();

                    m_creatingNode = false;
                    m_creatingFace = null;
                }
                break;
            case FACES:
                {
                    TxFaceHandle[] selectedFaces = m_target.selectedFaces;
                    string selectedFacesLabel = string.Format("{0} Faces Selected", selectedFaces.Length);
                    if (selectedFaces.Length == 0) selectedFacesLabel = "No Faces Selected";
                    if (selectedFaces.Length == 1) selectedFacesLabel = string.Format("Face {0} Selected", selectedFaces[0].index);
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.alignment = TextAnchor.MiddleRight;
                    EditorGUILayout.LabelField(selectedFacesLabel, style);
                    if (selectedFaces.Length > 0)
                    {
                        SerializedObject serializedFaces = new SerializedObject(selectedFaces);
                        SerializedProperty matter = serializedFaces.FindProperty("matter");
                        EditorGUILayout.PropertyField(matter, new GUIContent("Matter Index"));
                        SerializedProperty envelope = serializedFaces.FindProperty("envelope");
                        EditorGUILayout.PropertyField(envelope, new GUIContent("Skinning Envelope"));
                        if (GUI.changed) serializedFaces.ApplyModifiedProperties();
                    }
                    else
                    {
                        GUI.enabled = false;
                        SerializedObject serializedDummy = new SerializedObject(this);
                        SerializedProperty dummy = serializedDummy.FindProperty("m_dummyProp");
                        EditorGUILayout.PropertyField(dummy, new GUIContent("Matter Index"));
                        EditorGUILayout.PropertyField(dummy, new GUIContent("Skinning Envelope"));
                    }

                    EditorGUILayout.Separator();

                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = true;
                    if (GUILayout.Button("Create", (m_creatingFace != null) ? TxEditor.PressedStyle() : TxEditor.UnpressedStyle()))
                    {
                        if (m_creatingFace != null) m_creatingFace = null;
                        else m_creatingFace = new int[0];
                        EditorUtility.SetDirty(m_target);
                    }
                    GUI.enabled = (selectedFaces.Length > 0);
                    if (GUILayout.Button("Hide"))
                    {
                        Undo.RecordObjects(selectedFaces, "Hide Truss Faces");
                        HideSelection();
                        EditorUtility.SetDirty(m_target);
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button("Unhide All"))
                    {
                        Undo.RecordObjects(m_target.nodes, "Unhide Truss Faces");
                        if (UnhideAll())
                        {
                            EditorUtility.SetDirty(m_target);
                        }
                    }
                    GUI.enabled = (selectedFaces.Length > 0);
                    if (GUILayout.Button("Delete"))
                    {
                        DeleteSelection();
                        EditorUtility.SetDirty(m_target);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Separator();

                    NamedSets(ref m_facesSet, selectedFaces.Length > 0);

                    EditorGUILayout.Separator();

                    GUI.enabled = (selectedFaces.Length > 0);
                    if (GUILayout.Button("Flip"))
                    {
                        Undo.RecordObjects(selectedFaces, "Flip Truss Faces");
                        for (int i = 0; i < selectedFaces.Length; ++i)
                        {
                            TxFaceHandle face = selectedFaces[i];
                            int tmp = face.node0;
                            face.node0 = face.node1;
                            face.node1 = tmp;
                        }
                        EditorUtility.SetDirty(m_target);
                    }

                    m_creatingNode = false;
                    m_creatingLink = null;
                }
                break;
        }

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(m_target.trussData);
        }
    }

    void OnSceneGUI()
    {
        if (subObjects.intValue == TRUSS) return;

        int controID = GUIUtility.GetControlID(FocusType.Keyboard);
        //
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
        {
            DeleteSelection();
            Event.current.Use();
        }
        //
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(controID);
        }
        else if (Event.current.type == EventType.Repaint)
        {
            DrawHandles();
        }
        else
        {
            if (HandleUtility.nearestControl == controID)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    PointSelect();
                }
                else if (Event.current.type == EventType.mouseDrag)
                {
                    if (m_regionSelection)
                    {
                        m_selectionRect.xMin = Mathf.Min(m_selectionStart.x, Event.current.mousePosition.x);
                        m_selectionRect.xMax = Mathf.Max(m_selectionStart.x, Event.current.mousePosition.x);
                        m_selectionRect.yMin = Mathf.Min(m_selectionStart.y, Event.current.mousePosition.y);
                        m_selectionRect.yMax = Mathf.Max(m_selectionStart.y, Event.current.mousePosition.y);
                        HandleUtility.Repaint();
                    }
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    RegionSelect();
                }
            }
        }

        if (m_creatingNode)
        {
        }

        if (m_creatingLink != null)
        {
            for (int i = 0; i < m_target.nodes.Length; ++i)
            {
                TxNodeHandle node = m_target.nodes[i];
                if (node.hidden) continue;
                Vector3 position = m_target.transform.TransformPoint(node.position);
                Handles.color = Color.blue;
                float size = HandleUtility.GetHandleSize(position) * 0.08f;
                Handles.SphereCap(-1, position, Quaternion.identity, size);
            }
            if (m_creatingLink.Length == 1)
            {
                Vector3 nodePos = m_target.transform.TransformPoint(m_target.nodes[m_creatingLink[0]].position);
                Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                Vector3 mousePos = mouseRay.origin + mouseRay.direction * Vector3.Dot(mouseRay.direction, nodePos - mouseRay.origin);
                Handles.color = Color.red;
                Handles.DrawLine(nodePos, mousePos);
            }
            HandleUtility.Repaint();
        }

        if (m_creatingFace != null)
        {
            for (int i = 0; i < m_target.nodes.Length; ++i)
            {
                TxNodeHandle node = m_target.nodes[i];
                if (node.hidden) continue;
                Vector3 position = m_target.transform.TransformPoint(node.position);
                Handles.color = Color.blue;
                float size = HandleUtility.GetHandleSize(position) * 0.08f;
                Handles.SphereCap(-1, position, Quaternion.identity, size);
            }
            if (m_creatingFace.Length > 0)
            {
                Vector3 nodePos = m_target.transform.TransformPoint(m_target.nodes[m_creatingFace[0]].position);
                Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                Vector3 mousePos = mouseRay.origin + mouseRay.direction * Vector3.Dot(mouseRay.direction, nodePos - mouseRay.origin);
                Vector3[] verts = { nodePos, mousePos, mousePos, mousePos };
                if (m_creatingFace.Length > 1)
                {
                    verts[1] = m_target.transform.TransformPoint(m_target.nodes[m_creatingFace[1]].position);
                }
                Handles.color = Color.red;
                Handles.DrawSolidRectangleWithOutline(verts, new Color(1.0f, 0.0f, 0.0f, 0.1f), Color.red);
            }
            HandleUtility.Repaint();
        }

        if (!m_regionSelection && !m_creatingNode && m_creatingLink == null && m_creatingFace == null && !SelectionEmpty()) TransformSelection(SelectionTransform());
        if (GUI.changed) EditorUtility.SetDirty(target);
    }

    bool SelectionEmpty()
    {
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    foreach (TxNodeHandle n in m_target.nodes) if (n.selected) return false;
                }
                break;
            case LINKS:
                {
                    foreach (TxLinkHandle l in m_target.links) if (l.selected) return false;
                }
                break;
            case FACES:
                {
                    foreach (TxFaceHandle f in m_target.faces) if (f.selected) return false;
                }
                break;
        }
        return true;
    }

    void PointSelect()
    {
        if (Event.current.alt || Event.current.button == 2 || Tools.current == Tool.View) return;

        bool changed = false, hit = false;

        switch (subObjects.intValue)
        {
            case NODES:
                {
                    if (m_creatingNode)
                    {
                        Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        Vector3 position;
                        if (Mathf.Abs(mouseRay.direction.x) > Mathf.Abs(mouseRay.direction.y))
                        {
                            if (Mathf.Abs(mouseRay.direction.x) > Mathf.Abs(mouseRay.direction.z)) position = mouseRay.origin - mouseRay.direction * mouseRay.origin.x / mouseRay.direction.x;
                            else position = mouseRay.origin - mouseRay.direction * mouseRay.origin.z / mouseRay.direction.z;
                        }
                        else if (Mathf.Abs(mouseRay.direction.y) > Mathf.Abs(mouseRay.direction.z)) position = mouseRay.origin - mouseRay.direction * mouseRay.origin.y / mouseRay.direction.y;
                        else position = mouseRay.origin - mouseRay.direction * mouseRay.origin.z / mouseRay.direction.z;
                        CreateNode(m_target.transform.InverseTransformPoint(position));
                        return;
                    }

                    TxNodeHandle[] nodes = m_target.nodes;
                    Undo.RecordObjects(nodes, "Truss Nodes Selection");
                    for (int i = 0; i < nodes.Length; ++i)
                    {
                        TxNodeHandle node = nodes[i];
                        if (node.hidden) continue;
                        if (node.selected && !Event.current.control)
                        {
                            node.selected = false;
                            changed = true;
                        }
                        Vector3 position = m_target.transform.TransformPoint(node.position);
                        float size = HandleUtility.GetHandleSize(position) * 0.08f;
                        if (!hit && HandleUtility.DistanceToCircle(position, size) <= 0)
                        {
                            if (Event.current.control) node.selected = !node.selected;
                            else node.selected = true;
                            changed = hit = true;
                        }
                    }
                }
                break;
            case LINKS:
                {
                    if (m_creatingLink != null)
                    {
                        for (int i = 0; i < m_target.nodes.Length; ++i)
                        {
                            TxNodeHandle node = m_target.nodes[i];
                            if (node.hidden) continue;
                            Vector3 position = m_target.transform.TransformPoint(node.position);
                            Handles.color = Color.blue;
                            float size = HandleUtility.GetHandleSize(position) * 0.08f;
                            if (HandleUtility.DistanceToCircle(position, size) <= 0)
                            {
                                switch (m_creatingLink.Length)
                                {
                                    case 0:
                                        System.Array.Resize(ref m_creatingLink, 1);
                                        m_creatingLink[0] = i;
                                        break;
                                    case 1:
                                        if (m_creatingLink[0] != i)
                                            CreateLink(m_creatingLink[0], i);
                                        break;
                                }
                            }
                        }
                        return;
                    }

                    TxLinkHandle[] links = m_target.links;
                    Undo.RecordObjects(links, "Truss Links Selection");
                    TxNodeHandle[] nodes = m_target.nodes;
                    for (int i = 0; i < links.Length; ++i)
                    {
                        TxLinkHandle link = links[i];
                        if (link.hidden) continue;
                        if (link.selected && !Event.current.control)
                        {
                            link.selected = false;
                            changed = true;
                        }
                        Vector3 pos0 = m_target.transform.TransformPoint(nodes[link.node0].position);
                        Vector3 pos1 = m_target.transform.TransformPoint(nodes[link.node1].position);
                        if (!hit && HandleUtility.DistancePointToLineSegment(Event.current.mousePosition, HandleUtility.WorldToGUIPoint(pos0), HandleUtility.WorldToGUIPoint(pos1)) < 3.0f)
                        {
                            if (Event.current.control) link.selected = !link.selected;
                            else link.selected = true;
                            changed = hit = true;
                        }
                    }
                }
                break;
            case FACES:
                {
                    if (m_creatingFace != null)
                    {
                        for (int i = 0; i < m_target.nodes.Length; ++i)
                        {
                            TxNodeHandle node = m_target.nodes[i];
                            if (node.hidden) continue;
                            Vector3 position = m_target.transform.TransformPoint(node.position);
                            Handles.color = Color.blue;
                            float size = HandleUtility.GetHandleSize(position) * 0.08f;
                            if (HandleUtility.DistanceToCircle(position, size) <= 0)
                            {
                                switch (m_creatingFace.Length)
                                {
                                    case 0:
                                        System.Array.Resize(ref m_creatingFace, 1);
                                        m_creatingFace[0] = i;
                                        break;
                                    case 1:
                                        if (m_creatingFace[0] != i)
                                        {
                                            System.Array.Resize(ref m_creatingFace, 2);
                                            m_creatingFace[1] = i;
                                        }
                                        break;
                                    case 2:
                                        if (m_creatingFace[0] != i && m_creatingFace[1] != i)
                                        {
                                            CreateFace(m_creatingFace[0], m_creatingFace[1], i);
                                        }
                                        break;
                                }
                            }
                        }
                        return;
                    }

                    TxFaceHandle[] faces = m_target.faces;
                    Undo.RecordObjects(faces, "Truss Faces Selection");
                    TxNodeHandle[] nodes = m_target.nodes;
                    for (int i = 0; i < faces.Length; ++i)
                    {
                        TxFaceHandle face = faces[i];
                        if (face.hidden) continue;
                        if (face.selected && !Event.current.control)
                        {
                            face.selected = false;
                            changed = true;
                        }
                        Vector3 pos0 = m_target.transform.TransformPoint(nodes[face.node0].position);
                        Vector3 pos1 = m_target.transform.TransformPoint(nodes[face.node1].position);
                        Vector3 pos2 = m_target.transform.TransformPoint(nodes[face.node2].position);
                        if (!hit && TxEditor.RayTriangleTest(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), pos0, pos1, pos2))
                        {
                            if (Event.current.control) face.selected = !face.selected;
                            else face.selected = true;
                            changed = hit = true;
                        }
                    }
                }
                break;
        }

        if (changed)
        {
            EditorUtility.SetDirty(target);
            HandleUtility.Repaint();
        }

        if (!hit)
        {
            m_selectionStart = Event.current.mousePosition;
            m_selectionRect.width = m_selectionRect.height = 0;
            m_regionSelection = true;
        }
    }

    void RegionSelect()
    {
        if (!m_regionSelection) return;
        bool changed = false;
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    TxNodeHandle[] nodes = m_target.nodes;
                    Undo.RecordObjects(nodes, "Truss Nodes Selection");
                    for (int i = 0; i < nodes.Length; ++i)
                    {
                        TxNodeHandle node = nodes[i];
                        if (node.hidden) continue;
                        Vector3 position = m_target.transform.TransformPoint(node.position);
                        if (m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(position)))
                        {
                            if (!Event.current.control)
                            {
                                if (!node.selected)
                                {
                                    node.selected = true;
                                    changed = true;
                                }
                            }
                            else
                            {
                                node.selected = !node.selected;
                                changed = true;
                            }
                        }
                    }
                }
                break;
            case LINKS:
                {
                    TxLinkHandle[] links = m_target.links;
                    Undo.RecordObjects(links, "Truss Links Selection");
                    TxNodeHandle[] nodes = m_target.nodes;
                    for (int i = 0; i < links.Length; ++i)
                    {
                        TxLinkHandle link = links[i];
                        if (link.hidden) continue;
                        Vector3 pos0 = m_target.transform.TransformPoint(nodes[link.node0].position);
                        Vector3 pos1 = m_target.transform.TransformPoint(nodes[link.node1].position);
                        if (m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos0)) && m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos1)))
                        {
                            if (!Event.current.control)
                            {
                                if (!link.selected)
                                {
                                    link.selected = true;
                                    changed = true;
                                }
                            }
                            else
                            {
                                link.selected = !link.selected;
                                changed = true;
                            }
                        }
                    }
                }
                break;
            case FACES:
                {
                    TxFaceHandle[] faces = m_target.faces;
                    Undo.RecordObjects(faces, "Truss Faces Selection");
                    TxNodeHandle[] nodes = m_target.nodes;
                    for (int i = 0; i < faces.Length; ++i)
                    {
                        TxFaceHandle face = faces[i];
                        if (face.hidden) continue;
                        Vector3 pos0 = m_target.transform.TransformPoint(nodes[face.node0].position);
                        Vector3 pos1 = m_target.transform.TransformPoint(nodes[face.node1].position);
                        Vector3 pos2 = m_target.transform.TransformPoint(nodes[face.node2].position);
                        if (m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos0)) && m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos1)) && m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos2)))
                        {
                            if (!Event.current.control)
                            {
                                if (!face.selected)
                                {
                                    face.selected = true;
                                    changed = true;
                                }
                            }
                            else
                            {
                                face.selected = !face.selected;
                                changed = true;
                            }
                        }
                    }
                }
                break;
        }
        if (changed)
        {
            EditorUtility.SetDirty(target);
        }
        m_regionSelection = false;
        HandleUtility.Repaint();
    }

    Vector3 SelectionCentre()
    {
        Vector3 centre = Vector3.zero;
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    TxNodeHandle[] nodes = m_target.selectedNodes;
                    if (nodes.Length > 0)
                    {
                        for (int i = 0; i < nodes.Length; ++i)
                        {
                            TxNodeHandle node = nodes[i];
                            centre += node.position;
                        }
                        centre /= nodes.Length;
                    }
                }
                break;
            case LINKS:
                {
                    TxNodeHandle[] nodes = m_target.selectedLinksNodes;
                    if (nodes.Length > 0)
                    {
                        for (int i = 0; i < nodes.Length; ++i)
                        {
                            TxNodeHandle node = nodes[i];
                            centre += node.position;
                        }
                        centre /= nodes.Length;
                    }
                }
                break;
            case FACES:
                {
                    TxNodeHandle[] nodes = m_target.selectedFacesNodes;
                    if (nodes.Length > 0)
                    {
                        for (int i = 0; i < nodes.Length; ++i)
                        {
                            TxNodeHandle node = nodes[i];
                            centre += node.position;
                        }
                        centre /= nodes.Length;
                    }
                }
                break;
        }
        return centre;
    }

    Matrix4x4 SelectionTransform()
    {
        if (Event.current.type == EventType.MouseUp)
        {
            m_selectionRotation = Quaternion.identity;
            m_selectionScale = Vector3.one;
        }
        Vector3 position = m_target.transform.TransformPoint(SelectionCentre());
        Quaternion rotation = (Tools.pivotRotation == PivotRotation.Local) ? m_target.transform.rotation : Quaternion.identity;
        Matrix4x4 transform = Matrix4x4.identity;
        switch (Tools.current)
        {
            case Tool.Move:
                {
                    Vector3 newPosition = Handles.PositionHandle(position, rotation);
                    if (GUI.changed)
                    {
                        transform = m_target.transform.worldToLocalMatrix
                                    * Matrix4x4.TRS(newPosition - position, Quaternion.identity, Vector3.one)
                                    * m_target.transform.localToWorldMatrix;
                    }
                }
                break;
            case Tool.Rotate:
                {
                    Quaternion newRotation = Handles.RotationHandle(m_selectionRotation * rotation, position);
                    if (GUI.changed)
                    {
                        transform = m_target.transform.worldToLocalMatrix
                                    * Matrix4x4.TRS(position, Quaternion.identity, Vector3.one)
                                    * Matrix4x4.TRS(Vector3.zero, newRotation * Quaternion.Inverse(m_selectionRotation * rotation), Vector3.one)
                                    * Matrix4x4.TRS(-position, Quaternion.identity, Vector3.one)
                                    * m_target.transform.localToWorldMatrix;
                        m_selectionRotation = newRotation * Quaternion.Inverse(rotation);
                    }
                }
                break;
            case Tool.Scale:
                {
                    Vector3 newScale = Handles.ScaleHandle(m_selectionScale, position, m_target.transform.rotation, HandleUtility.GetHandleSize(position));
                    if (GUI.changed)
                    {
                        transform = Matrix4x4.TRS(m_target.transform.InverseTransformPoint(position), Quaternion.identity, Vector3.one)
                                    * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(newScale.x / m_selectionScale.x, newScale.y / m_selectionScale.y, newScale.z / m_selectionScale.z))
                                    * Matrix4x4.TRS(-m_target.transform.InverseTransformPoint(position), Quaternion.identity, Vector3.one);
                        m_selectionScale = newScale;
                    }
                }
                break;
        }
        return transform;
    }

    void TransformSelection(Matrix4x4 _transform)
    {
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    TxNodeHandle[] nodes = m_target.selectedNodes;
                    if (nodes.Length > 0)
                    {
                        if (GUI.changed)
                        {
                            Undo.RecordObjects(nodes, "Truss Nodes Transform");
                            for (int i = 0; i < nodes.Length; ++i)
                            {
                                TxNodeHandle node = nodes[i];
                                node.position = _transform.MultiplyPoint(node.position);
                            }
                        }
                    }
                }
                break;
            case LINKS:
                {
                    TxNodeHandle[] nodes = m_target.selectedLinksNodes;
                    if (nodes.Length > 0)
                    {
                        if (GUI.changed)
                        {
                            Undo.RecordObjects(nodes, "Truss Links Transform");
                            for (int i = 0; i < nodes.Length; ++i)
                            {
                                TxNodeHandle node = nodes[i];
                                node.position = _transform.MultiplyPoint(node.position);
                            }
                        }
                    }
                }
                break;
            case FACES:
                {
                    TxNodeHandle[] nodes = m_target.selectedFacesNodes;
                    if (nodes.Length > 0)
                    {
                        if (GUI.changed)
                        {
                            Undo.RecordObjects(nodes, "Truss Faces Transform");
                            for (int i = 0; i < nodes.Length; ++i)
                            {
                                TxNodeHandle node = nodes[i];
                                node.position = _transform.MultiplyPoint(node.position);
                            }
                        }
                    }
                }
                break;
        }
    }

    void DrawHandles()
    {
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    TxNodeHandle[] nodes = m_target.nodes;
                    for (int i = 0; i < nodes.Length; ++i)
                    {
                        TxNodeHandle node = nodes[i];
                        if (node.hidden) continue;
                        Vector3 position = m_target.transform.TransformPoint(node.position);
                        bool selected = node.selected;
                        if (m_regionSelection && m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(position))) selected = !selected;
                        Handles.color = selected ? Color.red : Color.blue;
                        float size = HandleUtility.GetHandleSize(position) * 0.08f;
                        Handles.SphereCap(-1, position, Quaternion.identity, size);
                    }
                }
                break;
            case LINKS:
                {
                    TxNodeHandle[] nodes = m_target.nodes;
                    TxLinkHandle[] links = m_target.links;
                    for (int i = 0; i < links.Length; ++i)
                    {
                        TxLinkHandle link = links[i];
                        if (link.hidden) continue;
                        TxNodeHandle node0 = nodes[link.node0];
                        TxNodeHandle node1 = nodes[link.node1];
                        Vector3 pos0 = m_target.transform.TransformPoint(node0.position);
                        Vector3 pos1 = m_target.transform.TransformPoint(node1.position);
                        bool selected = link.selected;
                        if (m_regionSelection && m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos0)) && m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos1))) selected = !selected;
                        Handles.color = selected ? Color.red : Color.white;
                        Handles.DrawLine(pos0, pos1);
                    }
                }
                break;
            case FACES:
                {
                    TxNodeHandle[] nodes = m_target.nodes;
                    TxFaceHandle[] faces = m_target.faces;
                    for (int i = 0; i < faces.Length; ++i)
                    {
                        TxFaceHandle face = faces[i];
                        if (face.hidden) continue;
                        TxNodeHandle node0 = nodes[face.node0];
                        TxNodeHandle node1 = nodes[face.node1];
                        TxNodeHandle node2 = nodes[face.node2];
                        Vector3 pos0 = m_target.transform.TransformPoint(node0.position);
                        Vector3 pos1 = m_target.transform.TransformPoint(node1.position);
                        Vector3 pos2 = m_target.transform.TransformPoint(node2.position);
                        Vector3[] verts = { pos0, pos1, pos2, pos2 };
                        bool selected = face.selected;
                        if (m_regionSelection && m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos0)) && m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos1)) && m_selectionRect.Contains(HandleUtility.WorldToGUIPoint(pos2))) selected = !selected;
                        if (selected) Handles.DrawSolidRectangleWithOutline(verts, new Color(1.0f, 0.0f, 0.0f, 0.1f), Color.red);
                        else Handles.DrawSolidRectangleWithOutline(verts, new Color(1.0f, 1.0f, 1.0f, 0.1f), Color.white);
                        Vector3 centre = (pos0 + pos1 + pos2) / 3;
                        Vector3 normal = Vector3.Cross(pos1 - pos0, pos2 - pos0).normalized;
                        float size = HandleUtility.GetHandleSize(centre) * 0.2f;
                        Handles.color = Color.blue;
                        Handles.DrawLine(centre, centre + normal * size);
                        Handles.color = Color.white;
                    }
                }
                break;
        }
        if (m_regionSelection && m_selectionRect.width > 0 && m_selectionRect.height > 0)
        {
            Handles.BeginGUI();
            GUI.Box(m_selectionRect, new GUIContent(), TxEditor.SelectionRectStyle());
            Handles.EndGUI();
        }
    }

    bool HideSelection()
    {
        bool changed = false;
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    TxNodeHandle[] nodes = m_target.selectedNodes;
                    for (int i = 0; i < nodes.Length; ++i)
                    {
                        TxNodeHandle node = nodes[i];
                        node.hidden = true;
                        node.selected = false;
                        changed = true;
                    }
                }
                break;
            case LINKS:
                {
                    TxLinkHandle[] links = m_target.selectedLinks;
                    for (int i = 0; i < links.Length; ++i)
                    {
                        TxLinkHandle link = links[i];
                        link.hidden = true;
                        link.selected = false;
                        changed = true;
                    }
                }
                break;
            case FACES:
                {
                    TxFaceHandle[] faces = m_target.selectedFaces;
                    for (int i = 0; i < faces.Length; ++i)
                    {
                        TxFaceHandle face = faces[i];
                        face.hidden = true;
                        face.selected = false;
                        changed = true;
                    }
                }
                break;
        }
        return changed;
    }

    bool UnhideAll()
    {
        bool changed = false;
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    TxNodeHandle[] nodes = m_target.nodes;
                    for (int i = 0; i < nodes.Length; ++i)
                    {
                        TxNodeHandle node = nodes[i];
                        if (node.selected)
                        {
                            node.selected = false;
                            changed = true;
                        }
                        if (node.hidden)
                        {
                            node.hidden = false;
                            node.selected = true;
                            changed = true;
                        }
                    }
                }
                break;
            case LINKS:
                {
                    TxLinkHandle[] links = m_target.links;
                    for (int i = 0; i < links.Length; ++i)
                    {
                        TxLinkHandle link = links[i];
                        if (link.selected)
                        {
                            link.selected = false;
                            changed = true;
                        }
                        if (link.hidden)
                        {
                            link.hidden = false;
                            link.selected = true;
                            changed = true;
                        }
                    }
                }
                break;
            case FACES:
                {
                    TxFaceHandle[] faces = m_target.faces;
                    for (int i = 0; i < faces.Length; ++i)
                    {
                        TxFaceHandle face = faces[i];
                        if (face.selected)
                        {
                            face.selected = false;
                            changed = true;
                        }
                        if (face.hidden)
                        {
                            face.hidden = false;
                            face.selected = true;
                            changed = true;
                        }
                    }
                }
                break;
        }
        return changed;
    }

    TxNodeHandle CreateNode()
    {
        return CreateNode(Vector3.zero);
    }
    TxNodeHandle CreateNode(Vector3 _position)
    {
        Undo.RecordObject(m_target, "Create Truss Node");
        TxNodeHandle node = m_target.CreateNode(_position, 1.0f);
        Undo.RegisterCreatedObjectUndo(node, "Create Truss Node");
        node.selected = true;
        return node;
    }

    TxLinkHandle CreateLink(int _node0, int _node1)
    {
        Undo.RecordObject(m_target, "Create Truss Link");
        TxLinkHandle link = m_target.CreateLink(_node0, _node1);
        Undo.RegisterCreatedObjectUndo(link, "Create Truss Link");
        link.selected = true;
        m_creatingLink = new int[0];
        return link;
    }

    TxFaceHandle CreateFace(int _node0, int _node1, int _node2)
    {
        Undo.RecordObject(m_target, "Create Truss Face");
        TxFaceHandle face = m_target.CreateFace(_node0, _node1, _node2);
        Undo.RegisterCreatedObjectUndo(face, "Create Truss Face");
        face.selected = true;
        m_creatingFace = new int[0];
        return face;
    }

    void CreateFromMesh(Mesh _mesh)
    {
        Undo.RecordObject(m_target, "Create Truss From Mesh");
        m_target.CreateFromMesh(_mesh);
    }

    void DeleteSelection()
    {
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    TxNodeHandle[] nodes = m_target.selectedNodes;
                    if (nodes.Length > 0)
                    {
                        Undo.RecordObject(m_target, "Delete Truss Nodes");
                        for (int i = nodes.Length - 1; i >= 0; --i)
                        {
                            if (nodes[i].selected)
                            {
                                m_target.DeleteNode(nodes[i].index, o => { EditorApplication.delayCall += () => Undo.DestroyObjectImmediate(o); });
                            }
                        }
                        EditorApplication.delayCall += () => Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                    }
                }
                break;
            case LINKS:
                {
                    TxLinkHandle[] links = m_target.selectedLinks;
                    if (links.Length > 0)
                    {
                        Undo.RecordObject(m_target, "Delete Truss Links");
                        for (int i = links.Length - 1; i >= 0; --i)
                        {
                            if (links[i].selected)
                            {
                                m_target.DeleteLink(links[i].index, o => { EditorApplication.delayCall += () => Undo.DestroyObjectImmediate(o); });
                            }
                        }
                        EditorApplication.delayCall += () => Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                    }
                }
                break;
            case FACES:
                {
                    TxFaceHandle[] faces = m_target.selectedFaces;
                    if (faces.Length > 0)
                    {
                        Undo.RecordObject(m_target, "Delete Truss Faces");
                        for (int i = faces.Length - 1; i >= 0; --i)
                        {
                            if (faces[i].selected)
                            {
                                m_target.DeleteFace(faces[i].index, o => { EditorApplication.delayCall += () => Undo.DestroyObjectImmediate(o); });
                            }
                        }
                        EditorApplication.delayCall += () => Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                    }
                }
                break;
        }
    }

    void CloneNodes(TxNodeHandle[] _nodes)
    {
        TxNodeHandle[] newNodes = new TxNodeHandle[_nodes.Length];
        for (int i = 0; i < _nodes.Length; ++i)
        {
            TxNodeHandle node = _nodes[i];
            TxNodeHandle newNode = CreateNode();
            newNode.mass = node.mass;
            newNode.position = node.position;
            newNodes[i] = newNode;
        }
    }

    void NamedSets(ref string _name, bool _selection)
    {
        GUI.enabled = true;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Named Set");
        EditorGUIUtility.labelWidth = 0;
        _name = EditorGUILayout.TextField(_name);
        Rect textRect = GUILayoutUtility.GetLastRect();
        if (GUILayout.Button('\u2026'.ToString(), EditorStyles.miniButtonLeft, GUILayout.Width(20))) // 'u2026' - '…' - Horizontal Ellipsis
        {
            SelectNamedSet(textRect);
        }
        GUI.enabled = _selection && (_name != "");
        if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(20)))
        {
            AddNamedSet();
        }
        GUI.enabled = (_name != "");
        if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20)))
        {
            RemoveNamedSet();
        }
        TxEditor.LookLikeControls();
        EditorGUILayout.EndHorizontal();
    }

    void AddNamedSet()
    {
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    TxNodeHandle[] nodes = m_target.selectedNodes;
                    if (nodes.Length > 0 && m_nodesSet != "")
                    {
                        int index = -1;
                        foreach (TxTruss.NamedSet s in m_target.trussData.nodesSet)
                        {
                            if (s.name == m_nodesSet)
                            {
                                index = Array.IndexOf(m_target.trussData.nodesSet, s);
                                break;
                            }
                        }
                        if (index == -1)
                        {
                            index = m_target.trussData.nodesSet.Length;
                            Array.Resize(ref m_target.trussData.nodesSet, m_target.trussData.nodesSet.Length + 1);
                            m_target.trussData.nodesSet[index] = new TxTruss.NamedSet();
                        }
                        m_target.trussData.nodesSet[index].name = m_nodesSet;
                        Array.Resize(ref m_target.trussData.nodesSet[index].indices, nodes.Length);
                        for (int i = 0; i < nodes.Length; ++i)
                        {
                            m_target.trussData.nodesSet[index].indices[i] = nodes[i].index;
                        }
                    }
                }
                break;
            case LINKS:
                {
                    TxLinkHandle[] links = m_target.selectedLinks;
                    if (links.Length > 0 && m_linksSet != "")
                    {
                        int index = -1;
                        foreach (TxTruss.NamedSet s in m_target.trussData.linksSet)
                        {
                            if (s.name == m_linksSet)
                            {
                                index = Array.IndexOf(m_target.trussData.linksSet, s);
                                break;
                            }
                        }
                        if (index == -1)
                        {
                            index = m_target.trussData.linksSet.Length;
                            Array.Resize(ref m_target.trussData.linksSet, m_target.trussData.linksSet.Length + 1);
                            m_target.trussData.linksSet[index] = new TxTruss.NamedSet();
                        }
                        m_target.trussData.linksSet[index].name = m_linksSet;
                        Array.Resize(ref m_target.trussData.linksSet[index].indices, links.Length);
                        for (int i = 0; i < links.Length; ++i)
                        {
                            m_target.trussData.linksSet[index].indices[i] = links[i].index;
                        }
                    }
                }
                break;
            case FACES:
                {
                    TxFaceHandle[] faces = m_target.selectedFaces;
                    if (faces.Length > 0 && m_facesSet != "")
                    {
                        int index = -1;
                        foreach (TxTruss.NamedSet s in m_target.trussData.facesSet)
                        {
                            if (s.name == m_facesSet)
                            {
                                index = Array.IndexOf(m_target.trussData.facesSet, s);
                                break;
                            }
                        }
                        if (index == -1)
                        {
                            index = m_target.trussData.facesSet.Length;
                            Array.Resize(ref m_target.trussData.facesSet, m_target.trussData.facesSet.Length + 1);
                            m_target.trussData.facesSet[index] = new TxTruss.NamedSet();
                        }
                        m_target.trussData.facesSet[index].name = m_facesSet;
                        Array.Resize(ref m_target.trussData.facesSet[index].indices, faces.Length);
                        for (int i = 0; i < faces.Length; ++i)
                        {
                            m_target.trussData.facesSet[index].indices[i] = faces[i].index;
                        }
                    }
                }
                break;
        }
    }

    void RemoveNamedSet()
    {
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    if (m_nodesSet != "")
                    {
                        int index = -1;
                        for (int i = 0; i < m_target.trussData.nodesSet.Length; ++i)
                        {
                            if (index == -1)
                            {
                                if (m_target.trussData.nodesSet[i].name == m_nodesSet) index = i;
                            }
                            if (index != -1 && i < m_target.trussData.nodesSet.Length - 1)
                            {
                                m_target.trussData.nodesSet[i] = m_target.trussData.nodesSet[i + 1];
                            }
                        }
                        if (index != -1)
                        {
                            Array.Resize(ref m_target.trussData.nodesSet, m_target.trussData.nodesSet.Length - 1);
                        }
                    }
                }
                break;
            case LINKS:
                {
                    if (m_linksSet != "")
                    {
                        int index = -1;
                        for (int i = 0; i < m_target.trussData.linksSet.Length; ++i)
                        {
                            if (index == -1)
                            {
                                if (m_target.trussData.linksSet[i].name == m_linksSet) index = i;
                            }
                            if (index != -1 && i < m_target.trussData.linksSet.Length - 1)
                            {
                                m_target.trussData.linksSet[i] = m_target.trussData.linksSet[i + 1];
                            }
                        }
                        if (index != -1)
                        {
                            Array.Resize(ref m_target.trussData.linksSet, m_target.trussData.linksSet.Length - 1);
                        }
                    }
                }
                break;
            case FACES:
                {
                    if (m_facesSet != "")
                    {
                        int index = -1;
                        for (int i = 0; i < m_target.trussData.facesSet.Length; ++i)
                        {
                            if (index == -1)
                            {
                                if (m_target.trussData.facesSet[i].name == m_facesSet) index = i;
                            }
                            if (index != -1 && i < m_target.trussData.facesSet.Length - 1)
                            {
                                m_target.trussData.facesSet[i] = m_target.trussData.facesSet[i + 1];
                            }
                        }
                        if (index != -1)
                        {
                            Array.Resize(ref m_target.trussData.facesSet, m_target.trussData.facesSet.Length - 1);
                        }
                    }
                }
                break;
        }
    }

    void SelectNamedSet(Rect _textRect)
    {
        switch (subObjects.intValue)
        {
            case NODES:
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (TxTruss.NamedSet s in m_target.trussData.nodesSet)
                    {
                        menu.AddItem(new GUIContent(s.name), false, v =>
                        {
                            TxTruss.NamedSet namedSet = (TxTruss.NamedSet)v;
                            m_nodesSet = namedSet.name;
                            foreach (TxNodeHandle n in m_target.nodes) n.selected = false;
                            foreach (int i in namedSet.indices) { m_target.nodes[i].selected = true; m_target.nodes[i].hidden = false; }
                        }
                        , s);
                    }
                    menu.DropDown(_textRect);
                }
                break;
            case LINKS:
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (TxTruss.NamedSet s in m_target.trussData.linksSet)
                    {
                        menu.AddItem(new GUIContent(s.name), false, v =>
                        {
                            TxTruss.NamedSet namedSet = (TxTruss.NamedSet)v;
                            m_linksSet = namedSet.name;
                            foreach (TxLinkHandle l in m_target.links) l.selected = false;
                            foreach (int i in namedSet.indices) { m_target.links[i].selected = true; m_target.links[i].hidden = false; }
                        }
                        , s);
                    }
                    menu.DropDown(_textRect);
                }
                break;
            case FACES:
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (TxTruss.NamedSet s in m_target.trussData.facesSet)
                    {
                        menu.AddItem(new GUIContent(s.name), false, v =>
                        {
                            TxTruss.NamedSet namedSet = (TxTruss.NamedSet)v;
                            m_facesSet = namedSet.name;
                            foreach (TxFaceHandle f in m_target.faces) f.selected = false;
                            foreach (int i in namedSet.indices) { m_target.faces[i].selected = true; m_target.faces[i].hidden = false; }
                        }
                        , s);
                    }
                    menu.DropDown(_textRect);
                }
                break;
        }
    }
}
