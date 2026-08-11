using UnityEngine;

namespace SolarSystem
{
    [CreateAssetMenu(fileName = "NewPlanet", menuName = "SolarSystem/Planet")]
    public class Planet : ScriptableObject, ICelestialBody
    {
        public string planetName;
        public float rotationSpeed;
        
        public string Name => planetName;
        public float RotationSpeed => rotationSpeed;
    }
}