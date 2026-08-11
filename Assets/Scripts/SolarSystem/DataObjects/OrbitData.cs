using UnityEngine;

namespace SolarSystem
{
    [CreateAssetMenu(fileName = "NewOrbitData", menuName = "SolarSystem/OrbitData")]
    public class OrbitData : ScriptableObject
    {
        [SerializeField] private float semiMajorAxis;
        // wprowadzić poprawkę na ustawiane automatycznie
        [SerializeField] private float orbitalPeriod;
        [Range(0f, 1f)] [SerializeField] public float eccentricity;
        [SerializeField] private float inclination;
        
        public float SemiMajorAxis => semiMajorAxis;
        public float OrbitalPeriod => orbitalPeriod;
        public float Eccentricity => eccentricity;
        public float Inclination => inclination;

    }
}