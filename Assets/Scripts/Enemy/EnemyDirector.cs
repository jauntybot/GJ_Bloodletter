using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(EnemyPathfinding))]
public class EnemyDirector : MonoBehaviour {

    [HideInInspector] public EnemyPathfinding enemy;
    Bloodletter bloodletter;


    [Range(0, 100)]
    public float terrorLevel;
    float fitnessMod; // Used to locally track changes from bloodletter's wellness
    [Range(0, 100)]
    public float hostilityLevel;
    [SerializeField] float hostilityGainRate;
    [Range(0,100)]
    public float downtime;
    public float downtimeTimer;
    public Coroutine downtimeCo;
    public float downtimeThreshold;
    [SerializeField] AnimationCurve downtimeCurve;
    
    public List<Interactable> interactables;

    public Transform poi;
    


    void Start() {
        bloodletter = Bloodletter.instance;
        enemy = GetComponent<EnemyPathfinding>();
        StartCoroutine(PassiveTracking());
        foreach (Interactable inter in FindObjectsOfType<Interactable>())
            interactables.Add(inter);

        poi.parent = transform.parent;
    }



    public IEnumerator PassiveTracking() {
        fitnessMod = 0;
        while (true) {
            terrorLevel -= fitnessMod;
            fitnessMod = Mathf.Lerp(0, 15, Mathf.InverseLerp(0, 100, 100 - bloodletter.bloodLevel)) + Mathf.Lerp(0, 15, Mathf.InverseLerp(0, 100, bloodletter.infectionLevel)) + Mathf.Lerp(0, 10, Mathf.InverseLerp(0, 100, 100 - bloodletter.staminaLevel));
            terrorLevel += fitnessMod;
            hostilityLevel += hostilityGainRate * terrorLevel;

            yield return null;
        }
    }

    public IEnumerator Downtime() {
        downtimeTimer = 0f;
        float timer = 0f;
        while (downtimeTimer <= downtimeThreshold) {
            if (!enemy.detecting) {
                downtimeTimer += Time.deltaTime;
                downtime = downtimeCurve.Evaluate(downtimeTimer/downtimeThreshold) * 100;
            }
                
            yield return null;
        }
    }



    public void UpdatePOI(Vector3 pos) {

        poi.position = pos;
    }


    

}
