using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public enum FunctionTypes{
    PartialViedoSeeThrough = 1, Mirroring = 2, VRController = 3, VRKeyboard = 4
}


public class TCP_Client : MonoBehaviour
{
    [SerializeField] private ModeChanger modeChanger;
    public InputField IPInput, PortInput;
    public string serverIP;
    public int serverPort;
    
    private bool socketReady;
    TcpClient socket;
    private NetworkStream stream = null;
    private StreamWriter writer;
    private StreamReader reader;

    private byte[] functionTypeBytes;
    
    private static TCP_Client instance;

    public static TCP_Client GetInstance() => instance;

    public bool isConnected() => socketReady;
    
    private void OnEnable()
    {
        if (TCP_Client.instance == null)
        {
            TCP_Client.instance = this;
        }
        else if (TCP_Client.instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        Time.fixedDeltaTime = 1f / 60f;
    }

    private void Update()
    {
        if (stream == null)
            return;
        
        if (stream.DataAvailable)
        {
            byte[] functionTypeBytes = BitConverter.GetBytes((int) 0);
            stream.Read(functionTypeBytes, 0, functionTypeBytes.Length);
            int currentFunctionType = BitConverter.ToInt32(functionTypeBytes, 0);
            modeChanger.SceneChagne(currentFunctionType);
        }
        
    }

    public void ConnectToServer()
    {
        // 이미 연결되었다면 함수 무시
        if (socketReady) return;

        string ip;
        int port;
        // 기본 호스트 / 포트번호
        if (IPInput != null)
        {
            ip = IPInput.text == "" ? serverIP : IPInput.text;
        }
        else
        {
            ip = serverIP;
        }

        if (PortInput != null)
        {
            port = PortInput.text == "" ? serverPort : int.Parse(PortInput.text);
        }
        else
        {
            port = serverPort;
        }

        // 소켓 생성
        try
        {
            socket = new TcpClient(ip, port);
            socket.SendBufferSize = 12000000;
            stream = socket.GetStream();
            socketReady = true;
			
        }
        catch (Exception e)
        {
            Debug.Log("Socket Error : " + e.Message);
        }
        Debug.Log("Connected to Server");
        modeChanger.SceneChagne(SceneType.partialVideoSeeThrough);
    }
    
    
    public void Send(FunctionTypes functionType, byte[] bytes = null)
    {
        if (!socketReady) return;
        if (!stream.CanWrite) return;
        
        functionTypeBytes = BitConverter.GetBytes((int)functionType);
        Array.Reverse(functionTypeBytes);
        
        byte[] dataBytes;
        
        switch (functionType)
        {
            case FunctionTypes.PartialViedoSeeThrough:
                if (bytes == null)
                    return;
                DataStructs.partialTrackingData.bytesLen = bytes.Length;
                Debug.Log("Sending Byte Length : " + bytes.Length);
                dataBytes = DataStructs.StructToBytes(DataStructs.partialTrackingData);
                
                stream.WriteAsync(functionTypeBytes, 0, functionTypeBytes.Length);
                stream.WriteAsync(dataBytes, 0, dataBytes.Length);
                stream.WriteAsync(bytes, 0, bytes.Length);
                stream.FlushAsync();
                break;
            case FunctionTypes.Mirroring:


                break;
            
            
            case FunctionTypes.VRController:
                dataBytes = DataStructs.StructToBytes(DataStructs.vrControllersData);
                stream.WriteAsync(functionTypeBytes, 0, functionTypeBytes.Length);
                stream.WriteAsync(dataBytes, 0, dataBytes.Length);
                stream.FlushAsync();
                break;
            
            
            case FunctionTypes.VRKeyboard:
                byte[] byteLengths = BitConverter.GetBytes((int)bytes.Length);
                Array.Reverse(byteLengths);
                stream.WriteAsync(byteLengths, 0, byteLengths.Length);
                stream.WriteAsync(bytes, 0, bytes.Length);
                break;
        }
		
    }
    
    public void Send(FunctionTypes functionType, DataStructs.VRControllerStruct controllerData)
    {
        if (!socketReady) return;
        if (!stream.CanWrite) return;
        
        functionTypeBytes = BitConverter.GetBytes((int)functionType);
        Array.Reverse(functionTypeBytes);
        
        byte[] dataBytes;
        
        switch (functionType)
        {
            case FunctionTypes.PartialViedoSeeThrough:
                break;
            case FunctionTypes.Mirroring:
                break;
            case FunctionTypes.VRController:
                dataBytes = DataStructs.StructToBytes(controllerData);
                stream.WriteAsync(functionTypeBytes, 0, functionTypeBytes.Length);
                stream.WriteAsync(dataBytes, 0, dataBytes.Length);
                stream.FlushAsync();
                break;
            case FunctionTypes.VRKeyboard:
                break;
        }
		
    }
    
    
    public void Send(FunctionTypes functionType, DataStructs.VRKeyboardStruct keyboardData)
    {
        if (!socketReady) return;
        if (!stream.CanWrite) return;
        
        functionTypeBytes = BitConverter.GetBytes((int)functionType);
        Array.Reverse(functionTypeBytes);
        
        byte[] dataBytes;
        
        switch (functionType)
        {
            case FunctionTypes.PartialViedoSeeThrough:
                break;
            case FunctionTypes.Mirroring:
                break;
            case FunctionTypes.VRController:
                break;
            case FunctionTypes.VRKeyboard:
                dataBytes = DataStructs.StructToBytes(keyboardData);
                stream.WriteAsync(functionTypeBytes, 0, functionTypeBytes.Length);
                stream.WriteAsync(dataBytes, 0, dataBytes.Length);
                stream.FlushAsync();
                break;
        }
		
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}
