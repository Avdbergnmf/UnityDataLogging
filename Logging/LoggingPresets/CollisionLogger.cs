using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionLogger : Logger
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
    [SerializeField] public string fileNamePrefix = "COLL_";

    // Save the collisions happening in to this gameobject in public variables so that they can easily be accessed by external scripts.
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

    public string[] getData()
    {
        string[] strings = new string[5] { 
                name.ToString(),
                collisionPoints[0].x.ToString(), // Just logging the first collision point in the array
                collisionPoints[0].y.ToString(),
                collisionPoints[0].z.ToString(),
                collidingBodyName
        };
        return strings;
    }
}
