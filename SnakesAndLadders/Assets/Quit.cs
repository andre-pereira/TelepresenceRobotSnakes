using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class Quit : MonoBehaviour
{   
    public static DatabaseReference reference;
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool Beep(uint dwFreq, uint dwDuration);


    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        
    }

    public static void saveGameState()
    {
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();

        childUpdates["/GameState/UniversalTime"] = DateTime.Now.ToString("HH:mm:ss:fff");
        childUpdates["/GameState/EntireGameTime"] = Time.time.ToString("f5");
        childUpdates["/GameState/TurnTime"] = Time.time - GameControl.turnTime;

        reference.UpdateChildrenAsync(childUpdates);
    }

    public void ExitGame()
    {   
        saveGameState();
        Beep(4000, 500);
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
        Debug.Log("Game is exiting");
    }
}
