using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bloodletter))]
public class AtmosphereController : MonoBehaviour
{

    Bloodletter bloodletter;
    EnemyPathfinding enemy;

    [SerializeField] Material skyboxMat;
    [SerializeField] Light pointLight;
    [SerializeField] Vector2 pointLightRange, skyboxExposureRange;
    [SerializeField] Color fogStart;

    [SerializeField] float falloffDist, effectAmp, effectFreq;



    void Start() {
        bloodletter = GetComponent<Bloodletter>();
        enemy = EnemyPathfinding.instance;
    }


    void Update() {

        if (enemy.state == EnemyPathfinding.EnemyState.Chasing) {
            float strength = (falloffDist - Vector3.Distance(enemy.transform.position, bloodletter.transform.position)) / falloffDist;
            float sinMod = Mathf.Sin(Time.time * effectFreq) * effectAmp;
            
            skyboxMat.SetFloat("_Exposure", Mathf.Lerp(skyboxExposureRange.x, skyboxExposureRange.y, Mathf.InverseLerp(0, falloffDist, sinMod * (falloffDist - Vector3.Distance(enemy.transform.position, bloodletter.transform.position)))));
            pointLight.intensity = Mathf.Lerp(pointLightRange.x, pointLightRange.y, Mathf.InverseLerp(0, 100, sinMod * (falloffDist - Vector3.Distance(enemy.transform.position, bloodletter.transform.position))));
            RenderSettings.fogColor = Color.Lerp(fogStart, Color.black, sinMod * strength);
        } else {
            skyboxMat.SetFloat("_Exposure", skyboxExposureRange.x);
            pointLight.intensity = pointLightRange.x;
            RenderSettings.fogColor = fogStart;
        }
    }

}
