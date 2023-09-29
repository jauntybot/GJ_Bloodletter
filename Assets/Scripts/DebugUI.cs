using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour {

    public static DebugUI instance;
    private void Awake() {
        if (DebugUI.instance) return;
        instance = this;
    }    

    Bloodletter bloodletter;
    public Slider staminaSlider, bloodSlider, infectionSlider;
    [SerializeField] GameObject infectedPanel;
    [SerializeField] Image radialBar;

    void Start() {
        bloodletter = Bloodletter.instance;
    }

    public void Update() {
        staminaSlider.value = bloodletter.staminaLevel;
        bloodSlider.value = bloodletter.bloodLevel;
        infectionSlider.value = bloodletter.infectionLevel;
        if (bloodletter.infectionLevel >= 100 && !infectedPanel.activeSelf) infectedPanel.SetActive(true); 
    } 

    public IEnumerator DisplayTransfusion(TransfusionSite site) {
        radialBar.gameObject.SetActive(true);
        radialBar.fillAmount = 1 - site.bloodContent/site.bloodTotal;
        while (site.transfusing) {
            yield return null;
            radialBar.fillAmount = 1 - site.bloodContent/site.bloodTotal;
        }
        radialBar.gameObject.SetActive(false);
    }

}
