using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour {

    public Bloodletter bloodletter;
    public Slider staminaSlider, bloodSlider, infectionSlider;

    

    public void Update() {
        staminaSlider.value = bloodletter.staminaLevel;
        bloodSlider.value = bloodletter.bloodLevel;
        infectionSlider.value = bloodletter.infectionLevel;
    } 



}
