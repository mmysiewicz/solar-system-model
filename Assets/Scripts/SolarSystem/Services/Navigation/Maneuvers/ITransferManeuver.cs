using System.Collections;

namespace SolarSystem
{
    // Interfejs dzięki którymi klasy realizujące lot nie muszą wiedzieć
    // jaki etap lotu jest wykonywany
    public interface ITransferManeuver
    {
        // Czy dany manewr potrafi obsłużyć przejście między danymi dwiema orbitami
        bool CanHandle(Orbit source, Orbit destiny);
        
        IEnumerator Execute(CelestialObject passenger, Orbit source, Orbit destiny);
    }
}