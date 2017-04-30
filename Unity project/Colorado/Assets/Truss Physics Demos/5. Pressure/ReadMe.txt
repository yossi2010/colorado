Pressure Demo

  This demo shows the soft body internal pressure feature of Truss Physics simulation. The main purpose of this feature is to prevent a soft body from turning inside out as the spring-mass system simulation itself can not handle this situation. It can be used as well to model inflatable objects like balloons.

  On the scene:
  - Three sphere shaped soft bodies are placed one above the other.
  - The Truss asset of the soft bodies is designed to model only the soft surface of the objects. No internal links are created.
  - All the soft bodies have their Filling option activated with the Internal Pressure and Adiabatic Index properties set to reasonable values.
  - When you start the demo the soft bodies fall down and stack on the ground. The internal pressure lets them keep their shape.