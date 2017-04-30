Simple Truss Demo

  This demo shows three basic entities of Truss Physics simulation - Static Body, Soft Body and Constraint. You use static bodies to define your level geometry, and soft bodies with constraints to build your dynamic objects.

  On the scene:
  - A static body made of the Unity's built-in Plane object.
  - A soft body made of the Unity's built-in Cube object.
  - The soft body's Truss asset is designed to fit the Cube object's mesh. The truss has a node set named 'Anchor' containing one node of the truss.
  - A constraint is added to the soft body to snap its 'Anchor' node to the world space. The Strength property of the snap is adjusted to let the snap break after few seconds of simulation.


