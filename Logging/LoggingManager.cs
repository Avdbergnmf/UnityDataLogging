using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrialOrderManager))]
public class LoggingManager : MonoBehaviour
{
    // This is the central logging manager. It works together with the TrialOrderManager to start & stop logging of all the loggers assigned to this script, as well as creating their files etc.

    // Serializable
    [Tooltip("The directory to which the logs are saved, this is relative to your projects' root folder.")]
    [SerializeField] public string reportDirectoryName = "Assets/Report";
    [Tooltip("Do you also want to log the practiceruns (when TrialOrderManager.practiseState = true).")]
    [SerializeField] bool logPractice = false; 

    [Tooltip("When to write to the csv file? Setting this very low will cause lagg, setting this too high will ramp up the local memory stored and increase the risk of losing data.")]
    [SerializeField] float logEveryNSeconds = 10;
    [Tooltip("When enabled, will start all loggers at the startup. Otherwise startRepetition should be called from an external script.")]
    [SerializeField] bool logAtStartup = false;
    [Tooltip("If logging at the startup, after how much time should the loggers start? This prevents race conditions so that all loggers are properly initialized.")]
    [SerializeField] float startupLogWaitTime = 0.0f;

    public List<Logger> loggers = new List<Logger>();

    [Header("Editor Controls")]
    [Tooltip("Start logging by toggling this bool in the editor. Useful mostly for testing.")]
    [SerializeField] bool startLoggingNow = false;
    [Tooltip("Stop logging by toggling this bool in the editor. Useful mostly for testing.")]
    [SerializeField] bool stopLoggingNow = false;

    // private
    TrialOrderManager trialOrderManager = null;
    [HideInInspector] public string reportDirectoryPath;
    float timeLastLog;

    // Using this function you can stop and start the loggers from the editor by clicking the connected bools. This is only useful for testing really.
    private void OnValidate()
    {
        if (startLoggingNow)
        {
            print("Starting all connected loggers");
            startLoggingNow = false;
            startRepetition();
        }
        if (stopLoggingNow)
        {
            print("Stopping all connected loggers");
            stopLoggingNow = false;
            stopRepetition();
        }
    }


    private void Start()
    {
        timeLastLog = Time.time;
        trialOrderManager = GetComponent<TrialOrderManager>();

        if (logAtStartup)
            StartCoroutine(LateStart(startupLogWaitTime));
    }
    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        startRepetition();
    }

    private void Update()
    {
        writeAll(); // call all loggers to write

        if (Time.time - timeLastLog > logEveryNSeconds)
        {
            appendAll(); // Append to the log file
            timeLastLog = Time.time;
        }
    }

    void writeAll()
    {
        foreach (var log in loggers)
        {
            log.write();
        }
    }

    void appendAll()
    {
        foreach (var log in loggers)
        {
            log.append();
        }
    }


    public void startRepetition() // Call this function to create the logs and start the timers for all attached loggers
    {
        bool pracState = trialOrderManager.practiceState;
        int currPresetNo = trialOrderManager.getCurrPresetNo();
        int currRep = trialOrderManager.currRep;

        CreatePaths(currPresetNo, pracState);

        if (!pracState || pracState && logPractice)
        {
            CreateAll(currRep);
            setAllLogging(true); // enable the logging

            SetAllStartTime();
        }
        else
            setAllLogging(false); // disable the logging
    }

    public void stopRepetition()
    {
        setAllLogging(false); // disable the logging
        stopAll();
    }

    public void CreatePaths(int presetNo, bool practiceState = false)
    {
        string playerName = "";
        if (trialOrderManager)
            if (trialOrderManager.playerInfo.name.Length > 0)
                playerName = trialOrderManager.playerInfo.name.Replace(" ","");
            else
                Debug.LogError("Player name empty!");

        string path = reportDirectoryName;

        if (playerName.Length > 0)
        {
            path += "/" + playerName;
        }

        if (presetNo >= 0) // In this way, a negative number can be used to make logs appear in the players "root" folder
            path += "/" + presetNo.ToString(); // Log all the trials for this condition in the folder related to the loaded preset

        if (practiceState)
            path += "/practice";

        reportDirectoryPath = path;

        foreach (var log in loggers)
        {
            log.reportDirectoryPath = reportDirectoryPath;
        }
    }

    public void CreateAll(int trialNo)
    {
        foreach (var log in loggers)
        {
            log.createFile(trialNo); // trial number
        }
    }

    public void SetAllStartTime()
    {
        foreach (var log in loggers)
        {
            log.setStartTime();
        }
    }

    public void setAllLogging(bool state)
    {
        foreach (var log in loggers)
        {
            log.writeLogs = state;
        }
    }

    public void addLogger(Logger logger)
    {
        loggers.Add(logger);
    }

    public void rmLogger(Logger logger)
    {
        loggers.Remove(logger);
    }

    // Shutdown
    private void OnApplicationQuit()
    {
        stopAll();
    }
    private void OnDisable()
    {
        stopAll();
    }
    private void OnDestroy()
    {
        stopAll();
    }

    private void stopAll() // Write the last saved logs for all the loggers
    {
        foreach (var log in loggers)
        {
            log.shutDown();
        }
    }
}
