using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
public class TextPopUp : MonoBehaviour {

    Animator anim;
    [SerializeField] TMP_Text text;
    public string message;


    void Start() {
        anim = GetComponent<Animator>();
    }

    public void DisplayMessage(string _message, float time = 0) {

        text.text = _message;
        anim.ResetTrigger("Dismiss");
        anim.SetTrigger("Display");

        if (time != 0)
            StartCoroutine(WaitToDismiss(time));

    }

    public IEnumerator WaitToDismiss(float time) {
        float timer = 0;
        while (timer < time) {
            timer += Time.deltaTime;
            yield return null;
        }
        DismissMessage();
    }

    public void DismissMessage() {
        anim.ResetTrigger("Display");
        anim.SetTrigger("Dismiss");
    }

}
