using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public float downTime;
    bool downTimeActive;
    public float downTimeThreshold;
    [SerializeField] AnimationCurve downTimeCurve;
    
    public List<Interactable> interactables;

    public Transform poi;
    


    void Start() {
        bloodletter = Bloodletter.instance;
        enemy = GetComponent<EnemyPathfinding>();
        StartCoroutine(PassiveTracking());
        foreach (Interactable inter in FindObjectsOfType<Interactable>())
            interactables.Add(inter);
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
        if (!downTimeActive) {
            downTimeActive = true;
            float timer = 0f;
            while (timer <= downTimeThreshold) {
                timer += Time.deltaTime;
                downTime = downTimeCurve.Evaluate(timer/downTimeThreshold) * 100;
                
                yield return null;
            }
            downTimeActive = false;
        }
    }



    public void UpdatePOI(Vector3 pos) {

        poi.position = pos;
    }


    

}
