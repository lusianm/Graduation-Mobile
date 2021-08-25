using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ContollerType
{
    RightController = 1, LeftController = 2
}



public class TouchInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // Start is called before the first frame update
    public ContollerType controllerType = ContollerType.RightController;
    public bool touchPanelFlip = false;
    public bool button1Pressed, button2Pressed, joystickPressed;

    public Text debugText; 

    public Vector2 joystickAxis;
    int button1PointerID, button2PointerID, joystickPointerID;
    float button1TouchTime, button2TouchTime;
    RectTransform rectTransform;
    private Vector2 touchStartPoint, touchDragPoint, touchEndPoint;
    DeviceOrientation deviceOrientation;
    
    void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        Debug.Log("UI Position : " + transform.GetComponent<RectTransform>().anchoredPosition.ToString());


        Screen.orientation = ScreenOrientation.Landscape;
        deviceOrientation = DeviceOrientation.LandscapeLeft;
        Debug.Log(Screen.dpi);
        
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


        Debug.Log(Screen.orientation);
        
        Debug.Log("Screen Width And Height : " + Screen.width.ToString() + " : " + Screen.height.ToString());
        Debug.Log(Screen.dpi);
    }

    // Update is called once per frame
    void Update()
    {
        //Start에서는 정렬되기 전이라서 제대로 된 값이 나오지 않음
        //UI Pivot Position : Vector2 pivotPosition = transform.GetComponent<RectTransform>().anchoredPosition;

        //Check Device Orientation
        if (Input.deviceOrientation != DeviceOrientation.FaceDown
            && Input.deviceOrientation != DeviceOrientation.FaceUp
            && Input.deviceOrientation != DeviceOrientation.Unknown)
        {
            deviceOrientation = Input.deviceOrientation;
        }

        if (joystickPressed)
        {
            joystickAxis = (touchDragPoint - touchStartPoint).normalized;
        }

        debugText.text = "Button 1 Pressed : " + button1Pressed.ToString() +
            "\nButton 1 PointerID : " + button1PointerID.ToString() +
            "\nButton 2 Pressed : " + button2Pressed.ToString() +
            "\nButton 2 PointerID : " + button2PointerID.ToString() +
            "\nJoystick Pressed : " + joystickPressed.ToString() +
            "\nJoystick Axis : " + joystickAxis.ToString() +
            "\nDevice Orientation : " + deviceOrientation.ToString();

        //Debug.Log("Device Orientation : " + Input.deviceOrientation);

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
                button1Pressed = false;
        }
        if (button2Pressed)
        {
            if (button2PointerID == eventData.pointerId)
                button2Pressed = false;
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
                joystickPressed = true;
                joystickPointerID = eventData.pointerId;
                touchDragPoint = eventData.position;

                button1Pressed = false;
                button2Pressed = false;
            }
        }
        else
        {
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
                    (eventData.position.y < rectTransform.anchoredPosition.y):
                    (eventData.position.y >= rectTransform.anchoredPosition.y))
                    return true;
                else
                    return false;
            case DeviceOrientation.LandscapeRight:
            case DeviceOrientation.PortraitUpsideDown:
                if ((touchPanelFlip) ?
                    (eventData.position.y >= rectTransform.anchoredPosition.y) :
                    (eventData.position.y < rectTransform.anchoredPosition.y))
                    return true;
                else
                    return false;
        }
        return true;
    }
}
