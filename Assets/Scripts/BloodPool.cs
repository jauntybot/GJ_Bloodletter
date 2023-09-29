using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPool : MonoBehaviour {
    

    bool inspected;
    
    float _age;
    public float age { get { return _age;}}
    void Start() {
        StartCoroutine(Age());
    }

    IEnumerator Age() {
        while (true) {
            _age += Time.deltaTime;
            yield return null;
        }
    }
}
