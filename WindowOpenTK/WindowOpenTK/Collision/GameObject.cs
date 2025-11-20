using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowOpenTK
{
    // Represents any object in the 3D scene with position, scale, and collision
    public class GameObject
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public string Name { get; set; }

        public AABBCollider Collider { get; set; }

        public GameObject(string name, Vector3 position, Vector3 scale)
        {
            Name = name;
            Position = position;
            Scale = scale;

            // Automatically create collider
            Collider = new AABBCollider(position, scale);
        }

        // Gets the model transformation matrix for rendering
        public Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateScale(Scale) * Matrix4.CreateTranslation(Position);
        }
    }
}