using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DataStructs
{
    public struct partialTrackingStruct
    {
        public int bytesLen;
        public bool isTrack;
        public bool isRight;
        public Vector2 cameraResolution;
        public Vector2 trackedPosition;
        public Quaternion trackedRotation;
    }

    public static partialTrackingStruct partialTrackingData = default;

    public struct mirroringStruct
    {
        public int bytesLen;
        public int screenWidth;
        public int screenHeight;
    }

    public static mirroringStruct mirroringData = default;

    public struct VRControllerStruct
    {
        public ControllerType controllerType;
        public bool isButton1Pressed;
        public bool isButton2Pressed;
        public bool isJoysticPressed;
        public Vector2 joysticAxis;
        public DeviceOrientation deviceOrientation;
    }

    public static VRControllerStruct vrControllerData = default;
    public static byte[] StructToBytes(object obj)
    {
        //구조체 사이즈
        int iSize = Marshal.SizeOf(obj);
        //사이즈 만큼 메모리 할당 받기
        byte[] arr = new byte[iSize];
        IntPtr ptr = Marshal.AllocHGlobal(iSize);
        //구조체 주소값 가져오기
        Marshal.StructureToPtr(obj, ptr, false);
        //메모리 복사
        Marshal.Copy(ptr, arr, 0, iSize);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    public static T ByteToStruct<T>(byte[] buffer) where T : struct
    {
        //구조체 사이즈
        int size = Marshal.SizeOf(typeof(T));
        if (size > buffer.Length)
        {
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);
        T obj = (T) Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }
}
