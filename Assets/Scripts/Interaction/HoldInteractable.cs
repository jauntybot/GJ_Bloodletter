using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HoldInteractable : Interactable {


    [SerializeField] protected SFX openSFX, loopSFX, closeSFX;
    [SerializeField] protected float openDelay;
    [HideInInspector] public float timer;
    public float pilferDuration;
    
    public bool interacting;

    public override void Init() {
        base.Init();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    public override void Interact() {
        if (!hasInteracted) {
            FirstInteractionCallback?.Invoke();
            hasInteracted = true;
        }
        StartCoroutine(OpenSite());
    }

    protected virtual IEnumerator OpenSite() {
        interacting = true;
        timer = 0;
        if (openSFX)
            PlaySound(openSFX);
        float _timer = 0f;
        while (_timer < openDelay) {
            _timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(PilferSite());
    }

    protected virtual IEnumerator PilferSite() {
        if (loopSFX) {
            audioSource.loop = true;
            audioSource.clip = loopSFX.Get();
            audioSource.Play();
        }
        DebugUI.instance.StartCoroutine(DebugUI.instance.DisplayHoldInteract(this));
        while (Input.GetMouseButton(0) && interacting && inRange 
        && timer < pilferDuration) {
            while (!bloodletter.tick) {
                yield return null;
                if (!Input.GetMouseButton(0)) {
                    interacting = false;
                    bloodletter.interacting = false;
                    break;
                }
            }
            timer += 1;
            if (!Input.GetMouseButton(0) || !inRange) {
                interacting = false;
                bloodletter.interacting = false;
                break;
            }
            yield return null;    
        }
        if (audioSource.loop) {
            audioSource.loop = false;
            audioSource.Stop();
        }
        if (timer >= pilferDuration) {
            ExhaustSite();
        }
        bloodletter.interacting = false;
        bloodletter.interactingWith = null;
        interacting = false;
    }

    public virtual void ExhaustSite() {
        ExhaustSiteCallback?.Invoke();
        locked = true;
        inRange = false; inView = false;
        anim.SetBool("Empty", true);
        ExhaustSiteCallback?.Invoke();
    }

    public virtual void PlaySound(SFX sfx = null) {
        if (sfx) {
            if (sfx.outputMixerGroup) 
                audioSource.outputAudioMixerGroup = sfx.outputMixerGroup;   
            
                audioSource.PlayOneShot(sfx.Get());
        }
    }


}
