/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

public class TxSceneSettings : MonoBehaviour
{
    #region Properties

    public float simulationStep
    {
        get { return m_simulationStep; }
        set { m_simulationStep = value; }
    }

    public int substepPower
    {
        get { return m_substepPower; }
        set { m_substepPower = value; }
    }
    public int solverIterations
    {
        get { return m_solverIterations; }
        set { m_solverIterations = value; }
    }

    public Vector3 globalGravity
    {
        get { return m_globalGravity; }
        set { m_globalGravity = value; }
    }

    public float globalPressure
    {
        get { return m_globalPressure; }
        set { m_globalPressure = value; }
    }

    public int workerThreads
    {
        get { return m_workerThreads; }
    }

    #endregion

    #region Unity

    void OnValidate()
    {
        m_simulationStep = Time.fixedDeltaTime;// Mathf.Max(m_simulationStep, 0.0f);
        m_substepPower = Mathf.Max(m_substepPower, 0);
        m_solverIterations = Mathf.Max(m_solverIterations, 1);
        m_globalPressure = Mathf.Max(m_globalPressure, 0.0f);
        if (Application.isPlaying)
        {
            int worldID = TxWorld.instance.worldID;
            TxNative.WorldSetSimulationStep(worldID, m_simulationStep);
            TxNative.WorldSetSimulationSubstepPower(worldID, m_substepPower);
            TxNative.WorldSetSolverIterations(worldID, m_solverIterations);
            TxNative.WorldSetGlobalGravity(worldID, m_globalGravity);
            TxNative.WorldSetGlobalPressure(worldID, m_globalPressure);
            TxNative.ThreadsStopWorkers();
            int workerThreads = m_workerThreads < 0 ? SystemInfo.processorCount + m_workerThreads : Mathf.Min(SystemInfo.processorCount, m_workerThreads);
            if (workerThreads > 0) TxNative.ThreadsStartWorkers(workerThreads);
        }
    }

    #endregion

    #region Private

    [SerializeField]
    float m_simulationStep = 1.0f / 100.0f;
    [SerializeField]
    int m_substepPower = 0;
    [SerializeField]
    int m_solverIterations = 5;
    [SerializeField]
    Vector3 m_globalGravity = new Vector3(0, -9.80665f, 0);
    [SerializeField]
    float m_globalPressure = 101325.0f;
    [SerializeField]
    int m_workerThreads = -2;

    #endregion
}
