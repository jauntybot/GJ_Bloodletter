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
    [SerializeField] EnemyPathfinding enemy;
    public Slider staminaSlider, bloodSlider, infectionSlider, exposureSlider, detectionSlider, terrorSlider;
    [SerializeField] Image infectedPanel;
    [SerializeField] Image radialBar;

    void Start() {
        bloodletter = Bloodletter.instance;
    }

    public void Update() {
        staminaSlider.value = bloodletter.staminaLevel;
        bloodSlider.value = bloodletter.bloodLevel;
        infectionSlider.value = bloodletter.infectionLevel;
        exposureSlider.value = bloodletter.exposureLevel;
        detectionSlider.value = enemy.detectionLevel;
        infectedPanel.color = new Color (infectedPanel.color.r, infectedPanel.color.g, infectedPanel.color.b, Mathf.Lerp (0, 0.33f, bloodletter.infectionLevel/100));
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
