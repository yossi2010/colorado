Truss Physics 1.0.4 Beta

Desctiption:

  Truss Physics is a Unity3D extension package. Its purpose is the soft-body simulation. The soft bodies in Truss Physics are represented by mass-springs systems with optional internal pressure which allows to simulate a wide range of materials from jelly-like and springy to almost rigid but deformable. The simulation is done on CPU with extensive usage of SIMD and muli-threading optimizations.


Installation:

  Truss Physics is distributed as a single .unitypackage file. To import the package into your project use 'Assets>Import Package>Custom Package...' menu command, browse to the folder you have downloaded the package to, select TrussPhysics.unitypackage file and click Open button. In 'Importing package' dialog you can optionally uncheck 'Truss Physics Demos' folder if you want to import Truss Physics without the sample scenes.


Content:

  Truss Physics package will place its content in following folders of your project.

Assets/Plugins
  This special folder gets the Truss Physics native plugins. Each native plugin contains Truss Physics Engine, which is the core of the simulation, bulit for a specific platform.

Assets/Truss Physics
  All the integration code goes here. All game object components, asset types and custom inspectors classes are placed inside this folder.

Assets/Truss Physics Demos
  And here you'll find all the samples showing the Truss Physics features.


By downloading and installing Truss Physics package you agree to the following conditions:

  1. Truss Physics is free for evaluation, learning and non-profitable projects.
  2. Any non-profitable product using Truss Physics requires clearly readable Truss Physics logo on splash screen or credits screen.
  3. Any commercial product using Truss Physics requires the Truss Physics commercial license (though the logo is also welcome).
  4. Redistribution as anything except a final product (e.g. asset, library, engine, middleware, etc.) is not allowed.
  5. These conditions may change in the future, but the changes will not be retroactive.


Beta version notice:

  Truss Physics is currently in beta, that means that the final product quality is not yet achieved - the documentation is sparse, some important features are missing, some parameters configurations can make the simulation unstable which can freeze or even crash Unity. The author can not be responsible for any damage (like loss of unsaved data) caused by this issues. Use on your own risk and save often :) The most critical flaws will be fixed with the highest priority possible.
