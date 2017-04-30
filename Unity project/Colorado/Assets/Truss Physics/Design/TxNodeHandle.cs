/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

public class TxNodeHandle : ScriptableObject
{
    public int index = -1;
    public float mass = 1;
    public Vector3 position = Vector3.zero;
    public bool selected = false;
    public bool hidden = false;

    public static TxNodeHandle CreateInstance()
    {
        return CreateInstance<TxNodeHandle>();
    }
}
