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
    [SerializeField] private GameObject mode1Panel, mode2Panel;
    private static DataStructs.VRControllerStruct rightHand, leftHand;
    private static bool rightHandUpdated, leftHandUpdated;
    private TCP_Client client;
    private int curretnMode;
    private DeviceOrientation currentDeviceOrientation;
    
    void Start()
    {
        client = TCP_Client.GetInstance();
        Screen.orientation = ScreenOrientation.Landscape;
        
        /*
        if ((Screen.orientation == ScreenOrientation.Portrait) ||
            (Screen.orientation == ScreenOrientation.PortraitUpsideDown))
        {
            Debug.Log("Portrait Screen");
            Screen.SetResolution(Screen.height, Screen.width, true);
            
        }
        else
        {
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
        */
        
        rightHandUpdated = false;
        leftHandUpdated = false;
        InitControllerDate();
        mode1Panel.SetActive(true);
        mode2Panel.SetActive(false);
        curretnMode = 1;
        currentDeviceOrientation = DeviceOrientation.LandscapeLeft;
    }

    // Update is called once per frame
    void Update()
    {
        currentDeviceOrientation = (Input.deviceOrientation == DeviceOrientation.FaceDown ||
                                               Input.deviceOrientation == DeviceOrientation.FaceUp ||
                                               Input.deviceOrientation == DeviceOrientation.Unknown)
            ? currentDeviceOrientation : Input.deviceOrientation;

        switch (curretnMode)
        {
            case 1:
                if (currentDeviceOrientation == DeviceOrientation.Portrait)
                {
                    mode1Panel.SetActive(false);
                    mode2Panel.SetActive(true);
                    InitControllerDate();
                    curretnMode = 2;
                }

                break;
            case 2:
                if (currentDeviceOrientation == DeviceOrientation.LandscapeLeft)
                {
                    mode1Panel.SetActive(true);
                    mode2Panel.SetActive(false);
                    InitControllerDate();
                    curretnMode = 1;
                }

                break;
        }
        
    }

    private void FixedUpdate()
    {
        if(client != null)
            client.Send(functionType);
    }

    private void InitControllerDate()
    {
        DeviceOrientation deviceOrientation = (Input.deviceOrientation == DeviceOrientation.FaceDown ||
                                              Input.deviceOrientation == DeviceOrientation.FaceUp ||
                                              Input.deviceOrientation == DeviceOrientation.Unknown)
            ? DeviceOrientation.LandscapeLeft : Input.deviceOrientation;
        
        //Right Controller
        DataStructs.vrControllersData.rightController.controllerType = ControllerType.RightController;
        DataStructs.vrControllersData.rightController.isButton1Pressed = false;
        DataStructs.vrControllersData.rightController.isButton2Pressed = false;
        DataStructs.vrControllersData.rightController.isJoysticPressed = false;
        DataStructs.vrControllersData.rightController.joysticAxis = Vector2.zero;
        DataStructs.vrControllersData.rightController.deviceOrientation = deviceOrientation;
        
        //Left Controller
        DataStructs.vrControllersData.leftController.controllerType = ControllerType.LeftController;
        DataStructs.vrControllersData.leftController.isButton1Pressed = false;
        DataStructs.vrControllersData.leftController.isButton2Pressed = false;
        DataStructs.vrControllersData.leftController.isJoysticPressed = false;
        DataStructs.vrControllersData.leftController.joysticAxis = Vector2.zero;
        DataStructs.vrControllersData.leftController.deviceOrientation = deviceOrientation;
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
