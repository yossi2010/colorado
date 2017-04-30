/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;
using UnityEngine.Serialization;

public class TxMatter : ScriptableObject
{
    #region Properties

    public int matterID
    {
        get { return m_matterID; }
        set { m_matterID = value; }
    }

    public float staticFriction
    {
        get { return m_staticFriction; }
        set
        {
            m_staticFriction = value;
            if (TxNative.MatterExists(m_matterID))
            {
                TxNative.MatterSetStaticFriction(m_matterID, m_staticFriction);
                TxNative.MatterSetSlidingFriction(m_matterID, Mathf.Min(m_slidingFriction, m_staticFriction));
            }
        }
    }

    public float slidingFriction
    {
        get { return m_slidingFriction; }
        set
        {
            m_slidingFriction = value;
            if (TxNative.MatterExists(m_matterID))
            {
                TxNative.MatterSetSlidingFriction(m_matterID, Mathf.Min(m_slidingFriction, m_staticFriction));
            }
        }
    }

    #endregion

    #region Private

    [SerializeField]
    [FormerlySerializedAs("staticFriction")]
    float m_staticFriction = 0.7f;
    [SerializeField]
    [FormerlySerializedAs("slidingFriction")]
    float m_slidingFriction = 0.5f;

    int m_matterID = -1;

    #endregion
}
