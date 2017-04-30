/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

public class TxFaceHandle : ScriptableObject
{
    public int index = -1;
    public int node0 = -1, node1 = -1, node2 = -1;
    public int matter = 0;
    public float envelope = 0.1f;
    public bool collision = true;
    public bool skinning = true;
    public bool selected = false;
    public bool hidden = false;

    public static TxFaceHandle CreateInstance()
    {
        return CreateInstance<TxFaceHandle>();
    }
}
