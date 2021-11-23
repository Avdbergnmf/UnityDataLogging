using System.Globalization;
using UnityEngine;

namespace UnityDataLogging.LoggingPresets
{
    [RequireComponent(typeof(Collider))]
    public class CollisionLogger : Utility.Logger
    {
        // Log the collisions as found by the CollisionLoggingManager

        [HideInInspector]
        public string[] reportHeaders = new string[5] {
            "Object",
            "Collision Point X",
            "Collision Point Y",
            "Collision Point Z",
            "Colliding Body",
        };

        [Header("Logger Specific Settings")] // Create a header for any logger specific settings
        [SerializeField] public string fileNamePrefix = "COLL";

        // Save the collisions happening in to this gameobject in public variables so that they can easily be accessed by external scripts. (Can be handy so you don't need a separate script for this).
        [HideInInspector] public string collidingBodyName = "";
        [HideInInspector] public Vector3[] collisionPoints;
        [HideInInspector] public Vector3[] collisionNormals;

        void Start()
        {
            Initialize();
        }

        private void OnCollisionEnter(Collision collision)
        {
            GameObject collidingBody = collision.collider.gameObject;
            collidingBodyName = collidingBody.name;

            collisionPoints = new Vector3[collision.contacts.Length];
            collisionNormals = new Vector3[collision.contacts.Length];
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                collisionPoints[i] = collision.contacts[i].point;
                collisionNormals[i] = collision.contacts[i].normal;
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            collisionPoints = new Vector3[collision.contacts.Length];

            collisionPoints = new Vector3[collision.contacts.Length];
            collisionNormals = new Vector3[collision.contacts.Length];
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                collisionPoints[i] = collision.contacts[i].point;
                collisionNormals[i] = collision.contacts[i].normal;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            collidingBodyName = "";
        }

        public string[] GetData()
        {
            string[] strings = new string[5] { 
                name.ToString(),
                collisionPoints[0].x.ToString(CultureInfo.InvariantCulture), // Just logging the first collision point in the array
                collisionPoints[0].y.ToString(CultureInfo.InvariantCulture),
                collisionPoints[0].z.ToString(CultureInfo.InvariantCulture),
                collidingBodyName
            };
            return strings;
        }
    }
}
