using System.Collections.Generic;

namespace SolarSystem
{
    public class OrbitRegistry
    {
        private static List<Orbit> _orbits =  new List<Orbit>();
        
        public static IReadOnlyList<Orbit> All => _orbits;

        public static void Register(Orbit orbit)
        {
            if (orbit != null && _orbits.Contains(orbit)) 
                _orbits.Add(orbit);
        }

        public static void Unregister(Orbit orbit)
        {
            _orbits.Remove(orbit);
        }
    }
}