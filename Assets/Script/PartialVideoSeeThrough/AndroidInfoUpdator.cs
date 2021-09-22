using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidInfoUpdator : MonoBehaviour
{
    // Start is called before the first frame update

    private Gyroscope gyro;
    [SerializeField] private Transform rotationInfo;
    Quaternion ToUnityGyro(Quaternion q) => new Quaternion(q.x, q.y, -q.z, -q.w);  
    
    void Start()
    {
        DataStructs.partialTrackingData.isTrack = false;
        DataStructs.partialTrackingData.trackedPosition = Vector2.zero;
        DataStructs.partialTrackingData.trackedRotation = Quaternion.identity.eulerAngles;

        gyro = Input.gyro;
        gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            DataStructs.partialTrackingData.isTrack = true;
            DataStructs.partialTrackingData.isRight = (touch.position.x > ((float)Screen.width / 2));
            DataStructs.partialTrackingData.trackedPosition = touch.position;
            /*
            Transform androidTransform = transform;
            androidTransform.rotation = gyro.attitude;
            androidTransform.Rotate(0f, 0f, 180f, Space.Self);
            androidTransform.Rotate(90f, 180f, 0f, Space.World);
            */
            rotationInfo.rotation = ToUnityGyro(gyro.attitude);
            DataStructs.partialTrackingData.trackedRotation = rotationInfo.rotation.eulerAngles;
        }
        else
        {
            DataStructs.partialTrackingData.isTrack = false;
            DataStructs.partialTrackingData.isRight = false;
            DataStructs.partialTrackingData.trackedPosition = Vector2.zero;
            DataStructs.partialTrackingData.trackedRotation = Quaternion.identity.eulerAngles;
            
        }
        
        
    }
}
