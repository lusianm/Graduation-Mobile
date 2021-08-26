using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public enum FunctionTypes{
    PartialViedoSeeThrough = 1, Mirroring = 2, VRController = 3, LipMotion = 4
}


public class TCP_Client : MonoBehaviour
{
    public InputField IPInput, PortInput;
    public string serverIP;
    public int serverPort;
    
    public bool socketReady;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;

    private byte[] functionTypeBytes;
    
    public Text debugText;

    private static TCP_Client instance;

    public static TCP_Client GetInstance() => instance;
    
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
    
    /*
    [SerializeField] private ClickEventDetector eventDetector;
    */
    
    public void ConnectToServer()
    {
        // 이미 연결되었다면 함수 무시
        if (socketReady) return;

        // 기본 호스트 / 포트번호
        string ip = IPInput.text == "" ? serverIP : IPInput.text;
        int port = PortInput.text == "" ? serverPort : int.Parse(PortInput.text);

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
        
    }
    
    
    public void Send(FunctionTypes functionType, byte[] bytes = null)
    {
        if (!socketReady) return;
        if (!stream.CanWrite) return;
        
        functionTypeBytes = BitConverter.GetBytes((int)functionType);
        Array.Reverse(functionTypeBytes);
        
        switch (functionType)
        {
            case FunctionTypes.PartialViedoSeeThrough:
                DataStructs.partialTrackingData.bytesLen = bytes.Length;
                byte[] dataBytes = DataStructs.StructToBytes(DataStructs.partialTrackingData);
                
                stream.WriteAsync(functionTypeBytes, 0, functionTypeBytes.Length);
                stream.WriteAsync(dataBytes, 0, dataBytes.Length);
                stream.WriteAsync(bytes, 0, bytes.Length);
                
                stream.FlushAsync();
                break;
            case FunctionTypes.Mirroring:


                break;
            
            
            case FunctionTypes.VRController:


                break;
            
            
            case FunctionTypes.LipMotion:
                byte[] byteLengths = BitConverter.GetBytes((int)bytes.Length);
                stream.WriteAsync(byteLengths, 0, byteLengths.Length);
                stream.WriteAsync(bytes, 0, bytes.Length);
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
