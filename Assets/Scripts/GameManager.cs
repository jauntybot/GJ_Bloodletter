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

    public enum GameState { Menu, Running, Paused, Gameover };
    public GameState gameState;

    bool cursorLock;
    [SerializeField]
    string sceneName;


    void Start() {
        sceneName = SceneManager.GetActiveScene().name;
        gameState = GameState.Running;
        LockCursor(true);
    }

    void Update() {
    	if (gameState == GameState.Running) {
// lock when mouse is clicked
           if(Input.GetMouseButtonDown(0) && !cursorLock )
                LockCursor(!cursorLock);        
// unlock when escape is hit
            if  (Input.GetKeyDown(KeyCode.Escape) )
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
        gameState = GameState.Running;
        LockCursor(true);

    }


    public void RestartScene() {
        SceneManager.LoadScene(sceneName);
    }

}
