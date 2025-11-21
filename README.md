# GAM531 Assignment 9 : “3D Object Collision Detection in OpenTK (C# + OpenGL 3.3)”

## Description
a small 3D environment containing static objects such as cubes, a wall, a door, and a NPC placeholder with proper collision applied to ensure that the player cannot move through them.

## The collision detection method you used ?
I used Axis-Aligned Bounding Box (AABB) - which is axis-aligned box that wraps around our object and use maximum and minimum corner to create the box for collision detection, providing simple asnd efficient way to create collision detection for stationary objects.

## How your collision and movement integration works ?
My objects and player(camera - small AABB box) are all wrapped with AABBCollider that create a box with maximum and minimum bounds along X, Y, and Z axis of the each object. The collision was detected using boolean collisionDetected, newPosition and oldPosition assignment with cameraPosition, UpdatePosition and Intersects functions, where UpdatePosition updates the camera's AABB box location when camera moves, and Intersect function check the AABBs overlap on all three axis (X, Y, and Z) simultaneously using the Separating Axis Theorem., if yes, the collistionDetected is true and the caemra postion will be assigned with oldPosition which prevents from moving, else the camera moves freely.

And for my simple touch detection, I created a GetDoor() function that calculates the distance between the door's position and the player's position. When the distance is within 2 units (the interaction range), the function returns my door GameObject. Which in OnUpdateFrame() this function is called every frame to check is door is in range. When a door is detected nearby, a console message prompts the player that they are in range and can press F to open it. Also to prevent message spam, a boolean _wasNearDoorLastFrame was set to track whether the player was near the door in the previous frame or not, ensuring the prompt only displays once when first entering the interaction zone.

## Any challenges encountered and how you solved them ?
One of the chaleges was figureing out how to implement a GameObject class that works with my existing Mesh.cs, Shader.cs, and Texture.cs without breaking the system and also fufil assignment requirements. I needed to wrap collisions around objects while keeping the original rendering code functional.

The solution was to create a GameObject wrapper that stores the position, scale, and collider data, while using GetModelMatrix() fucntion to generate transformations for my existing shader system. This way it keeps my Mesh.cs, Shader.cs and Texture.cs unchanged and allows collision system to be added on top of my existing rendering pipeline

## Credits
AABB Collision context:

https://developer.mozilla.org/en-US/docs/Games/Techniques/3D_collision_detection

Textures:

https://www.behance.net/gallery/71992995/15-Free-Metallic-Gold-Textures

https://www.3dmd.net/gallery/displayimage-103.html

https://opengameart.org/content/tiny-texture-pack-3-gem11-512x512png

https://www.filterforge.com/filters/6821.html

https://fity.club/lists/w/wooden-crate-texture/

https://opengameart.org/node/10151

https://opengameart.org/content/retro-doors-texture-pack-door01orangepng

https://opengameart.org/content/grid-placeholder-texture-templategridalbedopng
