using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;


namespace WindowOpenTK
{
    public class Game : GameWindow
    {
        private Mesh _cubeMesh;
        private Texture[] _cubeTextures = new Texture[3];
        // NEW: Textures for room, wall and placeholder
        private Texture _roomTexture, _floorTexture, _doorTexture, _wallTexture, _placeHolderTexture;

        private Vector3[] _cubePos = new Vector3[]
        {
            new Vector3(-6f, -4.3f, 0f),
            new Vector3(0f, 0.5f, -2f),
            new Vector3(6f, 2f, 3f)
        };

        // NEW: Scene objects with collision
        private List<GameObject> _sceneObjects;
        private AABBCollider _playerCollider; // Player collision box

        // NEW: touch detection for door
        private const float _doorTouchDistance = 2.0f;
        private bool _wasAtDoor = false;

        private Vector3[] lightPos = new Vector3[3];
        private Vector3 lightColor = new Vector3(1.0f, 1.0f, 1.0f);

        private Shader _shader;
        private Camera _camera;

        private bool _firstMove = true;
        private Vector2 _lastPos;

        private float deltaTime = 0.0f;
        private float sensitivity = 0.1f;

        private bool[] _lightOn = { true, true, true };

        // add camera limits for room
        private const float X_limit = 9.5f;
        private const float Y_limit = 3.5f;
        private const float Z_limit = 9.5f;

        // constructor for the game class
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(new Color4(0.1f, 0.1f, 0.1f, 1f));
            GL.Enable(EnableCap.DepthTest);

            //36 vertices to have 12 triangle for a 3D cube (2 triangles per face * 6 faces with * 3 vertices per triangle)
            float[] _vertices =
            {
                //2 triangles each
                // Front face
                -0.5f, -0.5f,  0.5f,  0f, 0f, 1f, 0f,0f,
                 0.5f, -0.5f,  0.5f,  0f, 0f, 1f, 1f,0f,
                 0.5f,  0.5f,  0.5f,  0f, 0f, 1f, 1f,1f,

                 0.5f,  0.5f,  0.5f,  0f, 0f, 1f, 1f,1f,
                -0.5f,  0.5f,  0.5f,  0f, 0f, 1f, 0f,1f,
                -0.5f, -0.5f,  0.5f,  0f, 0f, 1f, 0f,0f,

                // Back face
                -0.5f, -0.5f, -0.5f,  0f, 0f, -1f, 1f,0f,
                -0.5f,  0.5f, -0.5f,  0f, 0f, -1f, 1f,1f,
                 0.5f,  0.5f, -0.5f,  0f, 0f, -1f, 0f,1f,

                 0.5f,  0.5f, -0.5f,  0f, 0f, -1f, 0f,1f,
                 0.5f, -0.5f, -0.5f,  0f, 0f, -1f, 0f,0f,
                -0.5f, -0.5f, -0.5f,  0f, 0f, -1f, 1f,0f,

                // Left face
                -0.5f,  0.5f, -0.5f,  -1f, 0f, 0f, 0f,1f,
                -0.5f,  0.5f,  0.5f,  -1f, 0f, 0f, 1f,1f,
                -0.5f, -0.5f,  0.5f,  -1f, 0f, 0f, 1f,0f,

                -0.5f, -0.5f,  0.5f,  -1f, 0f, 0f, 1f,0f,
                -0.5f, -0.5f, -0.5f,  -1f, 0f, 0f, 0f,0f,
                -0.5f,  0.5f, -0.5f,  -1f, 0f, 0f, 0f,1f,

                // Right face
                 0.5f,  0.5f, -0.5f,  1f, 0f, 0f, 1f,1f,
                 0.5f, -0.5f, -0.5f,  1f, 0f, 0f, 1f,0f,
                 0.5f, -0.5f,  0.5f,  1f, 0f, 0f, 0f,0f,

                 0.5f, -0.5f,  0.5f,  1f, 0f, 0f, 0f,0f,
                 0.5f,  0.5f,  0.5f,  1f, 0f, 0f, 0f,1f,
                 0.5f,  0.5f, -0.5f,  1f, 0f, 0f, 1f,1f,

                // Top face
                -0.5f,  0.5f, -0.5f,  0f, 1f, 0f, 0f,0f,
                 0.5f,  0.5f, -0.5f,  0f, 1f, 0f, 1f,0f,
                 0.5f,  0.5f,  0.5f,  0f, 1f, 0f, 1f,1f,

                 0.5f,  0.5f,  0.5f,  0f, 1f, 0f, 1f,1f,
                -0.5f,  0.5f,  0.5f,  0f, 1f, 0f, 0f,1f,
                -0.5f,  0.5f, -0.5f,  0f, 1f, 0f, 0f,0f,

                // Bottom face
                -0.5f, -0.5f, -0.5f,  0f, -1f, 0f, 1f,1f,
                -0.5f, -0.5f,  0.5f,  0f, -1f, 0f, 1f,0f,
                 0.5f, -0.5f,  0.5f,  0f, -1f, 0f, 0f,0f,

                 0.5f, -0.5f,  0.5f,  0f, -1f, 0f, 0f,0f,
                 0.5f, -0.5f, -0.5f,  0f, -1f, 0f, 0f,1f,
                -0.5f, -0.5f, -0.5f,  0f, -1f, 0f, 1f,1f,
            };

