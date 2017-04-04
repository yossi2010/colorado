using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        public float SteerSpeed = 0.1f;
        float Steer=0;
        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            Steer += SteerSpeed * (h - Steer);
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(Steer, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
