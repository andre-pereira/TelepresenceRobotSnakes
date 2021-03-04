using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using Firebase;
using Firebase.Database;

public class LogGame : MonoBehaviour
{
    public string strFilePath;
    public static DatabaseReference reference;
    public bool firstEntry;
    public string strSeperator;
    public StringBuilder log = new StringBuilder();
    public StringBuilder header = new StringBuilder();
    // Start is called before the first frame update
    void Start()
    {
        strSeperator = ",";
        firstEntry = true;
        strFilePath = @"C:\Users\ilian\Unity_Projects\saved_data\RecordingLogs_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".csv";
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("GameState")
        .ValueChanged += HandleValueChanged;
    }

    // Called when field in Firebase is altered
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }


        // Create and write the csv file
        if (firstEntry)
        {
            Debug.Log("Generating Headings");
            File.WriteAllText(strFilePath, string.Join(strSeperator,
                                                        "Date",
                                                        "UniversalTime",
                                                        "EntireGameTime",
                                                        "TurnTime",
                                                        "WhosTurn",
                                                        "DiceValue",
                                                        "Furhat",
                                                        "ifSnake",
                                                        "ifLadder",
                                                        "Gaze_x",
                                                        "Gaze_y",
                                                        "Gaze_z",
                                                        "Gaze_roll",
                                                        "Phone",
                                                        "PhoneIntensity") + "\n");
            firstEntry = false;
        }
        // To append more lines to the csv file
        else
        {
            Debug.Log("Logging Values");
            Debug.Log(args.Snapshot.Child("ifSnake").Value);
            File.AppendAllText(strFilePath, string.Join(strSeperator,
                                                        args.Snapshot.Child("Date").Value,
                                                        args.Snapshot.Child("UniversalTime").Value,
                                                        args.Snapshot.Child("EntireGameTime").Value,
                                                        args.Snapshot.Child("TurnTime").Value,
                                                        args.Snapshot.Child("WhosTurn").Value,
                                                        args.Snapshot.Child("DiceValue").Value,
                                                        args.Snapshot.Child("Furhat").Value,
                                                        args.Snapshot.Child("ifSnake").Value,
                                                        args.Snapshot.Child("ifLadder").Value,
                                                        args.Snapshot.Child("Gaze_x").Value,
                                                        args.Snapshot.Child("Gaze_y").Value,
                                                        args.Snapshot.Child("Gaze_z").Value,
                                                        args.Snapshot.Child("Gaze_roll").Value,
                                                        args.Snapshot.Child("Phone").Value,
                                                        args.Snapshot.Child("PhoneIntensity").Value) +"\n");
        }
        
        
    }

}
