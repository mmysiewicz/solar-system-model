namespace SolarSystem
{
    //Singleton - wspólny kalkulator dla orbit
    public class OrbitPhysicsCalculator
    {
        public static readonly IOrbitPath Circular = new CircularPath();
    }
}