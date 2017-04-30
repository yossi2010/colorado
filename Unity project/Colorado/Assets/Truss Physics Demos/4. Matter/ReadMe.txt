Matter Demo

  This demo shows how to use Tx Matter asset to control the soft body surface parameters.

  On the scene:
  - A static body made of the Unity's built-in Plane object. The 'Matters' array first element of the static body refers Tx Matter asset with 'Static Friction' and 'Sliding Friction' parameters set to 0.1.
  - Five cube shaped soft bodies. The soft bodies refer matter assets with different friction coefficients - from 0.1 to 0.5.
  - When you start the demo the soft bodies fall onto the plane and slide along its surface. The friction force preventing the soft bodies from sliding is different of each of them. Its magnitude is proportional to the geometrical mean of friction coefficients of two contacting matters.
