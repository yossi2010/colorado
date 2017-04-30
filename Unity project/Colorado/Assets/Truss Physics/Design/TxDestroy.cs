/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

[ExecuteInEditMode()]
public class TxDestroy : MonoBehaviour
{
    public Component target = null;

    void Start()
    {
        if (target != null) DestroyImmediate(target);
        DestroyImmediate(this);
    }

    static public void IfNotUnique<_Component>(_Component _component) where _Component : Component
    {
#if UNITY_EDITOR
        _Component[] allComponents = _component.GetComponents<_Component>();
        foreach (_Component c in allComponents)
        {
            if (c != _component)
            {
                Debug.LogWarning("New " + _component.GetType().FullName + " component was not added because it conflicts with the existing components.");
                _component.gameObject.AddComponent<TxDestroy>().target = _component;
                return;
            }
        }
#endif
    }
}
