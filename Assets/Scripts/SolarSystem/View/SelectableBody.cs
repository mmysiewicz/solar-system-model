using UnityEngine;
 
namespace SolarSystem
{
    // Umieszczany na tym samym GameObjeccie co Collider modelu pasażera
    // (czyli na obiekcie wskazywanym przez CelestialObject.Model).
    // Pozwala TransferInputController rozpoznać, do której orbity
    // i którego pasażera należy kliknięty model.
    [RequireComponent(typeof(Collider))]
    public class SelectableBody : MonoBehaviour
    {
        [SerializeField] private Orbit ownerOrbit;
 
        public Orbit OwnerOrbit => ownerOrbit;
 
        public void HandleClick()
        {
            if (ownerOrbit == null)
            {
                Debug.LogWarning($"SelectableBody na '{name}': brak przypisanej orbity (ownerOrbit) w Inspectorze.");
                return;
            }
 
            CelestialObject passenger = ownerOrbit.GetPassengerByTransform(transform);
            if (passenger == null)
            {
                Debug.LogWarning($"SelectableBody na '{name}': nie znaleziono pasażera o takim Model na orbicie '{ownerOrbit.name}'.");
                return;
            }
            
            if (TransferSelectionManager.Instance == null)
            {
                Debug.LogWarning("Brak TransferSelectionManager.Instance w scenie.");
                return;
            }
 
            TransferSelectionManager.Instance.OnBodySelected(ownerOrbit, passenger);
        }
    }
}