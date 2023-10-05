using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;


public class FullScreenEffectController : MonoBehaviour
{
    [Header("Time Stats")]
    [SerializeField] private float _bloodDisplayTime = 1.5f;
    [SerializeField] private float _bloodFadeOutTime = 0.5f;

    [Header("Intensity")]
    [SerializeField] private float _voronoiIntensityStat;
    [SerializeField] private float _vignetteIntensityStat;
    [SerializeField] private float _distortionStrengthStat;
    [SerializeField] private float _hueStrengthStat;
    
    [Header("References")]
    [SerializeField] private ScriptableRendererFeature _fullScreenBlood;
    [SerializeField] private Material _material;

    private int _voronoiIntensity = Shader.PropertyToID(("_VoronoIntensity"));
    private int _vignetteIntensity = Shader.PropertyToID(("_VignetteIntensity"));
    private int _distortionStrength = Shader.PropertyToID(("_DistortionStrength"));
    private int _hueStrength = Shader.PropertyToID(("_HueStrength"));
    

    private void Start()
    {
        _fullScreenBlood.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Hurt());
        }
    }

    private IEnumerator Hurt()
    {
        print("Coroutine start");
        _fullScreenBlood.SetActive(true);
        _material.SetFloat(_voronoiIntensity, _voronoiIntensityStat);
        _material.SetFloat(_vignetteIntensity, _vignetteIntensityStat);
        
        _material.SetFloat(_distortionStrength, _distortionStrengthStat);
        _material.SetFloat(_hueStrength, _hueStrengthStat);

        float elapsedTime = 0f;
        while (elapsedTime < _bloodFadeOutTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedVornoi = Mathf.Lerp(0f, _voronoiIntensityStat, (elapsedTime / _bloodFadeOutTime));
            float lerpedVignette = Mathf.Lerp(0f, _vignetteIntensityStat, (elapsedTime / _bloodFadeOutTime));

            float lerpedHue = Mathf.Lerp(0f, _hueStrengthStat, (elapsedTime / _bloodFadeOutTime));
            
            _material.SetFloat(_voronoiIntensity, lerpedVornoi);
            _material.SetFloat(_vignetteIntensity, lerpedVignette);
            
            _material.SetFloat(_hueStrength, lerpedHue);
            
            yield return null;
        }

        yield return new WaitForSeconds(_bloodDisplayTime);

        elapsedTime = 0f;
        while (elapsedTime < _bloodFadeOutTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedVornoi = Mathf.Lerp(_voronoiIntensityStat, 0f, (elapsedTime / _bloodFadeOutTime));
            float lerpedVignette = Mathf.Lerp(_vignetteIntensityStat, 0f, (elapsedTime / _bloodFadeOutTime));
            
            _material.SetFloat(_voronoiIntensity, lerpedVornoi);
            _material.SetFloat(_vignetteIntensity, lerpedVignette);
            
            yield return null;
        }
        
        _fullScreenBlood.SetActive(false);
        print("Coroutine end");
    }

}
