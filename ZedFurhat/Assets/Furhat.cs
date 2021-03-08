using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TCPFurhatComm;
using UnityEngine;
using UnityEngine.XR;
using static OVRLipSync;
using Firebase;
using Firebase.Database;

public class Furhat : MonoBehaviour
{
    private SkinnedMeshRenderer mesh;
    public OVRLipSyncContext lipSyncContext;

    FurhatInterface furhat = null;

    public string FurhatIPAddress;

    public Transform ballPosition;
    public Transform zedCamera;

    public double[] lipSyncAnimationPoints = new double[15];
    public Vector3 position;
    public float roll;
    public int numberOfCommandsSent;

    public float GazeRollThreshold = 0;
    public float GazeEuclideanThreshold = 0;
    public double lipSynchParamThreshold = 0;

    public static string strFilePath;
    public static DatabaseReference reference;
    public static int Entry;
    public static string strSeperator;
    public static float time;
    public static float Gaze_x;
    public static float Gaze_y;
    public static float Gaze_z;
    public static float Gaze_roll;
    public static int Phone;
    public static float Intensity;


    // Start is called before the first frame update
    void Start()
    {
        furhat = new FurhatInterface(FurhatIPAddress, nameForSkill: "CSharp Example");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        strSeperator = ",";
        Entry = 0;
        Gaze_x = 0;
        Gaze_y = 0;
        Gaze_z = 0;
        Gaze_roll = 0;
        Phone = 0;
        Intensity = 0;

        strFilePath = @"C:\Users\ilian\Unity_Projects\saved_data\GazePhoneLogs_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".csv";
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("GameState/StartTime")
        .ValueChanged += HandleValueChanged;
    }

    // Update is called once per frame
    void Update()
    {
        SetGazeRoll(-zedCamera.rotation.z * 100);
        Gaze(-ballPosition.position.x, ballPosition.position.y, ballPosition.position.z);
        Frame frame = lipSyncContext.GetCurrentPhonemeFrame();
        processLipSyncFrame(frame);
    }


    // Called when field in Firebase is altered
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Entry++;

        // Create and write the csv file
        if (Entry == 2)
        {
            Debug.Log("Generating Headings");
            time = Time.time;
            File.WriteAllText(strFilePath, string.Join(strSeperator,
                                                        "StartTime",
                                                        args.Snapshot.Value + "\n"));
            File.AppendAllText(strFilePath, string.Join(strSeperator,
                                                        "Time",
                                                        "Gaze_x",
                                                        "Gaze_y",
                                                        "Gaze_z",
                                                        "Gaze_roll",
                                                        "Phone",
                                                        "Intensity" + "\n"));
            Entry++;
        }



    }


    public static void saveGazePhone()
    {
        print(Entry);
        if (Entry > 2)
        {
            File.AppendAllText(strFilePath, string.Join(strSeperator,
                                                    (Time.time - time).ToString(),
                                                    Gaze_x.ToString(),
                                                    Gaze_y.ToString(),
                                                    Gaze_z.ToString(),
                                                    Gaze_roll.ToString(),
                                                    Phone.ToString(),
                                                    Intensity.ToString() + "\n"));
        }
    }


    private void Gaze(float x, float y, float z)
    {
        if (Vector3.Distance(position, new Vector3(x, y, z)) > GazeEuclideanThreshold)
        {
            numberOfCommandsSent++;
            furhat.Gaze(x, y, z);
            position = new Vector3(x, y, z);
            Gaze_x = x;
            Gaze_y = y;
            Gaze_z = z;

            saveGazePhone();
        }
    }

    private void SetGazeRoll(float v)
    {
        if (Math.Abs(roll - v) > GazeRollThreshold)
        {
            numberOfCommandsSent++;
            furhat.SetGazeRoll(v);
            roll = v;
            Gaze_roll = roll;
            saveGazePhone();
        }
    }

    private void processLipSyncFrame(Frame frame)
    {
        for (int i = 0; i < frame.Visemes.Length; i++)
        {
            UpdateParameter(i, frame.Visemes[i]);
        }
    }

    public void UpdateParameter(int paramNumber, float intensity)
    {
        if (Math.Abs(lipSyncAnimationPoints[paramNumber] - intensity) > lipSynchParamThreshold)
        {
            lipSyncAnimationPoints[paramNumber] = intensity;

            if (paramNumber == 1) 
                ChangeParameter(PARAMS.PHONE_B_M_P, 1, intensity);
            else if (paramNumber == 2) 
                ChangeParameter(PARAMS.PHONE_F_V, 1, intensity);    
            else if (paramNumber == 3) 
                ChangeParameter(PARAMS.PHONE_TH, 1, intensity);
            else if (paramNumber == 4) 
                ChangeParameter(PARAMS.PHONE_D_S_T, 1, intensity);
            else if (paramNumber == 5) 
                ChangeParameter(PARAMS.PHONE_K, 1, intensity);
            else if (paramNumber == 6) 
                ChangeParameter(PARAMS.PHONE_CH_J_SH, 1, intensity);
            else if (paramNumber == 7) 
                ChangeParameter(PARAMS.PHONE_D_S_T, 1, intensity);
            else if (paramNumber == 8) ChangeParameter(PARAMS.PHONE_N, 1, intensity);
            else if (paramNumber == 9) ChangeParameter(PARAMS.PHONE_R, 1, intensity);
            else if (paramNumber == 10) ChangeParameter(PARAMS.PHONE_AAH, 1, intensity);
            else if (paramNumber == 11) ChangeParameter(PARAMS.PHONE_EE, 1, intensity);
            else if (paramNumber == 12) ChangeParameter(PARAMS.PHONE_I, 1, intensity);
            else if (paramNumber == 14) ChangeParameter(PARAMS.PHONE_OH, 1, intensity);
            else if (paramNumber == 15) ChangeParameter(PARAMS.PHONE_OOH_Q, 1, intensity);
            Phone = paramNumber;
            Intensity = intensity;
            saveGazePhone();
        }
    }

    private void ChangeParameter(PARAMS phoneme, int number, float intensity)
    {
        numberOfCommandsSent++;
        //Debug.Log("Changing " + phoneme + " with number " + number + "and intensity " + intensity);
        furhat.ChangeParameter(phoneme, 0.05f, intensity);
    }

    private void OnApplicationQuit()
    {
        furhat?.CloseConnection();
    }
}
