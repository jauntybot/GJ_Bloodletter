using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugUI : MonoBehaviour {

    public static DebugUI instance;
    private void Awake() {
        if (DebugUI.instance) return;
        instance = this;
    }    

    Bloodletter bloodletter;
    [SerializeField] EnemyPathfinding enemy;
    public Slider staminaSlider, bloodSlider, infectionSlider, exposureSlider, detectionSlider, terrorSlider, hostilitySlider, energySlider, downtimeSlider;
    [SerializeField] Image radialBar;
    [SerializeField] TMP_Text tollCount;

    [SerializeField] GameObject gameoverPanel;

    public TextPopUp textPopUp;

    void Start() {
        bloodletter = Bloodletter.instance;
    }

    public void Update() {
        staminaSlider.value = bloodletter.staminaLevel;
        bloodSlider.value = bloodletter.bloodLevel;
        infectionSlider.value = bloodletter.infectionLevel;
        exposureSlider.value = bloodletter.exposureLevel;
        detectionSlider.value = enemy.detectionLevel;
        energySlider.value = enemy.energyLevel;
        terrorSlider.value = enemy.director.terrorLevel;
        hostilitySlider.value = enemy.director.hostilityLevel;
        downtimeSlider.value = enemy.director.downtime;
        tollCount.text = bloodletter.tollCount.ToString();
    } 

    public IEnumerator DisplayHoldInteract(HoldInteractable interact) {
        radialBar.gameObject.SetActive(true);
        radialBar.fillAmount = 1 - interact.timer/interact.pilferDuration;
        while (interact.interacting) {
            yield return null;
            radialBar.fillAmount = 1 - interact.timer/interact.pilferDuration;
        }
        radialBar.gameObject.SetActive(false);
    }
    public void EnableGameover() {
        gameoverPanel.SetActive(true);
    }

    public void RestartGame() {
        GameManager.instance.RestartScene();
    }

}
