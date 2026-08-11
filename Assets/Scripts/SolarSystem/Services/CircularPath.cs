using UnityEngine;

namespace SolarSystem
{
    public class CircularPath : IOrbitPath
    {
        public Vector3 GetPosition(float radius, float angle)
        {
            //float rad = angle * Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(angle);
            float y = 0;
            float z = radius * Mathf.Sin(angle);
            return new Vector3(x, y, z);
        }
    }
}