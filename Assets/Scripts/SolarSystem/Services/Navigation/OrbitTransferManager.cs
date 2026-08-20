using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarSystem
{
    // Punkt uruchamiania transferów międzyorbitalnych
    // Deleguje zlecenie manewru do TransferPlanner
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

        public bool IsTransferInProgress { get; private set; }

        public void StartTransfer(CelestialObject passenger, Orbit source, Orbit destiny)
        {
            if (IsTransferInProgress)
            {
                Debug.LogWarning("OrbitTransferManager: Transfer is already in progress.");
                return;
            }

            List<TransferPart> plan = TransferPlanner.BuildPlan(source, destiny);
            if (plan.Count == 0)
            {
                Debug.LogWarning("OrbitTransferManager: BuildPlan error");
                return;
            }

            StartCoroutine(ExecutePlan(passenger, plan));
        }

        private IEnumerator ExecutePlan(CelestialObject passenger, List<TransferPart> plan)
        {
            IsTransferInProgress = true;

            foreach (TransferPart part in plan)
            {
                yield return StartCoroutine(part.Maneuver.Execute(passenger, part.From, part.To));
            }
            IsTransferInProgress = false;
            Debug.Log("OrbitTransferManager: Execute plan success");
        }
    }
}
