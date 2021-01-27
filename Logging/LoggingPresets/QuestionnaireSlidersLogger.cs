using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class QuestionnaireSlidersLogger : Logger
{
    // Log the positions of a bunch of sliders attached to a canvas. A more simple version of QuestionnaireLogger which only takes into acount sliders.
    [HideInInspector]
    public string[] reportHeaders = new string[2]
    {
        "Question",
        "Value"
    };

    [Header("Logger Specific Settings")] // Create a header for any logger specific settings
    [SerializeField] public string fileNamePrefix = "Q_";
    [SerializeField] bool resetSlidersAfterLogging = true;
    [SerializeField] int resetValue = 0;

    Canvas canvas;
    Slider[] sliders;

    [HideInInspector] public int N;

    void Start()
    {
        Initialize();
        canvas = GetComponent<Canvas>();
        sliders = canvas.GetComponentsInChildren<Slider>();
        N = sliders.Length;
    }

    public string[] getData()
    {
        var s = sliders[it]; // int it is defined in the logger class, and increases untill N is reached and gets reset to 0

        string[] strings = new string[2]
        { 
                s.transform.parent.gameObject.name, // Question number
                s.value.ToString() // Answer
        };

        if (resetSlidersAfterLogging)
            s.value = resetValue;

        return strings;
    }
}
