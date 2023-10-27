using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake() {
        if (GameManager.instance && GameManager.instance != this) {
            Destroy(this.gameObject);
            return;
        }
        GameManager.instance = this;
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    Bloodletter bloodletter;
    PauseManager pauseManager;

    public enum GameState { Menu, Running, Paused, Gameover };
    public GameState gameState;

    [SerializeField] bool cursorLock;
    [SerializeField] string sceneName;


    void Start() {
        sceneName = SceneManager.GetActiveScene().name;
    }

    public void ChangeState(GameState state) {
        gameState = state;
        if (state == GameState.Running) {
            LockCursor(true);

        } else {
            LockCursor(false);
        }
    }

    void Update() {
    	if (gameState == GameState.Running) {
// lock when mouse is clicked
           if(!cursorLock)
                LockCursor(!cursorLock);        
        } else {
            if(cursorLock)
                LockCursor(!cursorLock);        
        }
    }
    
    public void LockCursor(bool state) {
    	if (state) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        cursorLock = state;
    }

    public void KillPlayer() {
        DebugUI.instance.EnableGameover();
        gameState = GameState.Gameover;
        LockCursor(false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        bloodletter = Bloodletter.instance;
        pauseManager = PauseManager.instance;
    }

    public void RestartScene() {
        SceneManager.LoadScene(sceneName);
    }

    public void Pause(bool state) {
        pauseManager.Pause(state);
        gameState = state ? GameState.Paused : GameState.Running;
    }

}
