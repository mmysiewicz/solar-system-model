using UnityEngine;

namespace SolarSystem
{
    [System.Serializable]
    public class CelestialObject
    {
        [SerializeField] public Transform Model;
        [SerializeField] public float StartAngle;
        [HideInInspector] public float CurrentAngle { get; set; }
        [SerializeField] private UnityEngine.Object bodySource;

        public ICelestialBody Body
        {
            get
            {
                if (bodySource is ICelestialBody body) return body;
                return null;
            }
        }
    }
}