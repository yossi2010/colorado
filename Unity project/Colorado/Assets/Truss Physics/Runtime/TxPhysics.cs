/* ______                   ___  __            _       
  /_  __/_____ _____ ___   / _ \/ /  __ _____ (_)______
   / / / __/ // (_-<(_-<  / ___/ _ \/ // (_-</ / __(_-<
  /_/ /_/  \_,_/___/___/ /_/  /_//_/\_, /___/_/\__/___/
  Soft-Body Simulation for Unity3D /___/               
                                         Heartbroken :( */

using UnityEngine;

public static class TxPhysics
{
    #region Methods

    public static bool RayCast(Vector3 _origin, Vector3 _direction, float _distance, TxBody _skip, out TxBody _body, out Vector3 _point, out Vector3 _normal, out int _face)
    {
        if (TxWorld.created)
        {
            return TxWorld.instance.RayCast(_origin, _direction, _distance, _skip, out _body, out _point, out _normal, out _face);
        }
        _body = null; _point = Vector3.zero; _normal = Vector3.zero; _face = -1;
        return false;
    }

    #endregion
}
