using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace WindowOpenTK
{
    // Axis-Aligned Bounding Box collider
    // Uses min/max bounds for efficient collision detection
    public class AABBCollider
    {
        public Vector3 Center { get; private set; }
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }
        public Vector3 Size { get; private set; }

        public AABBCollider(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = size;
            CalculateBounds();
        }

        // Calculate min/max bounds from center and size
        private void CalculateBounds()
        {
            Vector3 halfSize = Size * 0.5f;
            Min = Center - halfSize;
            Max = Center + halfSize;
        }

        // Update the collider position (recalculates bounds)
        public void UpdatePosition(Vector3 newPosition)
        {
            Center = newPosition;
            CalculateBounds();
        }

        // Check if this AABB intersects with another AABB
        // Uses separating axis theorem - checks overlap on all 3 axes
        public bool Intersects(AABBCollider other)
        {
            return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
                   (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
                   (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
        }
    }
}