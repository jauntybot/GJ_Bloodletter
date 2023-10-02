using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbiantAudio : MonoBehaviour {
    
    AudioSource audioSource;
    [SerializeField] SFX ambiantSFX;


    void Start() {
        audioSource = GetComponent<AudioSource>(); 
        audioSource.clip = ambiantSFX.Get();
        audioSource.outputAudioMixerGroup = ambiantSFX.outputMixerGroup;
        audioSource.Play();
    }

}
