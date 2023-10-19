using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BloodPivot : MonoBehaviour
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
    void Update()
    {

        transform.rotation = Quaternion.Inverse(cameraRotation);

    }
}