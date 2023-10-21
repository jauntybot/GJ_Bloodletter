using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class MainMenuManager : MonoBehaviour {

    AudioSource audioSource;
    public string gameScene;
    public Image blackFade;
    

    void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
        GameManager.instance.ChangeState(GameManager.GameState.Menu);
        StartCoroutine(FadeScene(false));
    }

    public void StartGameScene() {
        StartCoroutine(FadeToGameScene());

    }

    IEnumerator FadeToGameScene() {
        yield return FadeScene(true);
        SceneManager.LoadScene(gameScene);
    }
    public IEnumerator FadeScene(bool state) {
        blackFade.gameObject.SetActive(true);
        blackFade.color = state ? Color.clear : Color.black;
        float timer = 0;
        float from = blackFade.color.a;
        float to = state ? 1 : 0;
        while (timer < 2) {
            blackFade.color = new Color(blackFade.color.b, blackFade.color.g, blackFade.color.b, Mathf.Lerp(from, to, timer/2));
            audioSource.volume = Mathf.Lerp(to, from, timer/2);
            timer += Time.deltaTime;
            yield return null;
        }

        blackFade.color = state ? Color.black : Color.clear;
        blackFade.gameObject.SetActive(state);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        
    }




}
