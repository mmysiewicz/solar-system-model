using System;
using System.Collections.Generic;
using UnityEngine;


namespace SolarSystem
{
    public class Orbit : MonoBehaviour, IMovable
    {
        [SerializeField] private Transform _centerPoint;
        [SerializeField] private List<CelestialObject> _orbitPassengers = new List<CelestialObject>();
        [SerializeField] private OrbitData _data;
        
        private void Start()
        {
            //Inicjalizacja pasażerów
            foreach (var passenger in _orbitPassengers)
            {
                passenger.CurrentAngle = passenger.StartAngle;
            }
        }

        private void Update()
        {
            Move(Time.deltaTime);
        }


        public void Move(float deltaTime)
        {
            if (_data == null){return;}
            
            Vector3 positionOfCenter = _centerPoint.position;
            
            // prędkość kątowa w radianach
            // 2 * PI / okres orbitalny
            float speed = (float)(2f * Math.PI) / _data.OrbitalPeriod;
            foreach (var passenger in _orbitPassengers)
            {
                passenger.CurrentAngle += speed * deltaTime;
                
                
                //Logika wyznaczania pozycji przez wspólny kalkulator OrbitPhysicsCalculator
                Vector3 relativePosition =
                    OrbitPhysicsCalculator.Circular.GetPosition(_data.SemiMajorAxis, passenger.CurrentAngle);
                
                //Aplikacja nachylenia pasażera, według danych orbity
                Vector3 rotatedPosition = Quaternion.Euler(_data.Inclination, 0, 0) * relativePosition;
                
                //Aplikacja pozycji pasażerów wobec obiektu centralnego
                passenger.Model.position = positionOfCenter + rotatedPosition;
            }
            
        }
        public void SetData(OrbitData data) => _data = data;
        public OrbitData getData()
        {
            return _data;
        }
        public Transform getCenterPoint()
        {
            return _centerPoint;
        }
        
        // Znajduje pasażera tej orbity na podstawie jego Transform (modelu w scenie).
        // Używane przez SelectableBody do rozpoznania, który CelestialObject
        // odpowiada klikniętemu obiektowi.
        public CelestialObject GetPassengerByTransform(Transform modelTransform)
        {
            return _orbitPassengers.Find(p => p.Model == modelTransform);
        }
        
        public float GetMainPassengerAngle()
        {
            // Zwraca kąt pierwszego obiektu na orbicie (np. planety, do której lecimy)
            if (_orbitPassengers.Count > 0)
                return _orbitPassengers[0].CurrentAngle;
            return 0;
        }
        
        public void AddPassenger(CelestialObject passenger)
        {
            if (!_orbitPassengers.Contains(passenger))
                _orbitPassengers.Add(passenger);
        }
        public void RemovePassenger(CelestialObject passenger)
        {
            if (_orbitPassengers.Contains(passenger))
                _orbitPassengers.Remove(passenger);
        }
        
        private void OnEnable()
        {
            OrbitRegistry.Register(this);
        }
 
        private void OnDisable()
        {
            OrbitRegistry.Unregister(this);
        }
    }
}