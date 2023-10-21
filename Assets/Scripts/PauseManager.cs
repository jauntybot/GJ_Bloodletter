using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{

    [SerializeField] AudioMixer mixer;
    public static PauseManager instance;
    public GameObject pause, endOfDemo;
    public Slider volSlider;
    public Image blackFade, demoBlackFade;
    void Awake() {
        if (PauseManager.instance && PauseManager.instance != this) {
            Destroy(gameObject);
            return;
        }
        PauseManager.instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start() {
        volSlider.onValueChanged.AddListener(delegate { UpdateVolume(); } );
        UpdateVolume();
    }

    public string mainmenuScene;
    public void Pause(bool state) {
        Time.timeScale = state ? 0 : 1;
        pause.SetActive(state);
    }

    public void Resume() {
        GameManager.instance.Pause(false);
    }

    public void QuitToMenu() {
        Time.timeScale = 1;
        StartCoroutine(FadeScene(true));
        Bloodletter.instance.ResetShaders();
        SceneManager.LoadScene(mainmenuScene);
    }

    void UpdateVolume() {
        mixer.SetFloat("MasterVol", Mathf.Log10(volSlider.value) * 20);
    }

    public IEnumerator FadeScene(bool state) {
        blackFade.gameObject.SetActive(true);
        blackFade.color = state ? Color.clear : Color.black;
        float timer = 0;
        float from = blackFade.color.a;
        float to = state ? 1 : 0;
        while (timer < 2) {
            blackFade.color = new Color(blackFade.color.b, blackFade.color.g, blackFade.color.b, Mathf.Lerp(from, to, timer/2));

            timer += Time.deltaTime;
            yield return null;
        }

        blackFade.color = state ? Color.black : Color.clear;
        blackFade.gameObject.SetActive(state);
    }

    public void EndOfDemo() {
        StartCoroutine(EndOfDemoCo());
    }

    IEnumerator EndOfDemoCo() {
        demoBlackFade.gameObject.SetActive(true);
        demoBlackFade.color = Color.clear;
        float timer = 0;
        while (timer < 6) {
            demoBlackFade.color = new Color(blackFade.color.b, blackFade.color.g, blackFade.color.b, Mathf.Lerp(0, 1, timer/6));
            timer += Time.deltaTime;
            yield return null;
        }
        demoBlackFade.color = Color.black;
        GameManager.instance.ChangeState(GameManager.GameState.Menu);
        yield return new WaitForSecondsRealtime(1);
        endOfDemo.SetActive(true);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {}

}
