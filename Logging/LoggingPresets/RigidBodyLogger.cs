using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyLogger : Logger
{
    // Log the kinematic properties of the rigidbody this script is attached to.

    [HideInInspector] public string[] reportHeaders = new string[14] {
        "Object",
        "Position X",
        "Position Y",
        "Position Z",
        "Rotation X",
        "Rotation Y",
        "Rotation Z",
        "Rotation W",
        "LinVelocity X",
        "LinVelocity Y",
        "LinVelocity Z",
        "AngVelocity X",
        "AngVelocity Y",
        "AngVelocity Z",
    };

    Rigidbody body;

    [Header("Logger Specific Settings")] // Create a header for any logger specific settings
    [SerializeField] public string fileNamePrefix = "RB_";


    void Start()
    {
        Initialize();
        
        body = GetComponent<Rigidbody>();
    }

    public string[] getData()
    {
        string[] strings = new string[14] {
                body.name.ToString(),
                body.transform.position.x.ToString(),
                body.transform.position.y.ToString(),
                body.transform.position.z.ToString(),
                body.transform.rotation.x.ToString(),
                body.transform.rotation.y.ToString(),
                body.transform.rotation.z.ToString(),
                body.transform.rotation.w.ToString(),
                body.velocity.x.ToString(),
                body.velocity.y.ToString(),
                body.velocity.z.ToString(),
                body.angularVelocity.x.ToString(),
                body.angularVelocity.y.ToString(),
                body.angularVelocity.z.ToString(),
        };
        return strings;
    }
}
