using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IAudioSource {


    public virtual void PlaySound(AudioSource source, SFX sfx = null) {}
}
