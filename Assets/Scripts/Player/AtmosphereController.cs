using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bloodletter))]
public class AtmosphereController : MonoBehaviour
{

    Bloodletter bloodletter;

    [SerializeField] Material skyboxMat;
    [SerializeField] Light pointLight;
    [SerializeField] Vector2 pointLightRange, skyboxExposureRange;
    [SerializeField] Color fogStart;


    void Start() {
        bloodletter = GetComponent<Bloodletter>();
    }


    void Update() {

        skyboxMat.SetFloat("_Exposure", Mathf.Lerp(skyboxExposureRange.x, skyboxExposureRange.y, Mathf.InverseLerp(0, 100, bloodletter.infectionLevel)));
        pointLight.intensity = Mathf.Lerp(pointLightRange.x, pointLightRange.y, Mathf.InverseLerp(0, 100, bloodletter.infectionLevel));
        RenderSettings.fogColor = Color.Lerp(fogStart, Color.black, bloodletter.infectionLevel / 100);




    }

}
