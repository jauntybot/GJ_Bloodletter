using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDirector : MonoBehaviour {

    public static EnemyDirector instance;
    void Awake() {
        if (EnemyDirector.instance) return;
        instance = this;
    }

    public EnemyPathfinding enemy;
    Bloodletter bloodletter;


    [Range(0, 100)]
    public float terrorLevel;
    [Range(0, 100)]
    public float hostilityLevel;
    
    [SerializeField] Transform poi;
    


    void Start() {
        bloodletter = Bloodletter.instance;

        
    }



    public void UpdatePOI(Vector3 pos) {

        poi.position = pos;
    }


    

}
