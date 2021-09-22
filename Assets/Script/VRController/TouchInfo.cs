using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TouchInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // Start is called before the first frame update
    [SerializeField] private FunctionTypes functionType = FunctionTypes.VRController;
    public ControllerType controllerType = ControllerType.RightController;
    public bool touchPanelFlip = false;
    public bool button1Pressed, button2Pressed, joystickPressed;

    [SerializeField] private Text debugText1, debugText2; 

    public Vector2 joystickAxis;
    int button1PointerID, button2PointerID, joystickPointerID;
    float button1TouchTime, button2TouchTime;
    RectTransform rectTransform;
    private Vector2 touchStartPoint, touchDragPoint, touchEndPoint;
    DeviceOrientation deviceOrientation;

    private DataStructs.VRControllerStruct tempVRControllerData = default;
    [SerializeField] private bool isProtraitMode = false;
    
    void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        Debug.Log("UI Position : " + transform.GetComponent<RectTransform>().anchoredPosition.ToString());
        deviceOrientation = DeviceOrientation.LandscapeLeft;

        deviceOrientation = (Input.deviceOrientation == DeviceOrientation.FaceDown ||
                             Input.deviceOrientation == DeviceOrientation.FaceUp ||
                             Input.deviceOrientation == DeviceOrientation.Unknown)
                            ? deviceOrientation : Input.deviceOrientation;
        
        //Temp Struct Set
        tempVRControllerData.controllerType = controllerType;
        tempVRControllerData.isButton1Pressed = false;
        tempVRControllerData.isButton2Pressed = false;
        tempVRControllerData.isJoysticPressed = false;
        tempVRControllerData.joysticAxis = Vector2.zero;
        tempVRControllerData.deviceOrientation = deviceOrientation;
        
    }

    private void FixedUpdate()
    {
        if (debugText2 != null)
        {
            debugText2.text = "Controller Type : " + tempVRControllerData.controllerType.ToString() +
                              "\nIs Button 1 Pressed : " + tempVRControllerData.isButton1Pressed.ToString() +
                              "\nIs Button 2 Pressed : " + tempVRControllerData.isButton2Pressed.ToString() +
                              "\nIs Joystick Pressed : " + tempVRControllerData.isJoysticPressed.ToString() +
                              "\nJoystick Axis : " + tempVRControllerData.joysticAxis.ToString() +
                              "\nDevice Orientation : " + tempVRControllerData.deviceOrientation.ToString();
        }

        //Send Data
        TouchInfoManager.SetControllerData(tempVRControllerData);

        //Data�� ���� �ڿ� �ٽ� �� üũ�� �ϴ� ������
        //��� �ѹ��� Button�� ���ȴٴ� ��ȣ�� �� ������ ���ؼ� 
        
        //Temp Data Setting

        if (button1Pressed)
        {
            if(Time.time >= button1TouchTime)
                tempVRControllerData.isButton1Pressed = true;
            else
                tempVRControllerData.isButton1Pressed = false;
        }
        else
        {
            if(tempVRControllerData.isButton1Pressed)
                Debug.Log("Button 1 Short Click sended");
            tempVRControllerData.isButton1Pressed = false;
        }
        
        
        if (button2Pressed)
        {
            if(Time.time >= button2TouchTime)
                tempVRControllerData.isButton2Pressed = true;
            else
                tempVRControllerData.isButton2Pressed = false;
        }
        else
        {
            if(tempVRControllerData.isButton2Pressed)
                Debug.Log("Button 2 Short Click sended");
            tempVRControllerData.isButton2Pressed = false;
        }
        
        tempVRControllerData.isJoysticPressed = joystickPressed;

        if (joystickPressed)
            tempVRControllerData.joysticAxis = joystickAxis;
        else
            tempVRControllerData.joysticAxis = Vector2.zero;

        tempVRControllerData.deviceOrientation = deviceOrientation;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //Start������ ���ĵǱ� ���̶� ����� �� ���� ������ ����
        //UI Pivot Position : Vector2 pivotPosition = transform.GetComponent<RectTransform>().anchoredPosition;

        //Check Device Orientation
        deviceOrientation = (Input.deviceOrientation == DeviceOrientation.FaceDown ||
                             Input.deviceOrientation == DeviceOrientation.FaceUp ||
                             Input.deviceOrientation == DeviceOrientation.Unknown)
            ? deviceOrientation : Input.deviceOrientation;

        if (joystickPressed)
        {
            if (!isProtraitMode)
            {
                joystickAxis = (touchDragPoint - touchStartPoint).normalized;
            }
            else
            {
                joystickAxis = (touchDragPoint - touchStartPoint).normalized;
                joystickAxis = new Vector2(joystickAxis.y, -joystickAxis.x);
            }
        }

        if (debugText1 != null)
        {
            debugText1.text = "Button 1 Pressed : " + button1Pressed.ToString() +
                              "\nButton 1 PointerID : " + button1PointerID.ToString() +
                              "\nButton 2 Pressed : " + button2Pressed.ToString() +
                              "\nButton 2 PointerID : " + button2PointerID.ToString() +
                              "\nJoystick Pressed : " + joystickPressed.ToString() +
                              "\nJoystick Axis : " + joystickAxis.ToString() +
                              "\nCurrent Drag Point : " + touchDragPoint.ToString() +
                              "\nDevice Orientation : " + deviceOrientation.ToString();

            //Debug.Log("Device Orientation : " + Input.deviceOrientation);
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (joystickPressed)
            return;

        touchStartPoint = eventData.position;
        if (isButton1Area(eventData))
        {
            button1PointerID = eventData.pointerId;
            button1Pressed = true;
            button1TouchTime = Time.time + 0.1f;
        }
        else
        {
            button2PointerID = eventData.pointerId;
            button2Pressed = true;
            button2TouchTime = Time.time + 0.1f;
        }
        Debug.Log("Pointer Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (joystickPressed)
        {
            if (joystickPointerID == eventData.pointerId)
                joystickPressed = false;
        }
        if (button1Pressed)
        {
            if (button1PointerID == eventData.pointerId)
            {
                button1Pressed = false;
                if (Time.time < button1TouchTime)
                    tempVRControllerData.isButton1Pressed = true;
            }
        }
        if (button2Pressed)
        {
            if (button2PointerID == eventData.pointerId)
            {
                button2Pressed = false;
                if (Time.time < button2TouchTime)
                    tempVRControllerData.isButton2Pressed = true;
            }
        }
        Debug.Log("Pointer Up");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (button1Pressed && button2Pressed)
            return;

        if (!joystickPressed)
        {
            if ((button1Pressed && Time.time < button1TouchTime) ||
                (button2Pressed && Time.time < button2TouchTime))
            {
                /*
                if((eventData.pressPosition - eventData.position).magnitude > 0.2f)
                  
                 */
                
                    
                joystickPressed = true;
                joystickPointerID = eventData.pointerId;
                touchDragPoint = eventData.position;

                button1Pressed = false;
                button2Pressed = false;
            }
        }
        else
        {
            if(eventData.pointerId == joystickPointerID)
                touchDragPoint = eventData.position;
        }

        Debug.Log("Draging : " + eventData.position.ToString());
    }

    bool isButton1Area(PointerEventData eventData)
    {
        switch (deviceOrientation)
        {
            case DeviceOrientation.LandscapeLeft:
            case DeviceOrientation.Portrait:
                if ((touchPanelFlip) ?
                    (eventData.position.y >= -rectTransform.anchoredPosition.y):
                    (eventData.position.y < -rectTransform.anchoredPosition.y))
                    return true;
                else
                    return false;
            case DeviceOrientation.LandscapeRight:
            case DeviceOrientation.PortraitUpsideDown:
                if ((touchPanelFlip) ?
                    (eventData.position.y < -rectTransform.anchoredPosition.y) :
                    (eventData.position.y >= -rectTransform.anchoredPosition.y))
                    return true;
                else
                    return false;
        }
        return true;
    }
}
