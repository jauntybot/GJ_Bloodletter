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
    [SerializeField] AnimationCurve falloffCurve;
    [SerializeField] float falloffDist, effectAmp, effectFreq;



    void Start() {
        bloodletter = GetComponent<Bloodletter>();
        enemy = EnemyPathfinding.instance;
    }


    float _freq;
    float _amp;
    void Update() {

        if (enemy.state == EnemyPathfinding.EnemyState.Chasing) {
            float strength = falloffCurve.Evaluate((falloffDist - Mathf.Clamp(enemy.proximity, 0, falloffDist)) / falloffDist);
            _amp = effectAmp * strength;
            _freq += Time.deltaTime * effectFreq * strength;
            float sinMod = Mathf.Sin(_freq) * _amp + _amp;
        
            

            //Debug.Log("Strength: " + strength + ", sinMod: " + sinMod);
            skyboxMat.SetFloat("_Exposure", Mathf.Lerp(skyboxExposureRange.x, skyboxExposureRange.y, Mathf.InverseLerp(0, effectAmp * effectFreq, sinMod * strength)));
            pointLight.intensity = Mathf.Lerp(pointLightRange.x, pointLightRange.y, Mathf.InverseLerp(0, effectAmp * effectFreq, sinMod * strength));
            RenderSettings.fogColor = Color.Lerp(fogStart, Color.black, sinMod / effectAmp * effectFreq);
        } else {
            skyboxMat.SetFloat("_Exposure", skyboxExposureRange.x);
            pointLight.intensity = pointLightRange.x;
            RenderSettings.fogColor = fogStart;
        }
    }

}
