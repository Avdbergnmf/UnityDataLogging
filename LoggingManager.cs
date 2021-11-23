using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = UnityDataLogging.Utility.Logger;

namespace UnityDataLogging
{
    public class LoggingManager : MonoBehaviour // By Alex van den Berg - https://github.com/Avdbergnmf/UnityDataLogging
    {
        // This is the central logging manager. It has functionality to start & stop logging of all the loggers in the List<Logger>, as well as creating their files etc.
        [Header("Main Settings")]
        public List<Logger> loggers = new List<Logger>();
        [Tooltip("When enabled, will start all loggers at the startup. Otherwise startRepetition should be called from an external script (or using the editorcontrols).")]
        [SerializeField] bool logAtStartup = false;
        [Tooltip("If logging at the startup, after how much time should the loggers start? This prevents race conditions so that all loggers are properly initialized.")]
        [SerializeField] float startupLogWaitTime = 0.0f;


        [Header("Editor Controls")]
        [Tooltip("Write single log by toggling this bool in the editor. Useful mostly for testing.")]
        [SerializeField] bool writeSingleRecord = false;
        [Tooltip("Start logging by toggling this bool in the editor. Useful mostly for testing.")]
        [SerializeField] bool startLoggingNow = false;
        [Tooltip("Stop logging by toggling this bool in the editor. Useful mostly for testing.")]
        [SerializeField] bool stopLoggingNow = false;

        // Using this function you can stop and start the loggers from the editor by clicking the connected bools. This is only useful for testing really.
        private void OnValidate()
        {
            if (writeSingleRecord)
            {
                print("Writing a single log.");
                writeSingleRecord = false;
                WriteSingleLog();
            }
            if (startLoggingNow)
            {
                print("Starting all connected loggers");
                startLoggingNow = false;
                StartRecording();
            }
            if (stopLoggingNow)
            {
                print("Stopping all connected loggers");
                stopLoggingNow = false;
                StopRecording();
            }
        }


        private void Start()
        {
            if (logAtStartup)
                StartCoroutine(LateStart(startupLogWaitTime));
        }
        IEnumerator LateStart(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            StartRecording();
        }

        private void Update()
        {
            WriteAll(); // call all loggers to write
        }

        #region Configuration


        public void AddLogger(Logger logger)
        {
            loggers.Add(logger);
        }

        public void RmLogger(Logger logger)
        {
            loggers.Remove(logger);
        }


        #endregion

        #region WritingFunctions

        private void WriteSingleLog()
        {
            foreach (var log in loggers)
            {
                log.WriteSingleRecord();
            }
        }

        void WriteAll(bool forceWrite = false)
        {
            foreach (var log in loggers)
            {
                log.Write(forceWrite);
            }
        }

        #endregion

        #region LoggingControl

        private void StartRecording() // Call this function to create the logs and start the timers for all attached loggers
        {
            foreach (var log in loggers)
                log.StartRecording(); // trial number
        }

        private void StopRecording()
        {
            foreach (var log in loggers)
                log.StopRecording();
        }
        
        #endregion
        #region Shutdown
        private void OnApplicationQuit()
        {
            StopRecording();
        }
        private void OnDisable()
        {
            StopRecording();
        }
        private void OnDestroy()
        {
            StopRecording();
        }
        #endregion
    }
}
