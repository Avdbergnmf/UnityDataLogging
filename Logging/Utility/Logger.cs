using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class Logger : MonoBehaviour
{
    // Inherit from this class to create a data logger
    // The newly created class needs at least these 3 things:
    // 1 - Run the Initialize() function in the Start()
    // 2 - have a public getData() function that returns a string[] object containing the data to be logged
    // 3 - have a public string[] reportHeaders variable which contains all the headers used for the logged file (ie explains the type of data that is logged)
    // Optionally also include:
    // - A public string fileNamePrefix which will indicate which type of data is being logged in the log its filename

    LogToCSV log = null;
    [Header("File Name")]
    [Tooltip("The directory to save the logs into (this is relative to your projects' root folder). If this logger is added to the logging manager this field is unused.")]
    [SerializeField] public string reportDirectoryPath = "Assets/Report";
    private string finalFileName;

    [Tooltip("Include the name of the current gameobject in the filename of the logging file.")] 
    [SerializeField] bool includeObjectName = true;
    [Tooltip("Include the name of the player name (from the trialOrderManaer) in the filename of the logging file.")] 
    [SerializeField] bool includePlayerName = true;
    [Tooltip("Include the name of the current date in the filename of the logging file.")] 
    [SerializeField] bool includeDate = true;
    [Tooltip("Set a custom filename for this logger (ignoring any of the above options).")] 
    [SerializeField] string custom;

    [Header("Settings")]
    [Tooltip("If allowOverwrite is set to false, but the file already exists, a new file is created with a number appended to it.")] 
    [SerializeField] bool allowOverwrite = false;
    bool appendNumber = true; // Setting this to false disabled the appendnumber functionality.
    [Tooltip("If enabled, will add this instance to the LoggingManager on startup so that it can control this logger.")] 
    [SerializeField] bool addToLoggingManager = true;
    [SerializeField] public bool writeLogs = false;
    [SerializeField] bool makeFileOnStartup = false;
    [Tooltip("Use this feature to automatically append every N logs if <=0 will never automatically append.")]
    [SerializeField] int writeEveryNLogs = 0;

    //[Header("ReadOnly")]
    [HideInInspector] public string reportFileName = "";
    [HideInInspector] public bool madeFile = false;

    // Data Processing
    System.Reflection.MethodInfo getDataMethod;

    int m_N = 0;
    [HideInInspector]
    public int it // An iterator with a get function that increases its value by 1, this is used when the getData function needs to be ran multiple times using the writeN function.
    {
        get
        {
            var c_it = m_it; // save the current value

            if (m_it < m_N) // increase if below N
                m_it++;
            else
                m_it = 0; // reset if reached N

            return c_it; // return the unchanged value
        }
        set { m_it = value; }
    }
    int m_it = 0;


    [Header("Editor Controls")]
    [Tooltip("Log a single entry by clicking this bool in the editor. Useful mostly for testing.")]
    [SerializeField] bool clickToWriteSingleEntry = false;
    bool initialized = false;

    // Using this function you can stop and start the loggers from the editor by clicking the connected bools. This is only useful for testing really.
    private void OnValidate()
    {
        if (clickToWriteSingleEntry)
        {
            writeSingleEntry();
            clickToWriteSingleEntry = false;
        }
    }


    public void Initialize()
    {
        log = new LogToCSV(writeEveryNLogs);

        getDataMethod = GetType().GetMethod("getData");

        reportFileName = CreateName();
        if (makeFileOnStartup) createFile();

        // Add to the logging manager
        if (addToLoggingManager)
        {
            LoggingManager LM = GameObject.FindObjectOfType<LoggingManager>();
            if (LM)
                LM.addLogger(this); // place this logger instance in a list in the logging manager so that we can easily work with multiple different loggers
            else
                Debug.LogError("Can't find the logging manager!");
        }
        initialized = true;
    }

    #region WritingData

    public void write(bool forceWrite = false)
    {
        if (writeLogs || forceWrite)
        {
            if (!checkMadeFile()) return;

            writeCheck();
        }
    }

    public void forceWriteAndAppend()
    {
        write(true);
        append(true);
    }

    public void writeN() // Write lines using the iterator public int it (defined in this class), where the number of times it is run is defined by an int called N (defined in the log type script)
    {
        if (writeLogs)
        {
            if (!checkMadeFile()) return;

            m_N = (int)this.GetType().GetField("N").GetValue(this);

            if (m_N <= 0)
            {
                Debug.LogError("Please set a public int called N in the log type script to define how many times the getData function needs to be ran. Not Logging data");
                return;
            }

            for (int i = 0; i < m_N; i++)
                writeCheck();
        }
    }

    public void writeSingleEntry()
    {
        if (!initialized)
        {
            print("Not initialized yet, initializing now.");
            Initialize();
        }
        if (!madeFile)
        {
            print("File not created yet, creating file now.");
            createFile();
        }
        print("Writing single entry to the logs");

        forceWriteAndAppend();
    }

    void writeCheck() // Get the data, check if there is any, and write it
    {
        string[] data = (string[])getDataMethod.Invoke(this, null);

        if (data.Length > 0)
            log.appendToString(data);
        else
            Debug.LogError("Make sure to run Initialize() in the logger type script & make sure it has a public getData() function.");
    }

    public void append(bool forceWrite = false)
    {
        if (writeLogs || forceWrite)
        {
            if (!checkMadeFile()) return;

            log.append();
        }
    }

    bool checkMadeFile()
    {
        if (!madeFile)
            Debug.LogError("File not made yet!");

        return madeFile;
    }

    public void setStartTime()
    {
        log.setStartTime();
    }

    public void shutDown()
    {
        append();
    }

    #endregion

    #region FileCreation

    public void createFile(int trialNo = -1) // this is called by the logging manager once everything is set up
    {
        if (reportFileName.Length > 0)
        {
            finalFileName = reportFileName;
            if (trialNo >= 0) // only append if a number of at least 0 is given
                finalFileName += "_" + trialNo.ToString();

            createReport(finalFileName);
            madeFile = true;
        }
        else
        {
            Debug.LogError("FILE NAME NOT SET BEFORE CREATING LOG");
            madeFile = false;
        }
    }
    public void VerifyDirectory()
    {
        string dir = reportDirectoryPath;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    void createReport(string fileName)
    {
        VerifyDirectory();
        if (File.Exists(GetFilePath(fileName)))
        {
            Debug.Log("<color=red>WARNING: </color>overwriting previous report! With name: " + fileName);
            File.Delete(GetFilePath(fileName));
        }
        string[] localReportHeaders = (string[])this.GetType().GetField("reportHeaders").GetValue(this);
        if (localReportHeaders.Length > 0)
            log.createFileWithHeaders(GetFilePath(fileName), localReportHeaders);
        else
            Debug.LogError("Please define the reportHeaders in the logger type script.");
    }

    public string GetFilePath(string fileName)
    {
        return reportDirectoryPath + "/" + fileName + ".csv";
    }

    public string CreateName()
    {
        if (custom.Length > 0)
        {
            reportFileName = custom;
        }
        else if (includePlayerName || includeObjectName || includeDate)
        {
            string prefix = (string)this.GetType().GetField("fileNamePrefix").GetValue(this);

            if (prefix.Length > 0)
                reportFileName = prefix;
            else
                Debug.Log("Please set a fileNamePrefix in the logger type script.");

            if (includeObjectName)
                reportFileName += name + "_";
            if (includePlayerName)
            {
                PlayerInfo playerInfo = FindObjectOfType<TrialOrderManager>().playerInfo;
                if (playerInfo != null)
                    if (playerInfo.name.Length > 0)
                        reportFileName += playerInfo.name + "_";
                    else
                        Debug.LogError("Player name empty!");
            }
            if (includeDate)
                reportFileName += DateTime.Today.ToShortDateString().Replace("/", "_");

        }
        else
        {
            Debug.LogError("Log file name empty, cannot log data");
            return null;
        }
        if (File.Exists(GetFilePath(reportFileName)))
        {
            if (allowOverwrite)
            {
                Debug.Log("File already exists, but overwriting. DELETING old file.");
                File.Delete(GetFilePath(reportFileName));
            }
            else
            {
                if (appendNumber)
                {
                    Debug.Log("File already exists, but appending number to filename & writing logs anyway");

                    int number = 0;
                    while (File.Exists(GetFilePath(reportFileName)))
                    {
                        reportFileName += "_" + number++.ToString();

                        if (File.Exists(GetFilePath(reportFileName)))
                            reportFileName = reportFileName.Remove(reportFileName.Length - 2);
                    }
                }
                else
                {
                    Debug.Log("File already exists, not overwriting, and NOT logging data.");
                    reportFileName = "";
                    return null;
                }
            }
        }
        return reportFileName;
    }

    #endregion
}
