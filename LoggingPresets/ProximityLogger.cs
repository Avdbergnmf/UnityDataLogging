using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace UnityDataLogging.LoggingPresets
{
    public class ProximityLogger : Utility.Logger
    {
        // Log the proximity of the transform of this gameobject to the closest member of a list of colliders.
        [HideInInspector] public string[] reportHeaders = new string[6]
        {
            "Transform Name",
            "Closest collider",
            "Distance X",
            "Distance Y",
            "Distance Z",
            "Magnitude"
        };

        [Header("Logger Specific Settings")] // Create a header for any logger specific settings
        [SerializeField] public string fileNamePrefix = "PROX";

        [Tooltip("The colliders for which the proximity is evaluated.")] public List<Collider> colliderCheckList = null;
        [Tooltip("The GameObjects for which the children will be checked for colliders for each of which the proximity is evaluated.")] public List<GameObject> checkChildrenColliders = null;

        void Start()
        {
            Initialize();

            foreach (GameObject GO in checkChildrenColliders)
            {
                if (GO) // check if its assigned
                {
                    Collider[] colliders = GO.GetComponentsInChildren<Collider>();

                    foreach (Collider coll in colliders)
                    {
                        colliderCheckList.Add(coll); // Add all the child colliders
                    }
                }
            }

            Debug.Log("Number of colliders checked for proximity: " + colliderCheckList.Count.ToString());
        }

        public string[] GetData()
        {
            // Logged variables
            Vector3 CP = Vector3.positiveInfinity; // The closest point found (from/rel to the current transform point)
            string closestColl = ""; // the name of the closest collider

            // Current position checked
            Vector3 pos = transform.position; // the position vector of the currently checked transform

            foreach (Collider coll in colliderCheckList)
            {
                Vector3 CPCurr = coll.ClosestPoint(pos);
                Vector3 currDistVec = CPCurr - pos;
                if (currDistVec.magnitude < CP.magnitude) // check if the currently checked collider is the closest one
                { // if it is, set the closest point to that one
                    CP = currDistVec;
                    closestColl = coll.gameObject.name;
                }
            }

            // Make the to be logged string
            string[] strings = new string[6]
            {
                name,
                closestColl,
                CP.x.ToString(CultureInfo.InvariantCulture),
                CP.y.ToString(CultureInfo.InvariantCulture),
                CP.z.ToString(CultureInfo.InvariantCulture),
                CP.magnitude.ToString(CultureInfo.InvariantCulture)
            };
            return strings;
        }
    }
}
