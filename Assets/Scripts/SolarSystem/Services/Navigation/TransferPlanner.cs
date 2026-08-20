using System.Collections.Generic;
using UnityEngine;

namespace SolarSystem
{
    
    // Buduje plan transeru pomiędzy dwiema dowolnymi orbitami w hierarchii
    // Układu Słonecznego, dzieląc go na etapu (TransferLeg) obsługiwane
    // przez odpowiednie manewry (ITransferManeuver)
    
    public static class TransferPlanner
    {
        // Zarejestrowane rodzaje manewrów, sprawdzane w podanej kolejności
        // Pierwszy manewr który zwróci CanHandle == true, zostaje wykorzystany
        private static readonly List<ITransferManeuver> Maneuvers =  new List<ITransferManeuver>
        {
            new HohmannTransferManeuver(),
            new SphereOfInfluenceTransferManeuver()
        };

        public static List<TransferPart> BuildPlan(Orbit source, Orbit destiny)
        {
            var plan = new List<TransferPart>();
            
            if (source == null || destiny == null)
            {
                Debug.LogError("TransferPlanner: source orbit or destiny orbit is null");
                return plan;
            }
            
            if (source == destiny)
            {
                Debug.LogWarning("TransferPlanner: source orbit and destiny orbit are same");
                return plan;
            }

            List<Orbit> pathA = GetPathToRoot(source);
            List<Orbit> pathB = GetPathToRoot(destiny);

            int bestI = -1;
            int bestJ = -1;
            for (int i = 0; i < pathA.Count && bestI < 0; i++)
            {
                for (int j = 0; j < pathB.Count; j++)
                {
                    if (pathA[i].getCenterPoint() == pathB[j].getCenterPoint())
                    {
                        bestI = i;
                        bestJ = j;
                        break;
                    }
                }
            }

            if (bestI < 0)
            {
                Debug.LogError("TransferPlanner: orbits don't have a shared center point");
                return plan;
            }

            for (int i = 0; i < bestI; i++)
            {
                AddPart(plan, pathA[i], pathA[i+1]);
            }
            
            AddPart(plan, pathA[bestI], pathB[bestJ]);
            
            for (int j = bestJ; j > 0; j--)
            {
                AddPart(plan, pathB[j], pathB[j-1]);
            }

            return plan;
        }

        private static void AddPart(List<TransferPart> plan, Orbit from, Orbit to)
        {
            if(from == to)
                return;
            
            ITransferManeuver maneuver = Maneuvers.Find(m => m.CanHandle(from, to));
            if (maneuver == null)
            {
                Debug.LogError("TransferPlanner: cannot find maneuver serving those orbits");
                return;
            }
            
            plan.Add(new TransferPart(from, to, maneuver));
        }
        
        
        private static List<Orbit> GetPathToRoot(Orbit orbit)
        {
            var path = new List<Orbit>();
            Orbit current = orbit;
            while (current != null)
            {
                path.Add(current);
                current = current.Parent;
            }
            return path;
        }
    }
}