using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    [SerializeField] private FunctionTypes functionType = FunctionTypes.VRKeyboard;
    private TCP_Client client;
    private List<DataStructs.VRKeyboardStruct> vrKeyboardLists;
    private Vector2 screenResolution;


    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        
        client = TCP_Client.GetInstance();
        vrKeyboardLists = new List<DataStructs.VRKeyboardStruct>();
        screenResolution = new Vector2(Screen.currentResolution.height, Screen.currentResolution.width);
        //Debug.Log("Screen Size : " + Screen.currentResolution.ToString());

    }

    // Update is called once per frame
    void Update()
    {
        SendTouchInfo();
        RemovePointerUpTouchInfo();
    }

    private void SendTouchInfo()
    {
        if (vrKeyboardLists.Count == 0)
            return;
        
        foreach (DataStructs.VRKeyboardStruct keyboardInfo in vrKeyboardLists)
        {
            client.Send(functionType, keyboardInfo);
        }
    }

    private void RemovePointerUpTouchInfo()
    {
        if (vrKeyboardLists.Count == 0)
            return;
        
        List<DataStructs.VRKeyboardStruct> removeList = new List<DataStructs.VRKeyboardStruct>();
        
        //더이상 터치를 진행하지 않는 data를 찾아서 remove list에 추가
        foreach (DataStructs.VRKeyboardStruct keyboardInfo in vrKeyboardLists)
        {
            if (!keyboardInfo.isPressed)
                removeList.Add(keyboardInfo);
        }

        //remove list를 참고하여 touch가 끝난 정보 제거
        foreach (DataStructs.VRKeyboardStruct removeInfo in removeList)
        {
            vrKeyboardLists.Remove(removeInfo);
        }
    }

    public void UpdateTouchInfo(DataStructs.VRKeyboardStruct updateKeyboardInfo)
    {
        updateKeyboardInfo.screenResolution = this.screenResolution;
        //기존에 data가 존재하면 data를 업데이트
        if (vrKeyboardLists.Exists(info => info.touchID == updateKeyboardInfo.touchID))
        {
            for (int i = 0; i < vrKeyboardLists.Count; i++)
            {
                if (vrKeyboardLists[i].touchID == updateKeyboardInfo.touchID)
                {
                    vrKeyboardLists[i] = updateKeyboardInfo;
                }
            }
        }
        else
        {
            vrKeyboardLists.Add(updateKeyboardInfo);
        }
    }


    private void StructTestCode()
    {
        
        DataStructs.VRKeyboardStruct tempStruct = new DataStructs.VRKeyboardStruct();
        tempStruct.touchID = 1;
        tempStruct.isPressed = true;
        vrKeyboardLists.Add(tempStruct);
        tempStruct.touchID = 2;
        vrKeyboardLists.Add(tempStruct);
        tempStruct.touchID = 3;
        vrKeyboardLists.Add(tempStruct);
        tempStruct.touchID = 2;
        tempStruct.isPressed = false;
        
        
        foreach (DataStructs.VRKeyboardStruct keyboardInfo in vrKeyboardLists)
        {
            Debug.Log("Touch ID : " + keyboardInfo.touchID + "\nisPressed : " + keyboardInfo.isPressed);
        }
        
        UpdateTouchInfo(tempStruct);
        
        
        foreach (DataStructs.VRKeyboardStruct keyboardInfo in vrKeyboardLists)
        {
            Debug.Log("Touch ID : " + keyboardInfo.touchID + "\nisPressed : " + keyboardInfo.isPressed);
        }
        tempStruct.touchID = 4;
        UpdateTouchInfo(tempStruct);
        
        foreach (DataStructs.VRKeyboardStruct keyboardInfo in vrKeyboardLists)
        {
            Debug.Log("Touch ID : " + keyboardInfo.touchID + "\nisPressed : " + keyboardInfo.isPressed);
        }
    }
}
