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
    private static GameObject Recording;
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool Beep(uint dwFreq, uint dwDuration);
    public static DatabaseReference reference;


    void Start()
    {
        Recording = GameObject.Find("RecordingText");
        Recording.gameObject.SetActive(false);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public static void saveGameState()
    {
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/GameState/StartTime"] = DateTime.Now.ToString("HH:mm:ss:fff");

        reference.UpdateChildrenAsync(childUpdates);
    }

    public void DoBeep()
    {
        Recording.gameObject.SetActive(true);
        Beep(4000, 500);
        saveGameState();
        //EditorApplication.Beep();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
