using SolarSystem.View;
using UnityEngine;
 
namespace SolarSystem
{
    public class TransferSelectionManager : MonoBehaviour
    {
        public static TransferSelectionManager Instance;
 
        [SerializeField] private OrbitSelectionUI selectionUI;
 
        private CelestialObject _selectedPassenger;
        private Orbit _sourceOrbit;
 
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            
        }
 
        public bool HasActiveSelection => _selectedPassenger != null;
 
        public void OnBodySelected(Orbit sourceOrbit, CelestialObject passenger)
        {
            if (sourceOrbit == null || passenger == null)
                return;
 
            _sourceOrbit = sourceOrbit;
            _selectedPassenger = passenger;
 
            if (selectionUI == null)
            {
                Debug.LogWarning("TransferSelectionManager: brak referencji do OrbitSelectionUI.");
                return;
            }
 
            selectionUI.Show(OrbitRegistry.All, sourceOrbit, OnDestinationChosen);
        }
 
        private void OnDestinationChosen(Orbit destinationOrbit)
        {
            if (_selectedPassenger == null || _sourceOrbit == null || destinationOrbit == null)
            {
                CancelSelection();
                return;
            }
 
            if (OrbitTransferManager.Instance == null)
            {
                Debug.LogWarning("Brak OrbitTransferManager.Instance w scenie.");
                CancelSelection();
                return;
            }
 
            OrbitTransferManager.Instance.StartTransfer(_selectedPassenger, _sourceOrbit, destinationOrbit);
 
            _selectedPassenger = null;
            _sourceOrbit = null;
            selectionUI.Hide();
        }
 
        public void CancelSelection()
        {
            _selectedPassenger = null;
            _sourceOrbit = null;
            selectionUI?.Hide();
        }
    }
}