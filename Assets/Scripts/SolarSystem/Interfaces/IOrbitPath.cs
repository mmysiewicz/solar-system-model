using UnityEngine;

namespace SolarSystem
{
    public interface IOrbitPath
    {
        Vector3 GetPosition(float radius, float angle);
    }
}