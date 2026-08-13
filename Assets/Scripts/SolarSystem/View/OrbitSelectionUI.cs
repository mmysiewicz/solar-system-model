using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
namespace SolarSystem.View
{
    // Prosty panel UI wyświetlający listę orbit, na które można wykonać transfer.
    // Wymaga: panelRoot (obiekt panelu, włączany/wyłączany), buttonsContainer
    // (np. Vertical Layout Group) i buttonPrefab (przycisk z komponentem Text).
    public class OrbitSelectionUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform buttonsContainer;
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private Button cancelButton;
 
        private readonly List<GameObject> _spawnedButtons = new List<GameObject>();
        private Action<Orbit> _onChosen;
 
        private void Awake()
        {
            if (cancelButton != null)
                cancelButton.onClick.AddListener(() => TransferSelectionManager.Instance?.CancelSelection());
 
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }
 
        public void Show(IReadOnlyList<Orbit> allOrbits, Orbit excludeSource, Action<Orbit> onChosen)
        {
            //
            Debug.Log($"[UI] Metoda Show wywołana! Liczba przekazanych orbit: {(allOrbits != null ? allOrbits.Count : "NULL")}");

            if (panelRoot == null || buttonsContainer == null || buttonPrefab == null)
            {
                Debug.LogWarning("[UI] Brak przypisanych referencji w Inspectorze!");
                return;
            }

            ClearButtons();
            _onChosen = onChosen;
            panelRoot.SetActive(true);

            if (allOrbits == null || allOrbits.Count == 0)
            {
                Debug.LogWarning("[UI] Lista allOrbits jest PUSTA!");
                return;
            }

            foreach (Orbit orbit1 in allOrbits)
            {
                if (orbit1 == null)
                {
                    Debug.Log("[UI] Pominięto orbitę, bo jest NULL");
                    continue;
                }

                if (orbit1 == excludeSource)
                {
                    Debug.Log($"[UI] Pominięto orbitę {orbit1.name}, bo to orbita źródłowa (excludeSource)");
                    continue;
                }

                Debug.Log($"[UI] Sukces! Tworzę przycisk dla orbity: {orbit1.name}");
                //






                if (panelRoot == null || buttonsContainer == null || buttonPrefab == null)
                {
                    Debug.LogWarning("OrbitSelectionUI: brakuje przypisanych referencji w Inspectorze.");
                    return;
                }

                ClearButtons();
                _onChosen = onChosen;
                panelRoot.SetActive(true);

                foreach (Orbit orbit in allOrbits)
                {
                    if (orbit == null || orbit == excludeSource)
                        continue;

                    Button button = Instantiate(buttonPrefab, buttonsContainer, false);

                    button.transform.localScale = Vector3.one;
                    button.transform.localPosition = Vector3.zero;

                    OrbitData data = orbit.getData();
                    string orbitName = data != null ? data.name : orbit.name;


                    TMP_Text tmpLabel = button.GetComponentInChildren<TMP_Text>();
                    if (tmpLabel != null)
                    {
                        tmpLabel.text = orbitName;
                    }
                    else
                    {
                        Text legacyLabel = button.GetComponentInChildren<Text>();
                        if (legacyLabel != null)
                        {
                            legacyLabel.text = orbitName;
                        }
                    }

                    Orbit capturedOrbit = orbit; // uniknięcie problemu z domknięciem w pętli
                    button.onClick.AddListener(() => Choose(capturedOrbit));

                    _spawnedButtons.Add(button.gameObject);
                }
            }
        }
 
        private void Choose(Orbit orbit)
        {
            _onChosen?.Invoke(orbit);
        }
 
        public void Hide()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
 
            ClearButtons();
            _onChosen = null;
        }
 
        private void ClearButtons()
        {
            foreach (GameObject go in _spawnedButtons)
                Destroy(go);
            _spawnedButtons.Clear();
        }
    }
}