using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


[System.Serializable]
[CreateAssetMenu(fileName = "New Effect", menuName = "ScriptableObjects/Fullscreen Effect")]
public class FullscreenEffect : ScriptableObject {

    public ScriptableRendererFeature rendererFeature;
    public Material material;
    public List<EffectProperty> properties;

}

[System.Serializable]
public class EffectProperty {

    public string _shaderProperty;
    public int shaderProperty {
        get { return Shader.PropertyToID(_shaderProperty); }
    } 

    public Vector2 range;
    public Vector2 threshold;
    public AnimationCurve curve;

}