using System.Collections;
using UnityEngine;

namespace SolarSystem
{
    public class OrbitTransferManager : MonoBehaviour
    {
        public static OrbitTransferManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else 
                Destroy(gameObject);
        }
        // Planuje i wykonuje transfer Hohmanna
        public void StartTransfer(CelestialObject passenger, Orbit source, Orbit destiny)
        {
            StartCoroutine(TransferRoutine(passenger, source, destiny));
        }

        public IEnumerator TransferRoutine(CelestialObject passenger, Orbit source, Orbit destiny)
        {
            OrbitData dataA = source.getData();
            OrbitData dataB = destiny.getData();

            float r1 = dataA.SemiMajorAxis;
            float r2 = dataB.SemiMajorAxis;
            float aTrans = (r1 + r2) / 2f;

            if (dataA.OrbitalPeriod <= 0f || dataB.OrbitalPeriod <= 0f)
            {
                Debug.LogError("Transfer period out of range");
                yield break;
            }
            
            
            // Wyliczenie MU na podstawie okresu i promienia orbity źródłowej (3 prawo Keplera)
            // T^2 = (4 * PI^2 * a^3) / mu  =>  mu = (4 * PI^2 * a^3) / T^2
            float mu = (4f * Mathf.PI * Mathf.PI * Mathf.Pow(r1, 3)) / Mathf.Pow(dataA.OrbitalPeriod, 2);

            // Czas lotu po półelipsie
            float transferTime = Mathf.PI * Mathf.Sqrt(Mathf.Pow(aTrans, 3) / mu);

            // Prędkość kątowa celu (rad/s)
            float targetSpeed = (2f * Mathf.PI) / dataB.OrbitalPeriod;

            // Wymagany kąt fazowy (kąt, pod jakim musi być cel w momencie startu)
            float requiredPhaseAngle = Mathf.PI - (targetSpeed * transferTime);
            requiredPhaseAngle = NormalizeAngle(requiredPhaseAngle);

            //Oczekiwanie na okno transferowe
            // Usuwamy pasażera z orbity źródłowej, by przestała nim poruszać
            source.RemovePassenger(passenger);

            float previousDelta = float.NaN;
            bool waitingForWindow = true;
            while (waitingForWindow)
            {
                // Obliczamy aktualny kąt fazowy między pasażerem a celem
                // (Zakładamy, że celujemy w pierwszego pasażera na orbicie docelowej)
                float currentAngleMe = passenger.CurrentAngle;
                float currentAngleTarget = destiny.GetMainPassengerAngle();
 
                float angleDiff = currentAngleTarget - currentAngleMe;
                angleDiff = NormalizeAngle(angleDiff);
 
                // Różnica względem kąta docelowego, znormalizowana do -PI..PI,
                // żeby wykryć przejście przez zero niezależnie od kierunku
                float delta = NormalizeAngleSigned(angleDiff - requiredPhaseAngle);
 
                bool closeEnough = Mathf.Abs(delta) < 0.01f;
                bool crossedZero = !float.IsNaN(previousDelta) 
                                   && Mathf.Sign(previousDelta) != Mathf.Sign(delta)
                                   && Mathf.Abs(delta - previousDelta) < Mathf.PI;
 
                if (closeEnough || crossedZero)
                {
                    waitingForWindow = false;
                }
                else
                {
                    // W fazie czekania obiekt nadal krąży po orbicie A
                    UpdateObjectPosition(passenger, source.getCenterPoint().position, r1, passenger.CurrentAngle, dataA.Inclination);
                    passenger.CurrentAngle += (2f * Mathf.PI / dataA.OrbitalPeriod) * Time.deltaTime;
                    previousDelta = delta;
                    yield return null;
                }
            }

            // Lot po elipsie transferowej
            float elapsed = 0;
            float startAngle = passenger.CurrentAngle;
            float eccentricity = (r2 - r1) / (r2 + r1);

            while (elapsed < transferTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / transferTime);

                // Kąt na elipsie od 0 do 180 stopni (PI)
                float angleOnTransfer = Mathf.PI * t;
                
                // Równanie orbity eliptycznej we współrzędnych biegunowych
                float currentRadius = aTrans * (1 - eccentricity * eccentricity) / (1 + eccentricity * Mathf.Cos(angleOnTransfer));
                
                // Interpolacja nachylenia (jeśli orbity mają różne inklinacje)
                float currentInclination = Mathf.Lerp(dataA.Inclination, dataB.Inclination, t);

                float currentAbsAngle = startAngle + angleOnTransfer;
                UpdateObjectPosition(passenger, source.getCenterPoint().position, currentRadius, currentAbsAngle, currentInclination);
                
                // Aktualizujemy kąt pasażera, żeby wiedział gdzie jest w momencie wejścia na orbitę B
                passenger.CurrentAngle = currentAbsAngle;

                yield return null;
            }

            // Wejście na orbitę docelową
            destiny.AddPassenger(passenger);
            Debug.Log("Transfer zakończony sukcesem.");
        }
        
        private static float NormalizeAngle(float angle)
        {
            // Zakres [0, 2PI)
            angle %= 2f * Mathf.PI;
            if (angle < 0f)
                angle += 2f * Mathf.PI;
            return angle;
        }
        
        private static float NormalizeAngleSigned(float angle)
        {
            // Zakres [-PI, PI)
            angle = NormalizeAngle(angle);
            if (angle >= Mathf.PI)
                angle -= 2f * Mathf.PI;
            return angle;
        }
        
        private void UpdateObjectPosition(CelestialObject passenger, Vector3 center, float radius, float angle, float inclination)
        {
            Vector3 relativePosition = OrbitPhysicsCalculator.Circular.GetPosition(radius, angle);
            Vector3 rotatedPosition = Quaternion.Euler(inclination, 0, 0) * relativePosition;
            passenger.Model.localPosition = center + rotatedPosition;
        }
    }
}
