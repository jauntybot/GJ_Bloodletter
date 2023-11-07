using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableHighlight))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Interactable : MonoBehaviour {

// REFERENCES
    protected Bloodletter bloodletter;
    protected InteractableHighlight highlight;
    protected AudioSource audioSource;
    protected Animator anim;

// PARAMETERS
    public bool interactOnce;
    protected bool hasInteracted;
    public float interactRadius = 2.5f;
    public bool inRange, inView;
    public bool locked = false;

    public delegate void OnInteraction();
    public OnInteraction FirstInteractionCallback;
    public OnInteraction ExhaustSiteCallback;


    protected virtual void Start() {
        Init();
    }

    public virtual void Init() {
        bloodletter = Bloodletter.instance;
        highlight = GetComponent<InteractableHighlight>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }
    
    protected virtual void Update() {
        if (bloodletter && !locked && GameManager.instance.gameState == GameManager.GameState.Running) {
            inRange = Vector3.Distance(transform.position, bloodletter.transform.position) <= interactRadius;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.GetComponentInParent<Interactable>() == this) inView = true;
                else inView = false;
            } else inView = false;
        }
    }

// CHECK IF INTERACTION WAS SUCCESSFUL
    public virtual void OnInteract() {
// Check if this can be interacted with
        if (inRange && inView && (!interactOnce || (interactOnce && !hasInteracted))) {
// Check if player can unlock this
            if (locked) {}
            if (!locked) {
                bloodletter.interacting = true;
                bloodletter.interactingWith = this;
                Interact();
            }
        }
    }

// OVERIDE FUNCTIONALITY IN INHERITED CLASSES
    public virtual void Interact() {
        if (!hasInteracted) {
            FirstInteractionCallback?.Invoke();
            hasInteracted = true;
        }
    }


    void OnDrawGizmosSelected () {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, interactRadius);
	}

}
