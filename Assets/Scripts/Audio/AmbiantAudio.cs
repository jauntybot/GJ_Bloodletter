using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbiantAudio : MonoBehaviour {
    
    AudioSource audioSource;
    [SerializeField] SFX ambiantSFX;


    void Start() {
        audioSource = GetComponent<AudioSource>(); 
        PlaySound(audioSource, ambiantSFX);
    }

    public virtual void PlaySound(AudioSource source, SFX sfx = null) {
        if (sfx) {
            if (sfx.outputMixerGroup) 
                source.outputAudioMixerGroup = sfx.outputMixerGroup;   

            source.PlayOneShot(sfx.Get());
        }
    }
}
