using System.Globalization;
using UnityEngine;

namespace UnityDataLogging.LoggingPresets
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidBodyLogger : Utility.Logger
    {
        // Log the kinematic properties of the rigidbody this script is attached to.

        [HideInInspector] public string[] reportHeaders = new string[13] {
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

        Rigidbody _body;
        Vector3 _bodyPos;
        Quaternion _bodyRot;

        [Header("Logger Specific Settings")] // Create a header for any logger specific settings
        [SerializeField] public string fileNamePrefix = "RB";


        public void Start()
        {
            _body = GetComponent<Rigidbody>();
            _bodyPos = _body.transform.position;
            _bodyRot = _body.transform.rotation;
            Initialize();
        }

        public string[] GetData()
        {
            string[] strings = new string[13] {
                _bodyPos.x.ToString(CultureInfo.InvariantCulture),
                _bodyPos.y.ToString(CultureInfo.InvariantCulture),
                _bodyPos.z.ToString(CultureInfo.InvariantCulture),
                _bodyRot.x.ToString(CultureInfo.InvariantCulture),
                _bodyRot.y.ToString(CultureInfo.InvariantCulture),
                _bodyRot.z.ToString(CultureInfo.InvariantCulture),
                _bodyRot.w.ToString(CultureInfo.InvariantCulture),
                _body.velocity.x.ToString(CultureInfo.InvariantCulture),
                _body.velocity.y.ToString(CultureInfo.InvariantCulture),
                _body.velocity.z.ToString(CultureInfo.InvariantCulture),
                _body.angularVelocity.x.ToString(CultureInfo.InvariantCulture),
                _body.angularVelocity.y.ToString(CultureInfo.InvariantCulture),
                _body.angularVelocity.z.ToString(CultureInfo.InvariantCulture),
            };
            return strings;
        }
    }
}
