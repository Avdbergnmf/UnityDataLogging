using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrialOrderManager))]
public class PlayerInfoLogger : Logger
{
    // Log the playerinfo for later reference.

    [Header("Logger Specific Settings")] // Create a header for any logger specific settings
    [SerializeField] public string fileNamePrefix = "PlayerInfo_";
    TrialOrderManager trialOrderManager;

    [HideInInspector] public string[] reportHeaders = new string[4] {
            "Name",
            "Age",
            "Gender",
            "Condition Order"
    };


    private void Start()
    {
        Initialize();

        trialOrderManager = GetComponent<TrialOrderManager>();
    }


    public string[] getData()
    {
        var playerInfo = trialOrderManager.playerInfo;

        string[] strings = new string[4] { // Put the playerinfo in a list of strings
            playerInfo.name,
            playerInfo.age.ToString(),
            playerInfo.gender.ToString(),
            trialOrderManager.conditionOrder.ToString()
        };
        return strings;
    }
}
