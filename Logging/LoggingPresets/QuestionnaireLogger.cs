using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QuestionnaireLogger : Logger
{
    // Log the canvas items selected. 

    [HideInInspector]
    public string[] reportHeaders = new string[2]
    {
        "Question",
        "Value"
    };

    [Header("Logger Specific Settings")] // Create a header for any logger specific settings
    [SerializeField] public string fileNamePrefix = "Q_";

    [SerializeField] Slider[] sliders = null;
    [SerializeField] InputField[] inputFields = null;
    [SerializeField] Toggle[] toggles = null;
    [SerializeField] ToggleGroup[] toggleGroups = null;
    [SerializeField] Dropdown[] dropdowns = null;
    [Tooltip("Optionally enter what string to log for the questions in this list. If nothing is filled in, the gameobject's name will be used. Note that the questions are logged in the order of the UI items as defined in this component (i.e. all sliders are logged, then inputfields, etc.).")]
    [SerializeField] string[] questionStrings = null;

    [HideInInspector] public int N;
    List<object> list = new List<object>();

    Dictionary<object, Func<object, string>> methodDict = new Dictionary<object, Func<object, string>>();

    void Start()
    {
        Initialize();

        foreach (var i in sliders)
        {
            list.Add(i);
            methodDict.Add(i, getSlider);
        }
        foreach (var i in inputFields)
        {
            list.Add(i);
            methodDict.Add(i, getInputField);
        }
        foreach (var i in toggles)
        {
            list.Add(i);
            methodDict.Add(i, getToggle);
        }
        foreach (var i in toggleGroups)
        {
            list.Add(i);
            methodDict.Add(i, getToggleGroup);
        }
        foreach (var i in dropdowns)
        {
            list.Add(i);
            methodDict.Add(i, getDropdown);
        }
        N = list.Count;
    }

    public string[] getData()
    {
        int i = it;
        object currObject = list[i];

        string questionString = "";
        if (i < questionStrings.Length)
            if (questionStrings[i].Length > 0)
                questionString = questionStrings[i];

        if (questionString.Length == 0)
            questionString = currObject.ToString();

        string[] strings = new string[2]
        {
            questionString,
            methodDict[currObject](currObject)
        };
            
            

        return strings;
    }

    string getSlider(object o)
    {
        Slider t = (Slider)o;
        return t.value.ToString();
    }

    string getToggle(object o)
    {
        Toggle t = (Toggle)o;
        return t.isOn.ToString();
    }

    string getInputField(object o)
    {
        InputField t = (InputField)o;
        return t.text;
    }

    string getToggleGroup(object o)
    {
        ToggleGroup t = (ToggleGroup)o;
        toggles = t.GetComponentsInChildren<Toggle>();

        int activeToggle = -1;
        for (int i = 0; i < toggles.Length; i++)
            if (toggles[i].isOn)
                activeToggle = i;

        return activeToggle.ToString();
    }

    string getDropdown(object o)
    {
        Dropdown t = (Dropdown)o;
        return t.options[t.value].text;
    }
}
