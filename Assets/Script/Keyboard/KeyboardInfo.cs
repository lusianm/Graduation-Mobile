using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    [SerializeField] private KeyboardManager keyboardManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        DataStructs.VRKeyboardStruct tempData;
        tempData.touchID = eventData.pointerId;
        tempData.isPressed = true;
        tempData.touchPosition = eventData.position;
        tempData.screenResolution = default;
        keyboardManager.UpdateTouchInfo(tempData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DataStructs.VRKeyboardStruct tempData;
        tempData.touchID = eventData.pointerId;
        tempData.isPressed = true;
        tempData.touchPosition = eventData.position;
        //Debug.Log("ID : " + tempData.touchID + "\nPosition x: " + tempData.touchPosition.x.ToString() + "    Position y: " + tempData.touchPosition.y.ToString());
        tempData.screenResolution = default;
        keyboardManager.UpdateTouchInfo(tempData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DataStructs.VRKeyboardStruct tempData;
        tempData.touchID = eventData.pointerId;
        tempData.isPressed = false;
        tempData.touchPosition = eventData.position;
        tempData.screenResolution = default;
        keyboardManager.UpdateTouchInfo(tempData);
    }
}
