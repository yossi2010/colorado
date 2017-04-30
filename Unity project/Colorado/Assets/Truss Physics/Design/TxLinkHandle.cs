/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

public class TxLinkHandle : ScriptableObject
{
    public int index = -1;
    public int node0 = -1, node1 = -1;
    public float stiffness = 1000000.0f;
    public float damping = 1000.0f;
    public float elastic = 10.0f;
    public float breaking = 100.0f;
    public float stretching = 1.0f;
    public enum Resist { None = 0, Compression = 1, Stretching = 2, Any = 3 };
    public Resist resist = Resist.Any;
    public bool selected = false;
    public bool hidden = false;

    public static TxLinkHandle CreateInstance()
    {
        return CreateInstance<TxLinkHandle>();
    }
}
