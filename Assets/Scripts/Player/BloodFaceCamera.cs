using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFaceCamera : MonoBehaviour {   
    
    // Update is called once per frame
    void LateUpdate()
    {
        
        transform.LookAt(Camera.main.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}