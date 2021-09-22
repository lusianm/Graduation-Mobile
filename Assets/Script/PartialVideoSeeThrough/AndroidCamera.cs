using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidCamera : MonoBehaviour
{
    //Variables for Setting
    public int requestFPS = 24;
    public bool isUseFrontCamera;
    public int requestWidth, requestHeight;
    public InputField requestWidthInput, requestHeightInput;
    
    
    //Variables for external reference
    [SerializeField] private RawImage background;
    [SerializeField] private Text debugText;
    [SerializeField] private FunctionTypes functionType;
    
    TCP_Client client;
    
    //Variables for data save
    private bool camAvailable = false;
    private WebCamTexture cameraTexture;
    private Texture2D camTexture2D = null;
    private byte[] bytesToRawImage = null;


    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    void Start()
    {
        client = TCP_Client.GetInstance();
        SetCamera();
        StartCamera();
    }

    void SetCamera()
    {
        cameraTexture = null;
        
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            debugText.text = "There is no available Web Cam";
            Debug.LogError("There is no available Web Cam");
            return;
        }
        
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == isUseFrontCamera)
            {
                cameraTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                
                debugText.text = devices[i].availableResolutions.ToString();
                foreach (var resolution in devices[i].availableResolutions)
                {
                    Debug.Log("Resolution : " + resolution.ToString());
                }
                break;
            }
        }
        
        if (cameraTexture == null)
        {
            debugText.text = "There is no available Web Cam";
            Debug.LogError("There is no available Web Cam");
            return;
        }
    }

    public void StartCamera()
    {
        if (cameraTexture == null)
        {
            debugText.text = "There is no available Web Cam";
            Debug.LogError("There is no available Web Cam");
            return;
        }

        if (requestWidthInput != null)
        {
            cameraTexture.requestedWidth = (requestWidthInput.text == "") ? requestWidth : int.Parse(requestWidthInput.text);
        }
        else
        {
            cameraTexture.requestedWidth = requestWidth;
        }

        if (requestHeightInput != null)
        {
            cameraTexture.requestedHeight = (requestHeightInput.text == "") ? requestHeight : int.Parse(requestHeightInput.text);
        }
        else
        {
            cameraTexture.requestedHeight = requestHeight;
        }
        
        cameraTexture.requestedFPS = requestFPS;
        
        cameraTexture.Play();
        background.texture = cameraTexture;
        float scaleRatio = (float) background.canvas.GetComponent<CanvasScaler>().referenceResolution.x / (float) cameraTexture.height;
        background.rectTransform.sizeDelta =
            new Vector2(cameraTexture.width * scaleRatio, cameraTexture.height * scaleRatio);
        int orient = -cameraTexture.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        DataStructs.partialTrackingData.cameraResolution = new Vector2(cameraTexture.width, cameraTexture.height);

        camAvailable = true;
        camTexture2D = new Texture2D(cameraTexture.width, cameraTexture.height, TextureFormat.RGBA32, false);
        
    }

    void Update()
    {
        if (!camAvailable)
            return;
        if (!cameraTexture.didUpdateThisFrame)
            return;
        
        camTexture2D.SetPixels(cameraTexture.GetPixels());
        camTexture2D.Apply();
        
        debugText.text = "camera height : " + cameraTexture.height.ToString();
        debugText.text += "\ncamera width : " + cameraTexture.width.ToString();
        debugText.text += "\nscreen width : " + Screen.width.ToString();
        debugText.text += "\nscreen height : " + Screen.height.ToString();
        debugText.text += "\ntexture format : " + camTexture2D.format.ToString();
        
        if (client.isConnected())
        {
            bytesToRawImage = camTexture2D.EncodeToJPG();
            debugText.text += "\nbyte Length : " + bytesToRawImage.Length.ToString();
            debugText.text += "\nfunction Type Length : " + functionType.ToString();
            client.Send(functionType, bytesToRawImage);
        }

    }
}
