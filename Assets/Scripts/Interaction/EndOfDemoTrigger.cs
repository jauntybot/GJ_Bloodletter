using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EndOfDemoTrigger : MonoBehaviour
{



    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") PauseManager.instance.EndOfDemo();

    }

}
