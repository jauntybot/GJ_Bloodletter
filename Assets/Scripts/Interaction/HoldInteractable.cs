using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HoldInteractable : Interactable {

    protected Animator anim;
    public float maxContent;
    public float content;
    public float consumptionRate;
    public bool interacting;

    public override void Init()
    {
        base.Init();
        content = maxContent;
        anim = GetComponent<Animator>();
    }

    protected virtual void ExhaustSite() {
        locked = true;
        inRange = false; inView = false;
        anim.SetBool("Empty", true);
    }

}
