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
    [Range(0, 100)]
    public float hostilityLevel;
    
    [SerializeField] Transform poi;
    


    void Start() {
        bloodletter = Bloodletter.instance;
        enemy = GetComponent<EnemyPathfinding>();
        
    }



    public void UpdatePOI(Vector3 pos) {

        poi.position = pos;
    }


    

}
