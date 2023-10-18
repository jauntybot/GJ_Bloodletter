using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPool : MonoBehaviour {
    

    bool _inspected;
    public bool inspected { get { return _inspected; } }
    float _age;
    public float age { get { return _age; } }
    void Start() {
        _age = Time.time;
    }

    public void Inspect() {
        _inspected = true;
    }
}
