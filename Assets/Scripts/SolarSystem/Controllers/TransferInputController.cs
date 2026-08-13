using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
 
namespace SolarSystem.Controllers
{
    // Jedno centralne miejsce obsługi kliknięcia myszą na obiekty w scenie 3D.
    // Zamiast OnMouseDown na każdym obiekcie z osobna (co utrudnia kontrolę
    // nad kolejnością zdarzeń i konflikty z UI), robimy jeden raycast na klatkę.
    public class TransferInputController : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private LayerMask clickableLayerMask = ~0; // domyślnie wszystkie warstwy
        [SerializeField] private float maxRayDistance = 10000f;
 
        private void Awake()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
        }
 
        private void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;
 
            // Nie przetwarzamy kliknięcia w świecie 3D, jeśli gracz kliknął w element UI
            // (np. panel wyboru orbity docelowej).
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
 
            if (targetCamera == null)
                return;
 
            Ray ray = targetCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, clickableLayerMask))
            {
                SelectableBody selectable = hit.collider.GetComponent<SelectableBody>();
                if (selectable != null)
                {
                    selectable.HandleClick();
                }
                else
                {
                    // Kliknięcie w coś, co nie jest wybieralnym obiektem — anulujemy trwający wybór.
                    TransferSelectionManager.Instance?.CancelSelection();
                }
            }
            else
            {
                // Kliknięcie w pustkę — anulujemy trwający wybór.
                TransferSelectionManager.Instance?.CancelSelection();
            }
        }
    }
}