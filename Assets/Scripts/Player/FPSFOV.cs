using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class FPSFOV : MonoBehaviour {


    CinemachineVirtualCamera vCam;
    Bloodletter bloodletter;


    void Start() {
        bloodletter = Bloodletter.instance;
    }


    void Update() {
        
        


    }
}
