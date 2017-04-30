Constraints Demo

  This demo shows how to use Tx Constraint component to attach a soft body to another static, rigid, soft body or to the world.

  On the scene:
  - Two soft bodies. The first one is attached to the world and the second one is attached to the first one.
  - Both soft bodies share the same Tx Truss asset - Box Truss. The truss asset has several named node sets defined. First body's Tx Constraint component refers 'Axis0' named node set to snap its nodes to the world. The same node set is used to define the motor axis to rotate the soft body. Second body's Tx Constraint component snaps the node from 'Node0' named set of the second body to nodes from 'Node1' and 'Node2' named sets of the first body with max distance of 1 meter and to the edge formed by nodes from 'Edge0' named set with max distance of 0.1 meter.
  - When you start the demo first body starts to rotate around its central axis and second body follows attached by one of its corner to an edge of the first body.