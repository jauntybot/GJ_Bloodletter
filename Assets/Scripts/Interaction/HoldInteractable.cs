using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class HoldInteractable : Interactable {


    protected AudioSource audioSource;
    [SerializeField] protected SFX openSFX, loopSFX, closeSFX;
    protected Animator anim;
    public float maxContent;
    public float content;
    public float consumptionRate;
    public bool interacting;

    public override void Init() {
        base.Init();
        audioSource = GetComponent<AudioSource>();
        content = maxContent;
        anim = GetComponent<Animator>();
    }

    public override void Interact() {
        if (!hasInteracted) {
            FirstInteractionCallback?.Invoke();
            hasInteracted = true;
            PlaySound(openSFX);
        }
    }

    protected virtual void ExhaustSite() {
        PlaySound(closeSFX);
        locked = true;
        inRange = false; inView = false;
        anim.SetBool("Empty", true);
    }

    public virtual void PlaySound(SFX sfx = null) {
        if (sfx) {
            if (sfx.outputMixerGroup) 
                audioSource.outputAudioMixerGroup = sfx.outputMixerGroup;   
            
                audioSource.clip = sfx.Get();
                audioSource.Play();
        }
    }


}
