/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TxEditor
{
    #region Constants

    public const float LABELS_WIDTH = 140.0f;

    public const string MENU_ROOT = "Edit/Truss Physics/";

    #endregion

    #region Methods

    public static void LookLikeControls()
    {
        EditorGUIUtility.labelWidth = LABELS_WIDTH;
    }

    public static GUIStyle UnpressedStyle()
    {
        return GUI.skin.button;
    }
    public static GUIStyle PressedStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.background = style.active.background;
        style.normal.textColor = style.active.textColor;
        return style;
    }
    public static GUIStyle MiniUnpressedStyle()
    {
        return EditorStyles.miniButton;
    }
    public static GUIStyle MiniPressedStyle()
    {
        GUIStyle style = new GUIStyle(EditorStyles.miniButton);
        style.normal.background = style.active.background;
        style.normal.textColor = style.active.textColor;
        return style;
    }

    public static void HideHandles(bool _yes)
    {
        // @@@ !!! HACK !!! @@@
        Type type = typeof(Tools);
        FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
        field.SetValue(null, _yes);
    }

    static GUIStyle sm_selectionRectStyle = null;
    public static GUIStyle SelectionRectStyle()
    {
        if (sm_selectionRectStyle == null)
        {
            sm_selectionRectStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("selectionRect");
        }
        return sm_selectionRectStyle;
    }

    public static T CreateAsset<T>(string _name = "") where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "") path = "Assets";
        else if (Path.GetExtension(path) != "") path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

        if (_name == "") _name = typeof(T).ToString();
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + _name + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }

    public static bool RayTriangleTest(Ray R, Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 N = Vector3.Cross(B - A, C - A).normalized;
        float dotRN = Vector3.Dot(R.direction, N);
        if (dotRN >= 0) return false;

        Vector3 P = R.origin + R.direction * Vector3.Dot(N, A - R.origin) / dotRN;

        Vector3 v0 = C - A;
        Vector3 v1 = B - A;
        Vector3 v2 = P - A;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0) && (v >= 0) && (u + v < 1);
    }

    #endregion

    #region Menu Items

    [MenuItem(MENU_ROOT + "Create Asset/Tx Truss", false, 0)]
    static void CreateTruss()
    {
        CreateAsset<TxTruss>("New Truss");
    }

    [MenuItem(MENU_ROOT + "Create Asset/Tx Matter", false, 0)]
    static void CreateMatter()
    {
        CreateAsset<TxMatter>("New Matter");
    }

    [MenuItem(MENU_ROOT + "Add Component/Tx Soft Boby", false, 1)]
    static void AddSoftBobyComponent()
    {
        foreach (GameObject o in Selection.gameObjects)
        {
            o.AddComponent<TxSoftBody>();
        }
    }
    [MenuItem(MENU_ROOT + "Add Component/Tx Soft Boby", true)]
    static bool CanAddSoftBobyComponent()
    {
        foreach (GameObject o in Selection.gameObjects)
        {
            bool valid = o.GetComponent<TxSoftBody>() == null &&
                         o.GetComponent<TxStaticBody>() == null &&
                        (o.hideFlags & HideFlags.NotEditable) == 0;
            if (!valid) return false; 
        }
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem(MENU_ROOT + "Add Component/Tx Static Boby", false, 1)]
    static void AddStaticBobyComponent()
    {
        foreach (GameObject o in Selection.gameObjects)
        {
            o.AddComponent<TxStaticBody>();
        }
    }
    [MenuItem(MENU_ROOT + "Add Component/Tx Static Boby", true)]
    static bool CanAddStaticBobyComponent()
    {
        foreach (GameObject o in Selection.gameObjects)
        {
            bool valid = o.GetComponent<TxSoftBody>() == null &&
                         o.GetComponent<TxStaticBody>() == null &&
                        (o.hideFlags & HideFlags.NotEditable) == 0;
            if (!valid) return false;
        }
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem(MENU_ROOT + "Add Component/Tx Rigid Boby", false, 1)]
    static void AddRigidBobyComponent()
    {
        foreach (GameObject o in Selection.gameObjects)
        {
            o.AddComponent<TxRigidBody>();
        }
    }
    [MenuItem(MENU_ROOT + "Add Component/Tx Rigid Boby", true)]
    static bool CanAddRigidBobyComponent()
    {
        foreach (GameObject o in Selection.gameObjects)
        {
            bool valid = o.GetComponent<Rigidbody>() != null;
            if (!valid) return false;
        }
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem(MENU_ROOT + "Add Component/Tx Constraint", false, 1)]
    static void AddConstraintComponent()
    {
        foreach (GameObject o in Selection.gameObjects)
        {
            o.AddComponent<TxConstraint>();
        }
    }
    [MenuItem(MENU_ROOT + "Add Component/Tx Constraint", true)]
    static bool CanAddConstraintComponent()
    {
        foreach (GameObject o in Selection.gameObjects)
        {
            bool valid = o.GetComponent<TxSoftBody>() != null;
            if (!valid) return false;
        }
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem(MENU_ROOT + "Edit Scene Settings", false, 10)]
    static void OpenSceneSettings()
    {
        TxSceneSettings settings = UnityEngine.Object.FindObjectOfType<TxSceneSettings>();
        if (settings == null) settings = new GameObject("Tx Scene Settings").AddComponent<TxSceneSettings>();
        settings.gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
        settings.gameObject.transform.hideFlags |= HideFlags.HideInInspector;
        settings.hideFlags &= ~HideFlags.NotEditable;
        Selection.activeObject = settings.gameObject;
    }

    #endregion
}
