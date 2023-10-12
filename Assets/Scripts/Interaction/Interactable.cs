using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableHighlight))]
public class Interactable : MonoBehaviour {

    [SerializeField] protected Bloodletter bloodletter;
    protected InteractableHighlight highlight;
    public bool interactOnce;
    bool hasInteracted;
    public float interactRadius = 2.5f;
    public bool inRange, inView;
    public bool locked = false;


    protected virtual void Start() {
        Init();
    }

    public void Init() {
        bloodletter = Bloodletter.instance;
        highlight = GetComponent<InteractableHighlight>();
    }
    
    protected virtual void Update() {
        if (bloodletter)
            inRange = Vector3.Distance(transform.position, bloodletter.transform.position) <= interactRadius;
    }

// CHECK IF INTERACTION WAS SUCCESSFUL
    public virtual void OnInteract() {
// Check if this can be interacted with
        if (inRange && !interactOnce || (interactOnce && !hasInteracted)) {
// Check if player can unlock this
            if (locked) {}
            if (!locked) {
                Interact();
            }
        }
    }

// OVERIDE FUNCTIONALITY IN INHERITED CLASSES
    public virtual void Interact() {}


    void OnDrawGizmosSelected () {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, interactRadius);
	}

}