            //create our mesh and get texture from assets
            _cubeMesh = new Mesh(_vertices);

            _cubeTextures[0] = Texture.LoadFromFile("Assets/box.jpg");
            _cubeTextures[1] = Texture.LoadFromFile("Assets/crystal.png");
            _cubeTextures[2] = Texture.LoadFromFile("Assets/gold.jpg");
            _roomTexture = Texture.LoadFromFile("Assets/wall.jpg");
            _floorTexture = Texture.LoadFromFile("Assets/floor.jpg");
            _doorTexture = Texture.LoadFromFile("Assets/door_01_orange.png");
            _wallTexture = Texture.LoadFromFile("Assets/100_1174_seamless.jpg");
            _placeHolderTexture = Texture.LoadFromFile("Assets/templategrid_albedo.png");

            // setup shader 
            _shader = new Shader("Shaders/phong.vert", "Shaders/phong.frag");
            //cam setup
            _camera = new Camera(new Vector3(0.0f, 1.0f, 8.0f));
            //make sure mouse cursor invisible and captured so we can have proper FPS-camera movement
            CursorState = CursorState.Grabbed;

            // NEW: Initialize collision system
            _sceneObjects = new List<GameObject>();

            // Create player collision box (small box around camera)
            _playerCollider = new AABBCollider(_camera.Position, new Vector3(1f, 2f, 1f));

            // NEW: Add collision to original cubes, and wrap in GameObjects
            for (int i = 0; i < 3; i++)
            {
                var cube = new GameObject($"Cube {i}", _cubePos[i], new Vector3(1f, 1f, 1f));
                _sceneObjects.Add(cube);
            }

            // NEW: Add a door (tall and thin, like a door)
            var door = new GameObject("Door", new Vector3(-3f, 0f, -5f), new Vector3(2f, 4f, 0.3f));
            _sceneObjects.Add(door);

            // NEW: Add a wall (long horizontal wall)
            var wall = new GameObject("Wall", new Vector3(4f, 0f, 0f), new Vector3(0.5f, 4f, 6f));
            _sceneObjects.Add(wall);

            // NEW: Add an NPC placeholder (human-sized box)
            var npc = new GameObject("NPC", new Vector3(0f, -3f, 4f), new Vector3(1f, 2f, 1f));
            _sceneObjects.Add(npc);

            //for testing purposes
            //Console.WriteLine($"aPosition location: {vertexLocation}");
            //Console.WriteLine($"aNormal location: {normalLocation}");

            //Console.WriteLine($"Objects with collision: {_sceneObjects.Count}"); //shoule be 6 objects total
        }

        //called when i need to update any game visuals
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            //clear the screen with background color
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //use our shader program
            _shader.Use();

            //create and set model, view, projection matrices
            Matrix4 projection = _camera.GetProjectionMatrix(Size.X, Size.Y);
            Matrix4 view = _camera.GetViewMatrix();

            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            //set light and view positions
            _shader.SetVector3("viewPos", _camera.Position);
            //_shader.SetVector3("objectColor", objectColor); // orange object
            _shader.SetVector3("lightColor", lightColor);//white light

            //draw room first
            GL.Disable(EnableCap.CullFace);//this makes it so we see the inside of faces
            Matrix4 roomModel = Matrix4.CreateScale(20f, 10f, 20f);
            _shader.SetMatrix4("model", roomModel);
            _roomTexture.Use(TextureUnit.Texture0);
            _cubeMesh.Draw(); // reuse same cube geometry (since room is also a cube)

            //draw floor
            Matrix4 floorModel = Matrix4.CreateScale(20f, 0.3f, 20f) * Matrix4.CreateTranslation(0.0f, -5f, 0.0f);
            _shader.SetMatrix4("model", floorModel);
            _floorTexture.Use(TextureUnit.Texture0);
            _cubeMesh.Draw(); // reuse same cube geometry (since room is also a cube)

            // setup model and light to each and draw
            for (int i = 0; i < _sceneObjects.Count; i++)
            {
                var obj = _sceneObjects[i];

                // the lights
                if (i < 3)
                {
                    lightPos[i] = obj.Position + new Vector3(0.0f, 2.0f, 0.0f);
                    _shader.SetVector3($"lightPos[{i}]", lightPos[i]);
                    GL.Uniform1(GL.GetUniformLocation(_shader.Handle, $"lightOn[{i}]"), _lightOn[i] ? 1 : 0);
                }

                Matrix4 model = obj.GetModelMatrix();
                _shader.SetMatrix4("model", model);

                // Choose texture based on object type
                if (obj.Name == "Door")
                    _doorTexture.Use(TextureUnit.Texture0);
                else if (obj.Name == "Wall")
                    _wallTexture.Use(TextureUnit.Texture0);
                else if (obj.Name == "NPC")
                    _placeHolderTexture.Use(TextureUnit.Texture0);
                else
                    _cubeTextures[i].Use(TextureUnit.Texture0);
                    

                _cubeMesh.Draw();
            }

