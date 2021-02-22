using System;
using System.Collections;
using System.Collections.Generic;
using TCPFurhatComm;
using UnityEngine;
using UnityEngine.XR;
using static OVRLipSync;

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

    // Start is called before the first frame update
    void Start()
    {
        furhat = new FurhatInterface(FurhatIPAddress, nameForSkill: "CSharp Example");
    }

    // Update is called once per frame
    void Update()
    {
        SetGazeRoll(-zedCamera.rotation.z * 100);
        Gaze(-ballPosition.position.x, ballPosition.position.y, ballPosition.position.z);
        Frame frame = lipSyncContext.GetCurrentPhonemeFrame();
        processLipSyncFrame(frame);
    }

    private void Gaze(float x, float y, float z)
    {
        if (Vector3.Distance(position, new Vector3(x, y, z)) > GazeEuclideanThreshold)
        {
            numberOfCommandsSent++;
            furhat.Gaze(x, y, z);
            position = new Vector3(x, y, z);
        }
    }

    private void SetGazeRoll(float v)
    {
        if (Math.Abs(roll - v) > GazeRollThreshold)
        {
            numberOfCommandsSent++;
            furhat.SetGazeRoll(v);
            roll = v;
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
        }
    }

    private void ChangeParameter(PARAMS phoneme, int number, float intensity)
    {
        numberOfCommandsSent++;
        Debug.Log("Changing " + phoneme + " with number " + number + "and intensity " + intensity);
        furhat.ChangeParameter(phoneme, 0.05f, intensity);
    }

    private void OnApplicationQuit()
    {
        furhat?.CloseConnection();
    }
}
