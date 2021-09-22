using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroTest : MonoBehaviour
{
    private Gyroscope androidGyro;


    // Start is called before the first frame update
    void Start()
    {
        androidGyro = Input.gyro;
        androidGyro.enabled = true;
    }

    Quaternion ToUnityGyro(Quaternion q) => new Quaternion(q.x, q.y, -q.z, -q.w);  

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = ToUnityGyro(androidGyro.attitude);
    }
}