            GL.Enable(EnableCap.CullFace);
            //display the rendered frame
            SwapBuffers();
        }

        //called every from to update game logic, phisucs, or input handling
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            //camera movement with WASD with frame-rate independant speed
            var input = KeyboardState;
            deltaTime = (float)args.Time;
            float speed = 6.5f * deltaTime;

            // NEW: Store old position for collision check
            Vector3 oldPosition = _camera.Position;
            Vector3 newPosition = _camera.Position;

            if (input.IsKeyDown(Keys.W))
                newPosition += _camera.Front * speed;
            if (input.IsKeyDown(Keys.S))
                newPosition -= _camera.Front * speed;
            if (input.IsKeyDown(Keys.A))
                newPosition -= _camera.Right * speed;
            if (input.IsKeyDown(Keys.D))
                newPosition += _camera.Right * speed;
            if (input.IsKeyDown(Keys.E))
                newPosition += _camera.Up * speed;
            if (input.IsKeyDown(Keys.Q))
                newPosition -= _camera.Up * speed;
            if (MouseState.IsButtonPressed(MouseButton.Left))
                _lightOn[0] = !_lightOn[0];
            if (MouseState.IsButtonPressed(MouseButton.Right))
                _lightOn[1] = !_lightOn[1];
            if (MouseState.IsButtonPressed(MouseButton.Middle))
                _lightOn[2] = !_lightOn[2];

            if (input.IsKeyDown(Keys.Escape))
                Close();


            // NEW: Check collision before moving
            // Update player collider to new position temporarily
            _playerCollider.UpdatePosition(newPosition);

            bool collisionDetected = false;
            foreach (var obj in _sceneObjects)
            {
                if (_playerCollider.Intersects(obj.Collider))
                {
                    collisionDetected = true;
                    break;
                }
            }

            // NEW: If collision detected, don't move (stay at old position)
            if (collisionDetected)
            {
                _playerCollider.UpdatePosition(oldPosition);
                _camera.Position = oldPosition;
            }
            else
            {
                // No collision, apply the movement
                _camera.Position = newPosition;
            }

            // Prevent camera from leaving the room
            _camera.Position = new Vector3(
                MathHelper.Clamp(_camera.Position.X, -X_limit, X_limit),
                MathHelper.Clamp(_camera.Position.Y, -Y_limit, Y_limit),
                MathHelper.Clamp(_camera.Position.Z, -Z_limit, Z_limit)
            );

            // NEW: Update player collider to final position
            _playerCollider.UpdatePosition(_camera.Position);

            // NEW: Xheck for simple door touch detection
            GameObject nearbyDoor = GetDoor();
            // NEW: Simple command prompt for interaction (no interaction system yet)
            if (nearbyDoor != null)
            {
                if (!_wasAtDoor)
                {
                    _wasAtDoor = true;
                    Console.WriteLine("You approached the door, press F to open!");
                }

                if (input.IsKeyPressed(Keys.F))
                {
                    Console.WriteLine("You opened the door!");
                }
            }
            else
            {
                _wasAtDoor = false;
            }
        }

        // NEW: Check if player is near the door
        private GameObject GetDoor()
        {
            foreach (var obj in _sceneObjects)
            {
                //check door object
                if (obj.Name == "Door")
                {
                    //calculate for length from camera to door
                    float distanceToDoor = (obj.Position - _camera.Position).Length;

                    //if within touch distance, return the door object
                    if (distanceToDoor <= _doorTouchDistance)
                    {
                        return obj;
                    }
                }
            }
            //return null if no door is close enough
            return null;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (!IsFocused) { return; }

            if (_firstMove)
            {
                _lastPos = new Vector2(e.X, e.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = e.X - _lastPos.X;
                var deltaY = e.Y - _lastPos.Y;
                _lastPos = new Vector2(e.X, e.Y);

                _camera.ProcessMouseMovement(deltaX, deltaY, sensitivity);
            }
        }

        //OnMouseWheel to control camera fov
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            _camera.Fov -= e.Offset.Y * 5.0f;
            _camera.Fov = MathHelper.Clamp(_camera.Fov, 30.0f, 90.0f);
        }

        // Called automatically whenever the window is resized
        protected override void OnResize(ResizeEventArgs e)
        {
            // Update the OpenGL viewport to match the new window dimensions
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            //unbind and delete all the buffers and shader programs
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            _cubeMesh.Dispose();
            _shader.Cleanup();

            base.OnUnload();

        }
    }
}