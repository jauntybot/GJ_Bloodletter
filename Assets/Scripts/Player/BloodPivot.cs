using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BloodPivot : MonoBehaviour
{
       
    Quaternion cameraRotation;

    // Start is called before the first frame update
    void Start()
    {
        cameraRotation = Camera.main.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        transform.rotation = Quaternion.Inverse(cameraRotation);

    }
}