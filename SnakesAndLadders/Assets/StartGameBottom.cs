using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using Firebase;
using Firebase.Database;

public class StartGameBottom : MonoBehaviour
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool Beep(uint dwFreq, uint dwDuration);
    public static DatabaseReference reference;
    public static float startTime;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public static void saveGameState()
    {
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/GameState/StartTime"] = DateTime.Now.ToString("HH:mm:ss:fff");
        childUpdates["/GameState/Date"] = "Starting";
        childUpdates["/GameState/UniversalTime"] = "Starting";
        childUpdates["/GameState/EntireGameTime"] = "Starting";
        childUpdates["/GameState/TurnTime"] = "Starting";
        childUpdates["/GameState/WhosTurn"] = "Starting";
        childUpdates["/GameState/DiceValue"] = "Starting";
        childUpdates["/GameState/Human"] = "Starting";
        childUpdates["/GameState/Furhat"] = "Starting";
        childUpdates["/GameState/ifSnake"] = "Starting";
        childUpdates["/GameState/ifLadder"] = "Starting";
        reference.UpdateChildrenAsync(childUpdates);
    }

    public void DoBeep()
    {
        startTime = Time.time;
        Beep(4000, 500);
        saveGameState();
        
        //EditorApplication.Beep();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
