using System.Collections;
using UnityEngine;

namespace SolarSystem
{
    public class SphereOfInfluenceTransferManeuver : ITransferManeuver
    {
        // Czas animacji SOI
        private const float TransitionDuration = 2.5f;
        
        public bool CanHandle(Orbit source, Orbit destiny)
        {
            return destiny == source.Parent || source == destiny.Parent;
        }

        public IEnumerator Execute(CelestialObject passenger, Orbit source, Orbit destiny)
        {
            bool isEscape = destiny == source.Parent;
            source.RemovePassenger(passenger);
            Vector3 startPosition = passenger.Model.position;
            Vector3 targetPosition = isEscape ? source.getCenterPoint().position : destiny.getCenterPoint().position;

            float elapsed = 0f;
            while (elapsed < TransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / TransitionDuration);
                passenger.Model.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
            
            passenger.CurrentAngle = destiny.GetMainPassengerAngle();
            destiny.AddPassenger(passenger);
            Debug.Log(isEscape ? "Obiekt przeszedł na orbitę nadrzędną" : "Obiekt przeszedł na orbitę podrzędną");
            
        }
    }
}