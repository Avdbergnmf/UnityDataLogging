using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LogToCSV
{
    // Settings

    string reportSeparator = ",";
    string timeStampHeader = "Time Stamp";

    public int writeEveryNLogs; // use this feature to automatically append every N logs if <=0 will never automatically append.
    public string FileName;

    float startingTime = 0f;

    // Stuff that is used to keep track
    string fullFinalString = "";
    int unsavedLogNo = 0;

    public LogToCSV(int WriteEveryNLogs = 0) // Constructor
    {
        writeEveryNLogs = WriteEveryNLogs;
    }

    #region Interactions

    public void append()
    {
        using (StreamWriter sw = File.AppendText(FileName))
        {
            sw.Write(fullFinalString);
            fullFinalString = "";
            unsavedLogNo = 0;
        }
    }

    public void appendToString(string[] strings)
    {
        string finalString = "";
        for (int i = 0; i < strings.Length; i++)
        {
            if (finalString != "")
            {
                finalString += reportSeparator;
            }
            finalString += strings[i];
        }
        finalString += reportSeparator + getTimeStamp();
        finalString += "\n";
        fullFinalString += finalString;

        checkAppend();
    }

    void checkAppend()
    {
        // Auto Append
        if (writeEveryNLogs != 0)
        {
            unsavedLogNo++;
            if (unsavedLogNo >= writeEveryNLogs)
                append();
        }
    }

    #endregion


    #region Create

    public void createFileWithHeaders(string csvFile, string[] headers)
    {
        FileName = csvFile; // Set the file name

        using (StreamWriter sw = File.CreateText(FileName))
        {
            string finalString = "";
            for (int i = 0; i < headers.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += headers[i];
            }
            finalString += reportSeparator + timeStampHeader;
            sw.WriteLine(finalString);
        }
    }

    #endregion

    #region Queries
    public string getTimeStamp() // time since the log file was created
    {
        return (Time.time - startingTime).ToString();
    }

    public void setStartTime()
    {
        startingTime = Time.time;
    }

    #endregion
}

