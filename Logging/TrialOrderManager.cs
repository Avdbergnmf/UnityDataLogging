using UnityEngine;
using UnityEditor.Presets;
using UnityEditor;

// This script is here to manage the an array containing the order in which the presets will be given to the participant
// In this way we can keep track of which trial we are currenly at and which one we need to proceed to
// Whats more, this information can be saved, so that if a crash were to happen, we can pick up where we left off!
public class TrialOrderManager : MonoBehaviour
{
    #region Properties
    [Header("Main Settings")]
    [Tooltip("Do you want to enable practice runs?")] [SerializeField] public bool doPractice = false;
    [Tooltip("The number of trials per condition")] [SerializeField] public int numberOfRepetitions = 3;
    [Tooltip("How many secs should each trial take?")] [SerializeField] public float secPerTrial = 300; // These times are not used currently, but rather their intention is that you use other scripts to get these timers from this class to use them externally.
    [Tooltip("How many secs should the participant take a break for each trial?")] [SerializeField] public float breakTimePerTrial = 60;

    [Header("Current trial order state")]
    public int[] conditionOrder = new int[1]; // an array containing the order in which the conditions are presented to the operator
    public int currConditionNo = 0; // which condition no are we at?
    public int currRep = 0;
    
    public bool practiceState = true;

    [Header("The Saved PlayerInfo")]
    // Lets also save the player info here, you might want to get this using some UI Form
    public PlayerInfo playerInfo = null;

    // private
    LoggingManager LM;

    #endregion

    #region MainMethods

    void Start() // This is not very optimized, but convenient cause no other scripts will depend on this one and it will require minimum setup to use!
    {
        // Do we want to include practice trials?
        practiceState = doPractice; // if so, start with a practice round

        // find the logging manager (to get the folder where to save the presets)
        LM = FindObjectOfType<LoggingManager>();
        if (!LM) Debug.LogError("No LoggingManager found! Can't get the folder in which to store the preset");
    }


    #endregion

    #region trialManagement
    public void nextTrial()
    {
        if (practiceState) // is the current run a testrun?
            practiceState = false; // make it a good run, keep the rest the same.
        else
        {
            if (++currRep < numberOfRepetitions) // Do we need to do more repetitions?
            { // Progress to next repetition
                Debug.Log("Next Trial: " + currRep.ToString());
            }
            else
            {
                if (currConditionNo + 1 <= conditionOrder.Length)
                {
                    currRep = 0; // Reset to 0
                    currConditionNo++; // Progress to the next condition

                    if (doPractice)
                        practiceState = true;
                }
                else
                {
                    Debug.Log("EXPERIMENT IS FINISHED!");
                    currConditionNo++;
                    currRep = 0; // So that scenestate can progress to the questionnaire
                }
            }
        }
    }

    public int getCurrPresetNo()
    {
        int presetNo = conditionOrder[currConditionNo];
        return presetNo;
    }


    #endregion

    #region PresetSaving
    public void SavePreset() // This functions saves a preset of this component to the reportDirectoryPath so that in the event of a crash, this preset can be reloaded and the experiment can be continued.
    { // In order to use this make sure to run this function at the start of every new trial.
        if (LM)
        {
            LM.loggers[0].VerifyDirectory(); // Make sure the directory exists, else create it
            CreatePresetAsset(this, "trialOrderManager", LM.reportDirectoryPath.Replace("/practice", "") + "/.."); // be sure that its not placed in the practicefolder, if we're logging runs!
        }
    }

    // This method creates a Preset from a given Object and save it as an asset in the project.
    public void CreatePresetAsset(Object source, string name, string folder)
    {
        Preset preset = new Preset(source);
        AssetDatabase.CreateAsset(preset, folder + '/' + name + ".preset");
    }
    #endregion
}
