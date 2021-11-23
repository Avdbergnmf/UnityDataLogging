using System.Globalization;
using UnityEngine;

namespace UnityDataLogging.LoggingPresets
{
    [RequireComponent(typeof(Rigidbody))]
    public class GameObjectLogger : Utility.Logger
    {
        // Log the kinematic properties of the rigidbody this script is attached to.

        [HideInInspector] public string[] reportHeaders = new string[7] {
            "Position X",
            "Position Y",
            "Position Z",
            "Rotation X",
            "Rotation Y",
            "Rotation Z",
            "Rotation W",
        };

        [Header("Logger Specific Settings")] // Create a header for any logger specific settings
        [SerializeField] public string fileNamePrefix = "GO";


        public void Start()
        {
            Initialize(); 
        }

        public string[] GetData()
        {
            string[] strings = new string[7] {
                this.transform.position.x.ToString(CultureInfo.InvariantCulture),
                this.transform.position.y.ToString(CultureInfo.InvariantCulture),
                this.transform.position.z.ToString(CultureInfo.InvariantCulture),
                this.transform.rotation.x.ToString(CultureInfo.InvariantCulture),
                this.transform.rotation.y.ToString(CultureInfo.InvariantCulture),
                this.transform.rotation.z.ToString(CultureInfo.InvariantCulture),
                this.transform.rotation.w.ToString(CultureInfo.InvariantCulture),
            };
            return strings;
        }
    }
}
