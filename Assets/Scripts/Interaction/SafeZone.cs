using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour {

    TollSite site;
    bool active;

    void Start() {
        site = GetComponentInParent<TollSite>();
    }

    void OnTriggerEnter(Collider other) {

        if (!active && site.locked && other.GetComponent<BloodPool>()) {
            site.StartCoroutine(site.ActivateSafeZone());
            active = true;
        }

    }

}
