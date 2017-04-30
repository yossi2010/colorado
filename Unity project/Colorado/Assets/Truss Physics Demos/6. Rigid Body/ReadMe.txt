Rigid Body Demo

  This demo shows how to use Tx Rigid Body component to add interaction between Truss Physics soft bodies and Unity rigid bodies.
  
  On the scene:
  - A sphere shaped soft body is placed above two stacks of cube shaped rigid bodies.
  - The rigid bodies have Tx Rigid Body components added. Tx Rigid Body component inherits the dynamic properties from Unity Rigidbody component and collision properties from Unity Collider component attached to the object. Tx Rigid Body component serves as a proxy representing Unity rigid body inside Truss Physics simulation.
  - When you start the demo the soft body falls on the rigid bodies stacks and makes them fall apart.