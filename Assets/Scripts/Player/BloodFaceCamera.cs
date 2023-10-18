using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFaceCamera : MonoBehaviour
{
    //have blood always facing downwards.
    public GameObject mainCamera;
    private Quaternion cameraRotation;

    // Start is called before the first frame update
    void Start() 
    {
        Transform cameraTransform = mainCamera.GetComponent<Transform>();
        cameraRotation = cameraTransform.transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.forward = mainCamera.transform.forward;
        transform.LookAt(mainCamera.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}