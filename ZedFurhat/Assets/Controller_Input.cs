using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Threading;

public class Controller_Input : MonoBehaviour
{

    public static DatabaseReference reference;
    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    // Update is called once per frame
    void Update()
    {
        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

        bool A_pressed;
        if (rightHandDevices.Count == 1)
        {
            UnityEngine.XR.InputDevice device = rightHandDevices[0];
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out A_pressed) && A_pressed)
            {
                Debug.Log("Trigger button is pressed.");
                saveTrigger();
                Thread.Sleep(500);
            }
        }
    }
    public static void saveTrigger()
    {
        reference.Child("Trigger").Child("Pressed").SetValueAsync("true");
    }
}
