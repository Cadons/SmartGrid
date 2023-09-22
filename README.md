# SmartGrid

SmartGrid is a usefull plugin to create games based on grid in 3D enviroments.
This type of grid updates its state in real time, locking and unlocking its cells when there are objects that have colliders on them.
![smartgrid](https://github.com/Cadons/SmartGrid/assets/43477517/4fce98f5-30c7-41e8-a41b-bc4508da85b2)

SmartGrid provides also an implementation of the A* algorithm to calculate paths that avoid obstacles on the grid.
![pathgrid](https://github.com/Cadons/SmartGrid/assets/43477517/46bfb8fb-5b50-482b-9deb-843dab1e2805)

Another feature is support for procedural levels; currently there is an implmenentation of BSP to create dangouns. This implementation creates rooms on the grid and connect them with doors and cooridors. The world builder creates also surrounding enviroment using perlin noise. This implmentation is not yet optimized with chank and it is not raccomended for large maps generation. In the future, the algorithm could be improved to allowed the generation of large maps.
![perlinterrain](https://github.com/Cadons/SmartGrid/assets/43477517/263a64a5-7f1d-45a4-a48e-d1f9d27c70f4)
