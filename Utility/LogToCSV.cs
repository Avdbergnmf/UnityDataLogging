using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityDataLogging.Utility
{
    public class LogToCSV // By Alex van den Berg - https://github.com/Avdbergnmf/UnityDataLogging
    {
        // Settings
        public bool FileCreated = false;
        private string _filePath;
        private const char Separator = ',';

        private string timeStampHeader = "Time Stamp [s]";

        private readonly TimeManager _timeManager;

        private float _startingTime = 0f;

        private StringBuilder _content = new StringBuilder();
        private int _unsavedLogNo = 0;
        private int _writeEveryNLogs = 0;

        public LogToCSV(TimeManager timeManager, int writeEveryNLogs = 0) // Set the data in string[] record using FillRecord, data will be taken from here and logged to a csv file.
        {
            this._timeManager = timeManager;
            _writeEveryNLogs = writeEveryNLogs;
        }
    
        #region Interactions
    
        public void CreateFileWithHeaders(string filePath, string[] headers)
        {
            if (!FileCreated)
            {
                _filePath = filePath; // Set the file path

                using (StreamWriter sw = File.CreateText(_filePath))
                {
                    string finalString = "";
                    foreach (var header in headers)
                    {
                        if (finalString != "")
                        {
                            finalString += Separator;
                        }

                        finalString += header;
                    }

                    finalString += Separator + timeStampHeader;
                    sw.WriteLine(finalString);
                }

                FileCreated = true;
            }
            else
            {
                Debug.Log("Tried to create file, but file was already created.");
            }
        }
    

        public void Write(string[] strings, bool forceWrite = false)
        {
            if (!FileCreated && !forceWrite)
            {
                Debug.LogError("Create file before writing!");
                return;
            }
            _content.Append(GetTimeStamp() + Separator);
            foreach (var s in strings)
                _content.Append(s + Separator);
            _content.Append("\n");

            if (_writeEveryNLogs != 0)
            {
                _unsavedLogNo++;
                if (_unsavedLogNo >= _writeEveryNLogs)
                    WriteToFile();
            }
            else
                WriteToFile();
        }

        void WriteToFile()
        {
            using (StreamWriter file = new StreamWriter(_filePath, true))
            {
                file.Write(_content);
                _content.Length = 0;
                _content.Capacity = 0;
                _unsavedLogNo = 0; 
            }
        }

        public void Stop()
        {
            Debug.Log("Stopping logger: " + _filePath);
            WriteToFile();
            FileCreated = false;
        }

        #endregion

        #region Queries
        private string GetTimeStamp() // time since the log file was created
        {
            return (_timeManager.SystemTime - _startingTime).ToString(CultureInfo.InvariantCulture);
        }

        private void SetStartTime()
        {
            _startingTime = _timeManager.SystemTime;
        }

        #endregion
    }
}

