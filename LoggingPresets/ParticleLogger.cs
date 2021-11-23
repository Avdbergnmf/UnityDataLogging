using UnityDataLogging.Utility;
using UnityEngine;

namespace UnityDataLogging.LoggingPresets
{
    public class ParticleLogger : Utility.Logger
    {
        // Log how much particles are positioned in certain detectioncolliders in the scene. 
        // This is only an example, originally written for an Obi Actor from the Obi Fluid package, demonstrating how to handle datalogging when the calculation of the data is very resource intensive.
        [HideInInspector]
        public string[] reportHeaders = new string[2] {
            "Particles in Collider",
            "Total particles"
        };

        [HideInInspector]
        public int inCollider = 0;
        [HideInInspector]
        public float fraction = 1.0f;
        [HideInInspector]
        public int numParticles = 0;

        [Header("Logger Specific Settings")] // Create a header for any logger specific settings
        [SerializeField] public string fileNamePrefix = "Particle";
        [SerializeField]
        [Tooltip("Optionally: Set this > 0 to refresh the data every x seconds to prevent heavy CPU load. ")] float refreshTime = 0.1f;

        [SerializeField] Collider[] detectionColliders = null;

        //ParticleActor m_actor = null;
        private CustomFixedUpdate FU_instance;

        private string[] data;

        private void Start()
        {
            Initialize();

            //m_actor = GetComponent<ParticleActor>();

            if (refreshTime > 0.0f)
                FU_instance = new CustomFixedUpdate(refreshTime, OnFixedUpdate); //so that the updating rate can be changed as it is quite cpu intensive (a LOT of data to process)
        }

        private void Update()
        {
            if (FU_instance != null && refreshTime > 0)
                FU_instance.Update();
        }

        #region ParticleDetection

        private void OnFixedUpdate(float dt)
        {
            FixedUpdateFunc();
        }

        public void FixedUpdateFunc() // run this function to make one log (only do this at the important times, running it at regular intervals using the fixedupdate is really intensive
        {
            if (detectionColliders.Length>0)
            {
                inCollider = 0;
                foreach (var zinc in detectionColliders)
                {
                    inCollider += scanColliderForParticles(zinc); // These are quite resource intensive, might consider doing them at larger intervals
                }
            }
            else inCollider = -1; // make it -1 to show that this data is missing

            bool isLoaded = true; //m_actor.isLoaded --> Detect if the particles are loaded
            if (isLoaded)
            {
                numParticles = 1;// m_actor.particleCount;

                fraction = (inCollider / (float)numParticles);

                data = new string[2] 
                {
                    inCollider.ToString(),
                    numParticles.ToString(),
                };
            }
        }

        int scanColliderForParticles(Collider coll)
        {
            int detected = 0;

            int particleCount = 1; // m_actor.particleCount --> detect particlecount
            for (int i = 0; i < particleCount; i++)
            {
                int solverIndex = i; // m_actor.solverIndices[i] -->get the current particle

                Vector3 particlePos = Vector3.zero;// m_actor.GetParticlePosition(solverIndex) -->get the current particle position

                Vector3 CP = coll.ClosestPoint(particlePos);

                if ((CP - particlePos).magnitude == 0) detected++;
            }

            return detected;
        }
        #endregion

        public string[] GetData()
        {
            return data;
        }

    }
}