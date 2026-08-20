using UnityEngine;

namespace SolarSystem
{
    public static class OrbitMath
    {
        public static readonly IOrbitPath Circular = new CircularPath();
        
        // Zakres [0, 2PI)
        public static float NormalizeAngle(float angle)
        {
            angle %= 2f * Mathf.PI;
            if (angle < 0f)
                angle += 2f * Mathf.PI;
            return angle;
        }
        
        // Zakres [-PI, PI)
        public static float NormalizeAngleSigned(float angle)
        {
            angle = NormalizeAngle(angle);
            if (angle >= Mathf.PI)
                angle -= 2f * Mathf.PI;
            return angle;
        }
        
        public static void UpdateObjectPosition(CelestialObject passenger, Vector3 center, float radius, float angle, float inclination)
        {
            Vector3 relativePosition = Circular.GetPosition(radius, angle);
            Vector3 rotatedPosition = Quaternion.Euler(inclination, 0, 0) * relativePosition;
            passenger.Model.position = center + rotatedPosition;
        }
    }
}