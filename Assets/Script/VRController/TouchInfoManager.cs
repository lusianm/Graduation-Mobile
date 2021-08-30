using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using UnityEngine;

public enum ControllerType
{
    RightController = 1, LeftController = 2
}

public class TouchInfoManager : MonoBehaviour
{
    [SerializeField] private FunctionTypes functionType = FunctionTypes.VRController;
    private static DataStructs.VRControllerStruct rightHand, leftHand;
    private static bool rightHandUpdated, leftHandUpdated;
    private TCP_Client client;
    
    void Start()
    {
        client = TCP_Client.GetInstance();
        rightHandUpdated = false;
        leftHandUpdated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(client != null)
            client.Send(functionType);
    }

    public static void SetControllerData(DataStructs.VRControllerStruct controllerData)
    {
        switch(controllerData.controllerType)
        {
            case ControllerType.RightController:
                DataStructs.vrControllersData.rightController = controllerData;
                break;
            case ControllerType.LeftController:
                DataStructs.vrControllersData.leftController = controllerData;
                break;
        }
    }
    
}
