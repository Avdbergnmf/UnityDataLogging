# UnityDataLogging
A fully functional easily customizable data logger for Unity 2019 and up.

Any experiments requires the collection of data. Unity isn't strictly made for doing experiments (it is a game-engine after all), so there aren't any built-in solutions to achieve this.

## Files
- The Unity DataLogging Package
- Made for Unity 2019.4.18f but should work in any version.

## How it works
The scripts collect the data into CSV files, ordered into folders, based on the name of the participant, and the current experimental condition (all of which is defined in the `TrialOrderManager` class). There is a central manager script called the DataloggingManager that handles all the separate logger scripts. A central class called `Logger` instantiates a class called `LogToCSV` which contains all the logging functions. This `Logger` class is inherited by data loggign templates which are relatively simple and used to define what data is actually logged. In this way, creating a new script to log other types of data should be relatively simple.

These CSV files can later be analyzed using for example Matlab, R or Python.

## How to use it
There is a set of templates you can use to log certain types of data, but if you need to log other things, it is quite easy to create a new logger yourself.

#### Using a template
This example explains how to log the properties of a RigidBody.

- Import the DataLogging.unitypackage into your Unity project. 
  - Drag it into your editor, click import.
- Create a new Sphere, and give it a RigidBody component
- Next, attach the RigidBody Logger script.
- You now have two options: Add it to the LoggerManager (enable the bool in the editor), or log the data directly.
  - Log the data directly using this single logger: Disable the AddToLoggingManager bool. Call the Logger.write() function from an external script, or modify the script to include it in for example the Update() function. Be sure to create the file first by calling the createFile() function, which creates a new file using the name you specified, and in the folder specified in the editor properties of this specific logger. You can also write a single entry using the writeSingleEntry() function, which creates the file for you if it's not done yet. This can be handy if you just need to log a couple of things at maximum.
  - Log the data using the LoggingManager: Enable the AddToLoggingManager bool. Create a new gameobject, call it LoggingManager, and attach the LoggingManager script to it. This will also automatically attach the TrialOrderManager component. Enable the LogAtStartup bool in the editor to make sure all added loggers start logging at startup. If this is not desired, you can instead call the startRepetition() function externally to create all the files, and start all the added loggers. The name of the log file is still determined by the individuel logger scripts, the folder it is saved to is determined by the LoggingManager in combination with the TrialOrderManager.

#### Create a new logger
It is quite simple to create your own custom loggers to log any sort of data you would like to (as long as it can be converted to strings). To do so simply create a new script that inherits from the `Logger` class, make sure it has the following properties:
1. Run the `Initialize()` function in the `Start()`
2. have a `public getData()` function that returns a `string[]` object containing the data to be logged
3. have a `public string[] reportHeaders` variable which contains all the headers used for the logged file (ie explains the type of data that is logged)

Optionally also include:
- A public string `fileNamePrefix` which will indicate which type of data is being logged in the log its filename

A minimal example is shown below:
```C#
using UnityEngine;

public class ObjectNameLogger : Logger
{
    // Log the object name. Not quite useful in itself but useful as a demonstration template

    [HideInInspector] 
    public string[] reportHeaders = new string[1] {
        "GameObject Name",
    };

    [Header("Logger Specific Settings")] // Create a header for any logger specific settings
    [SerializeField] public string fileNamePrefix = "Custom_";
    
    [HideInInspector] public int N; // Optional, for the Logger.writeN() function

    void Start()
    {
        Initialize();
    }

    public string[] getData()
    {
        string[] strings = new string[1] {
                name.ToString(),
        };
        return strings;
    }
}
```

Additionally if you want to log multiple times every log call you may want to use the `writeN()` function. This has not really been integrated into the LoggingManager script but is ment to be called externally. If you use this, be sure to define a `public int N` equal to the number of times that the getData() function needs to be called. An example of this is shown in the QuestionnaireLogger script.


#### Notes On performance
The logs are written to CSVs using `System.IO.StreamWriter`. Using this opens up a file, writes to it and closes it again, to prevent building up local memory usage. However, doing this is quite heavy so you defenitely don't want to do this every frame. To deal with this, you can set the `writeEveryNSeconds` parameter in the LoggingManager. Setting this very low will cause lagg, setting this too high will ramp up the local memory stored and increase the risk of losing data. 

Furthermore, for some loggers it might be quite resource intensive to collect the data. An example is included in the ParticleLogger. Imagine needing to find out how many particles are within a certain trigger collider. Scanning this collider every game update is probably not desired as it will take up a lot of resources. Instead you can make use of the CustomFixedUpdate which updates the data at a set interval. The data is still collected at the same rate, though only updated at a set interval.

## Additional Loggers
The AdditionalLoggers.zip contains some additional loggers that are only useful for specific situations
- OmniLogger: Uses the OpenHaptics plugin to log data for the Omni haptic device
- ObiActorLogger: Scans colliders for Obi particles to determine how many are in / not in those regions.
- FlexArrayLogger: the same as the ObiActorLogger but for NVIDIA Flex