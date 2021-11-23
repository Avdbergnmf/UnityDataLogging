using System;
using System.IO;
using UnityEngine;

namespace UnityDataLogging.Utility
{
    public class Logger : MonoBehaviour // By Alex van den Berg - https://github.com/Avdbergnmf/UnityDataLogging
    {
        // Inherit from this class to create a data logger
        // The newly created class needs at least these 3 things:
        // 1 - Run the Initialize() function in the Start()
        // 2 - have a public getData() function that returns a string[] object containing the data to be logged
        // 3 - have a public string[] reportHeaders variable which contains all the headers used for the logged file (ie explains the type of data that is logged)
        // Optionally also include:
        // - A public string fileNamePrefix which will indicate which type of data is being logged in the log its filename

        private LogToCSV _logToCsv;
        [Header("Folder")]
        [SerializeField] private bool useDataPath = false; // Whether to use the dataPath
        [SerializeField] private bool usePersistent = false; // Whether to use the persistentDataPath
        [Tooltip("Folder to use when useDataPath is not selected")] public string customSaveFolder = "";
    
        [Header("File Name")]
        [Tooltip("Include the name of the current gameObject in the filename of the logging file.")] 
        [SerializeField]
        private bool includeObjectName = true;
        [Tooltip("Include the name of the current date in the filename of the logging file.")] 
        [SerializeField]
        private bool includeDate = true;
        [Tooltip("Set a custom filename for this logger (ignoring any of the above options).")] 
        [SerializeField]
        private string custom;

        [Header("Settings")]
        [Tooltip("Use this feature to automatically append every N logs. If =0 will append every time (may induce lagg).")]
        [SerializeField] int writeEveryNLogs = 0;
        [Tooltip("If allowOverwrite is set to false, but the file already exists, a new file is created with a number appended to it.")] 
        [SerializeField]
        private bool allowOverwrite = false;
        [SerializeField] private bool appendNumberIfExists = true; // Setting this to false disabled the appendNumber functionality.
        [Tooltip("If enabled, will add this instance to the LoggingManager on startup so that it can control this logger.")] 
        [SerializeField]
        private bool addToLoggingManager = true;
        [SerializeField] public bool writeLogs = false;
    
        public string fileName = "";

        // Data Processing
        private System.Reflection.MethodInfo _getDataMethod;
    
        [Header("Editor Controls")]
        [Tooltip("Log a single entry by clicking this bool in the editor. Useful mostly for testing.")]
        [SerializeField] bool writeSingleRecord = false;
    
        // Using this function you can log a single entry from the editor by clicking the connected bool. This is only useful for testing really.
        private void OnValidate()
        {
            if (writeSingleRecord)
            {
                WriteSingleRecord();
                writeSingleRecord = false;
            }
        }

        private void OnApplicationQuit()
        {
            StopRecording();
        }
    
        public void Write(bool forceWrite = false)
        {
            if (writeLogs || forceWrite)
            {
                string[] data = (string[])_getDataMethod.Invoke(this, null);

                if (data.Length > 0)
                    _logToCsv.Write(data, forceWrite);
                else
                    Debug.LogError("Make sure to run Initialize() in the logger preset script & make sure it has a public getData() function.");
            }
        }

        protected void Initialize()
        {
            TimeManager tm = FindObjectOfType<TimeManager>();

            _logToCsv = new LogToCSV(tm, writeEveryNLogs);

            _getDataMethod = GetType().GetMethod("GetData");

            // Add to the logging manager
            if (addToLoggingManager)
            {
                LoggingManager LM = GameObject.FindObjectOfType<LoggingManager>();
                if (LM)
                    LM.AddLogger(this); // place this logger instance in a list in the logging manager so that we can easily work with multiple different loggers
                else
                    Debug.LogError("Can't find the logging manager!");
            }

            fileName = GenerateName();
            Debug.Log("INITIALIZED");
        }

        private void CreateFiles()
        {
            if (VerifyDirectory())
            {
                if (File.Exists(GetFilePath(fileName)))
                    Debug.LogError("<color=red>WARNING: </color>Report already exists: " + fileName + "\nNot Creating...");
                else
                {
                    string[] localReportHeaders = (string[])this.GetType().GetField("reportHeaders").GetValue(this);
                    _logToCsv.CreateFileWithHeaders(GetFilePath(fileName), localReportHeaders);
                }
            }
            else
                Debug.Log("Couldn't create files.");
        }

        #region WritingData

        public void StartRecording()
        {
            CreateFiles();
            writeLogs = true;
        }

        public void StopRecording()
        {
            _logToCsv.Stop();
            writeLogs = false;
        }

        public void WriteSingleRecord()
        {
            Debug.Log("Writing single log to: " + GetFilePath(fileName));
            if (!File.Exists(GetFilePath(fileName)))
            {
                StartRecording();
                Write(true);
                StopRecording();
            }
            else // If it does exist, and we want to write a single record, we probably just want to append to it.
            {
                Debug.Log("File already exists, appending to existing file."); // This is fine as this is for debugging only anyway...
                Write(true);
            }
        }

        #endregion

        #region FilePathDefining

        private bool VerifyDirectory()
        {
            string dir = GetSaveFolder();
            if (dir == "")
                return false;
            else
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return true;
            }
        }
    
        #endregion

        #region GetInfoUtility
    
        private string GetFilePath(string newFileName)
        {
            return GetSaveFolder() + Path.DirectorySeparatorChar + newFileName + ".csv";
        }

        private string GetSaveFolder()
        {
            if (useDataPath)
                return GetDataPath();
            else
            {
                if (customSaveFolder.Length > 0)
                    return customSaveFolder;
                else
                {
                    Debug.LogError("Not using dataPath, but no custom save folder defined");
                    return "";
                }
            }
        }
    
        private string GetDataPath()
        {
            if (usePersistent) return Application.persistentDataPath;
            else return Application.dataPath;
        }

        private string GenerateName()
        {
            string newFileName = "";

            if (custom.Length > 0)
            {
                newFileName = custom;
            }
            else if (includeObjectName || includeDate)
            {
                string prefix = (string)this.GetType().GetField("fileNamePrefix").GetValue(this);

                if (prefix.Length > 0)
                    newFileName = prefix;
                else
                    Debug.Log("Please set a fileNamePrefix in the logger preset script.");

                if (includeObjectName)
                    newFileName += AddSeparator(name);
                if (includeDate)
                    newFileName += DateTime.Today.ToShortDateString().Replace("/", "_");
            }
            else
            {
                Debug.LogError("Log file name empty, cannot log data");
                return null;
            }

            if (File.Exists(GetFilePath(newFileName)))
            {
                if (allowOverwrite)
                {
                    Debug.Log("File already exists, but overwriting. DELETING old file.");
                    File.Delete(GetFilePath(newFileName));
                }
                else
                {
                    if (appendNumberIfExists)
                    {
                        Debug.Log("File already exists, but appending number to filename & writing logs anyway");

                        int number = 0;
                        while (File.Exists(GetFilePath(newFileName)))
                        {
                            string testFileName = AddSeparator(newFileName);
                            testFileName += number++.ToString();

                            if (!File.Exists(GetFilePath(testFileName)))
                                newFileName = testFileName;
                        }
                    }
                    else
                    {
                        Debug.Log("File already exists, not overwriting, and NOT logging data.");
                        newFileName = "";
                        return null;
                    }
                }
            }
            return newFileName;
        }

        private string AddSeparator(string str)
        {
            if (str.Length > 0)
                str += "_";

            return str;
        }
    
        #endregion
    }
}
