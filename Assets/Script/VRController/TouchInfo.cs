using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInfo : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IDragHandler
{
    // Start is called before the first frame update

    private Vector2 touchStartPoint, touchEndPoint;
    
    void Start()
    {
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
        /*
        Debug.Log("Screen Width And Height : " + Screen.width.ToString() + " : " + Screen.height.ToString());
        Debug.Log(Screen.dpi);
        */
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStartPoint = eventData.position;
        Debug.Log("Pointer Down");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Pointer Click");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touchEndPoint = eventData.position;
        Debug.Log("Move Distance : " + Vector2.Distance(touchStartPoint, touchEndPoint) / Screen.dpi);
        Debug.Log("Pointer Up");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Draging : " + eventData.position.ToString());
    }
}
