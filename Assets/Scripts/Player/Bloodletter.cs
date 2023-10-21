using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class Bloodletter : MonoBehaviour {


    public static Bloodletter instance;
    void Awake() {
        if (Bloodletter.instance) Destroy(Bloodletter.instance.gameObject);
        Bloodletter.instance = this;
    }

    EnemyPathfinding enemy;
    [Header("Stats")]
    [Range(0,100)] public float bloodLevel;
    [Range(0,100)] public float infectionLevel, staminaLevel;
    public int tollCount;
    public const float potencyMin = 0.125f, potencyMax = 1f;
    public Vector2 potencyRange { get { return new Vector2(potencyMin, potencyMax); } }
    [Range(potencyMin, potencyMax)]
    public float infectionPotency;
    public float infectionSpeed, potencyIncrement;
    [SerializeField] List<FullscreenEffect> infectionEffects;
    [SerializeField] FullscreenEffect bloodEffect;


    [Header("States")]
    [SerializeField]
    bool sprinting;
    [SerializeField]
    bool crouching, staminaRegen, heavyBreathing, bloodletting, bloodRegen;

    [Header("Prefabs")]
    [SerializeField] GameObject bloodDecal;
    [SerializeField] List<Animator> handAnims;
    [SerializeField] SFX bloodletSFX, footstepWalkSFX, footstepRunSFX, heavyBreathingSFX;
    PlayableDirector cutsceneDirector;

    [Header("Controller")]	
    public bool alive;
    [SerializeField] AudioSource breathAudioSource, footstepAudioSource, oneShotAudioSource;
    [SerializeField] CinemachineVirtualCamera fpsCam, killCam;
    [SerializeField] Transform cameraRoot;
    [SerializeField] Vector2 cameraHeight = new Vector2(0.8f, -0.5f);
    [SerializeField] AnimationCurve crouchCurve;
    [SerializeField] float crouchDur;
    public float walkSpeed;
    [SerializeField] Vector2 sprintMultiplierRange;
    float sprintMultiplier { get { return Mathf.Lerp(sprintMultiplierRange.x, sprintMultiplierRange.y, Mathf.InverseLerp(0, 60, staminaLevel)); }}
    public float standMultiplier;
    [SerializeField] private float _moveSpeed;
    public float moveSpeed {
        get { return walkSpeed * _speedMultiplier; }
    }
    [SerializeField] private float _speedMultiplier;
    [SerializeField] public float speedMultiplier {
        get { return _speedMultiplier; }
    }

    [SerializeField] float staminaDrainRate, staminaDelay, staminaRegenRamp, staminaRegenRate, sprintDelay, bloodDrainRate, bloodDelay, bloodRegenRate;

    [SerializeField] float tickDur;
    float curTick;
    [HideInInspector] public bool tick;
    
	public Camera cam;
	public LayerMask interactionMask;

    [Header("Rates")]
    [SerializeField] float bloodletSFXDelay;
    [SerializeField] float decalSprintDelay, decalWalkDelay, footstepDelay, footstepRunDelay, heavyBreathingDelay;
    [SerializeField] AnimationCurve regenAnimationCurve;
    
    
    [Header("Exposure")]
    [Range(0,100)]
    public float exposureLevel;
    [SerializeField] AnimationCurve exposureCurve;
    [SerializeField] float expBase, expStill, expCrouch, expWalk, expSprint, expBloodletting, expInfected;
    public DetectionCone fovCone;
    [Range(0, 60)] public float enemyTerror;
    public AnimationCurve terrorProximity;
    [SerializeField] float terrorRate, terrorMod;
    

    [Header("Cinemachine Values")]
    [SerializeField] float baseFOV;
    [SerializeField] float sprintFOVMod, bloodletFOVMod, baseFOVAmplitude, bloodletFOVAmplitude, headbobFrequency;
    [SerializeField] Vector2 sprintFOVAmplitudeRange;
    [SerializeField] float FOV() {
        float fov = baseFOV;
        if (sprinting) fov += sprintFOVMod;
        if (bloodletting) fov += bloodletFOVMod;
        return fov;
    }
    float amp() {
        float amp = baseFOVAmplitude;
        if (sprinting) amp += Mathf.Lerp(sprintFOVAmplitudeRange.x, sprintFOVAmplitudeRange.y, Mathf.InverseLerp(0, 60, staminaLevel));
        if (bloodletting) amp += bloodletFOVAmplitude;
        amp += enemyTerror/60;
        amp = Mathf.Clamp(amp, 1, 4);
        return amp;
    }
    [SerializeField] Transform armsRoot;


    void Start() {
        Init();
    }

    public void Init() {
        alive = true;
        enemy = EnemyPathfinding.instance;
        
        cutsceneDirector = GetComponent<PlayableDirector>();
        StartCoroutine(InfectionSpread());
        StartCoroutine(BloodletterTick());
        StartCoroutine(Exposure());
        StartCoroutine(FootstepCheck());

        breathAudioSource.clip = heavyBreathingSFX.Get();
        breathAudioSource.volume = 0;
        breathAudioSource.loop = true;
        breathAudioSource.Play();

        GameManager.instance.ChangeState(GameManager.GameState.Running);
    }

    IEnumerator BloodletterTick() {
        while (true) {
            tick = false;
            curTick += Time.deltaTime;
            if (curTick > tickDur) {
                curTick = 0;
                tick = true;
            }
// SNEAK IN SHADER GRAPH SETTING
            foreach (FullscreenEffect fx in infectionEffects) {
                foreach (EffectProperty prop in fx.properties) {
                    fx.material.SetFloat(prop.shaderProperty, prop.range.x + prop.curve.Evaluate((infectionLevel - prop.threshold.x * 100) / ((prop.threshold.y - prop.threshold.x) * 100)) * prop.range.y);
                }
            }
// SNEAK IN TERROR LEVEL
            if (!enemy.hidden) {
                if (Vector3.Distance(transform.position, enemy.transform.position) < fovCone.dist) {
                    Vector3 dir = (enemy.transform.position - transform.position).normalized;
                    float angleDelta = Vector3.Angle(transform.forward, dir);
                    if (angleDelta < fovCone.viewAngle / 2f) {
                        if (!Physics.Linecast(transform.position, enemy.transform.position, fovCone.viewMask)) {
                            if (!fovCone.detecting) {
                                fovCone.detecting = true;
                            }
                            terrorMod = terrorProximity.Evaluate((fovCone.dist - Vector3.Distance(transform.position, enemy.transform.position))/fovCone.dist) * 10;
                        }
                        fovCone.inRange = true;
                    } else {
                        terrorMod -= terrorRate;
                        terrorMod = Mathf.Clamp(terrorMod, 0.25f, 1);
                        fovCone.detecting = false;
                        fovCone.inRange = false;
                    }
                } else {
                    terrorMod -= terrorRate;
                    terrorMod = Mathf.Clamp(terrorMod, -0.25f, 1);
                }
            } else {
                terrorMod -= terrorRate;
                terrorMod = Mathf.Clamp(terrorMod, -0.25f, 1);
            }
            enemyTerror += terrorRate * terrorMod;
            enemyTerror = Mathf.Clamp(enemyTerror, 0, 60);

            bloodEffect.material.SetFloat(bloodEffect.properties[0].shaderProperty, bloodEffect.properties[0].range.x + bloodEffect.properties[0].curve.Evaluate(1 - (bloodLevel/100)) * (bloodEffect.properties[0].range.y - bloodEffect.properties[0].range.x));
            
            yield return null;
        }
    }

    public IEnumerator InfectionSpread() {
        while (true) {
            while (!tick) yield return null;
            infectionSpeed = bloodLevel/100;
            if (!bloodletting) {
                if (infectionPotency + potencyIncrement < potencyRange.y)
                    infectionPotency += potencyIncrement;
                else infectionPotency = potencyRange.y;
                if (infectionLevel + infectionPotency < 100)
                    infectionLevel += infectionPotency * infectionSpeed;
                else infectionLevel = 100;
            }
            yield return null;
        }
    }

    public IEnumerator Sprint() {
        sprinting = true;
// RAMP UP TO SPRINT SPEED AND FOV LERP
        float timer = 0;
        
        StartCoroutine(LerpFOV(FOV(), sprintDelay));
        StartCoroutine(LerpFOVAmp(amp(), sprintDelay));
        while (timer < sprintDelay) {
            timer += Time.deltaTime;    
            _speedMultiplier = Mathf.Lerp(1, sprintMultiplier, timer / sprintDelay);
            if (!Input.GetButton("Run") || (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)) {
                sprinting = false;
                break;
            }
            yield return null;
        }

// WHILE LOOP OF SPRINTING
        while (sprinting) {
            _speedMultiplier = sprintMultiplier;
            fpsCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amp();
            fpsCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = sprintMultiplier * headbobFrequency;
            while (!tick) {
                if (!Input.GetButton("Run") || (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)) {
                    sprinting = false;
                    break;
                }
                yield return null;
            }
            if (sprinting && staminaLevel >= staminaDrainRate)
                staminaLevel -= staminaDrainRate;
            else if (sprinting)
                staminaLevel = 0;

            if (staminaLevel <= 0 && !heavyBreathing) StartCoroutine(HeavyBreathing());

            yield return null;
        }
        _speedMultiplier = 1f;
        sprinting = false;

        StartCoroutine(LerpFOV(FOV(), sprintDelay));    
        StartCoroutine(LerpFOVAmp(baseFOVAmplitude, sprintDelay));
        fpsCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 1;
    }

    public IEnumerator RegainStamina() {
        staminaRegen = true;
// DELAY START OF STAMINA REGEN
        float regen = 0;
        while (regen < staminaDelay) {
            regen += Time.deltaTime;
            if (Input.GetButton("Run")) {
                staminaRegen = false;
                break;
            }
            yield return null;
        }
// REGENERATE STAMINA
        regen = 0;
        while (staminaRegen && staminaLevel < 100) {
            while (!tick) {
                if (Input.GetButton("Run")) {
                    staminaRegen = false;
                    break;
                }
                yield return null;
                regen += Time.deltaTime;
            }

            if (staminaRegen)
                staminaLevel += staminaRegenRate * regenAnimationCurve.Evaluate(regen/staminaRegenRamp);

            yield return null;
        }
        staminaRegen = false;
    }

    IEnumerator HeavyBreathing() {
        heavyBreathing = true;
        float timer = 0;
        while (timer < 1) {
            timer += Time.deltaTime;
            breathAudioSource.volume = Mathf.Lerp(0, 1, timer/1);
            yield return null;
        }
        while (staminaLevel <= 25) {
            yield return null;
        }
        timer = 0;
        while (timer < 1) {
            timer += Time.deltaTime;
            breathAudioSource.volume = Mathf.Lerp(1, 0, timer/1);
            yield return null;
        }
        heavyBreathing = false;
    }

    void ToggleBloodletting(bool state) {
        if (state && bloodLevel > 0) {
            bloodletting = state;
            StartCoroutine(Bloodlet());
            StartCoroutine(BloodletSFX());
            StartCoroutine(BloodletDecal());
            foreach(Animator anim in handAnims)
                anim.SetBool("Bloodletting", true);
        } else {
            bloodletting = false;
            foreach(Animator anim in handAnims)
                anim.SetBool("Bloodletting", false);
        }
        StartCoroutine(LerpFOVAmp(amp(), bloodDelay));
    }

    public IEnumerator Bloodlet() {
        while (bloodletting && bloodLevel > 0) {
            while (!tick) {
                if (Input.GetButtonDown("Bloodlet")) 
                    break;
                yield return null;
            } 
            if (bloodletting) {
                bloodLevel -= bloodDrainRate * _speedMultiplier;
            }
            
            yield return null;
        }

        if (bloodLevel <= 0) bloodletting = false;
        ToggleBloodletting(false);
    }

    IEnumerator LerpFOVAmp(float target, float duration) {
        float startAmp = fpsCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain;
        float timer = 0;
        while (timer < duration) {
            timer += Time.deltaTime;
            fpsCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Mathf.Lerp(startAmp, target, timer/bloodDelay);

            yield return null;
        }
        fpsCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = target;
    }

    IEnumerator LerpFOV(float target, float duration) {
        float startFOV = fpsCam.m_Lens.FieldOfView;
        float timer = 0;
        while (timer < duration) {
            timer += Time.deltaTime;
            fpsCam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, target, timer/duration);

            yield return null;
        }
        fpsCam.m_Lens.FieldOfView = target;
    }

    IEnumerator BloodletSFX() {
        while (bloodletting) {
            float timer = 0f;

            while (bloodletSFXDelay > timer) {
                timer += Time.deltaTime;
                yield return null;
            }

            if (bloodletting) {
                PlaySound(bloodletSFX);
            }

            yield return null;
        }
    }

    IEnumerator BloodletDecal() {
        while (bloodletting) {
            float delay = sprinting ? decalSprintDelay : decalWalkDelay;
            float timer = 0f;

            while (delay > timer) {
                timer += Time.deltaTime;
                yield return null;
            }

            if (bloodletting) {
// DECAL LOGIC
                GameObject decal = Instantiate(bloodDecal);
                decal.transform.position = transform.position;
            }

            yield return null;
        }
    }

    public IEnumerator RegainBlood() {
        bloodRegen = true;
// DELAY START OF BLOOD REGEN
        float regen = 0;
        while (!bloodletting && regen < bloodDelay) {
            regen += Time.deltaTime;
            if (bloodletting) {
                bloodRegen = false;
                break;
            }
            yield return null;
        }
// REGENERATE BLOOD
        while (!bloodletting && bloodRegen && bloodLevel < 100) {
            while (!tick) {
                if (bloodletting) {
                    bloodRegen = false;
                    break;
                }
                yield return null;
            }
            if (!bloodletting)
                bloodLevel += bloodRegenRate;
            yield return null;
        }
        bloodRegen = false;
    }

    public IEnumerator Crouch() {
        crouching = true;
        Vector3 startPos = cameraRoot.localPosition;
        float timer = 0;
        while (timer < crouchDur && Input.GetButton("Crouch")) {
            if (!Input.GetButton("Crouch")) break;
            timer += Time.deltaTime;
            cameraRoot.localPosition = Vector3.Lerp(startPos, new Vector3(cameraRoot.localPosition.x, cameraHeight.y, cameraRoot.localPosition.z), crouchCurve.Evaluate(timer/crouchDur));
            yield return null;
        }
        cameraRoot.localPosition = new Vector3(cameraRoot.localPosition.x, cameraHeight.y, cameraRoot.localPosition.z);
        while (Input.GetButton("Crouch")) {
            if (!Input.GetButton("Crouch")) break;
            yield return null;
        }   
        startPos = cameraRoot.localPosition;
        timer = 0;
        while (timer < crouchDur) {
            timer += Time.deltaTime;
            cameraRoot.localPosition = Vector3.Lerp(startPos, new Vector3(0, cameraHeight.x, 0), crouchCurve.Evaluate(timer/crouchDur));
            yield return null;
        }
        cameraRoot.localPosition = new Vector3(0, cameraHeight.x, 0);
        crouching = false;
    }

    IEnumerator Exposure() {
        while (true) {
            while (!tick) yield return null;
            exposureLevel = expBase;
            if (sprinting) exposureLevel += expSprint;
            else if (speedMultiplier > standMultiplier) exposureLevel += expWalk;
            else exposureLevel += expStill;
            if (crouching) exposureLevel += expCrouch;
            if (bloodletting) exposureLevel += expBloodletting;
            exposureLevel += expInfected * infectionLevel/100;
            exposureLevel = exposureCurve.Evaluate(exposureLevel/100) * 100;
            yield return null;
        }
    }

    public void Perish(Transform killer) {
        killCam.m_LookAt = killer;
        cutsceneDirector.Play();
        bloodLevel = 0f;
        alive = false;
        _moveSpeed = 0;
        walkSpeed = 0;
    }

    float freq = 0;
    public void Update() {
        if (GameManager.instance.gameState != GameManager.GameState.Menu && Input.GetButtonDown("Pause"))
            GameManager.instance.Pause(GameManager.instance.gameState == GameManager.GameState.Running ? true : false);
        if (alive && GameManager.instance.gameState == GameManager.GameState.Running) {
// MOUSE INPUT
// INTERACTION INPUT
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
// CLICKED ON AN INTERACTABLE
                if (Physics.Raycast(ray, out hit, 100f, interactionMask)) {
                    Interactable target = hit.collider.GetComponentInParent<Interactable>();
                    target.OnInteract();
                }
            }

// BLOODLET INPUT
            if (Input.GetButtonDown("Bloodlet")) 
                ToggleBloodletting(!bloodletting);
            if (bloodLevel < 100 && !bloodRegen && !bloodletting) 
                StartCoroutine(RegainBlood());

// CROUCH INPUT
            if (Input.GetButtonDown("Crouch") && !crouching) {
                StartCoroutine(Crouch());
            }

// MOVE INPUT
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");
            if (inputX != 0 || inputY != 0) {
                if (!sprinting) _speedMultiplier = 1f;
// SPRINT INPUT
                if (Input.GetButton("Run")) {
                    if (staminaLevel > 0 && !sprinting) StartCoroutine(Sprint());
                } else if (staminaLevel < 100 && !staminaRegen) StartCoroutine(RegainStamina());
                
            } else if (!sprinting) _speedMultiplier = standMultiplier;
        }
    }

    void LateUpdate() {
        freq += Time.deltaTime * Mathf.Clamp(_speedMultiplier, 0.5f, 2) * 5;
        armsRoot.position = new Vector3(armsRoot.position.x, cameraRoot.position.y + Mathf.Sin(freq) * 0.01f * _speedMultiplier, armsRoot.position.z);
        float rot = fpsCam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
        float rot2 = Mathf.Lerp(-15, 35, Mathf.InverseLerp(-70, 70, rot));
        rot = fpsCam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        float rot3 = Mathf.Lerp(180, 540, Mathf.InverseLerp(-180, 180, rot));
        armsRoot.rotation = Quaternion.Euler(rot2, rot3, 0);
    }

    IEnumerator FootstepCheck()
    {
        while (true) {
            float delay = sprinting ? footstepRunDelay/_speedMultiplier  : footstepDelay;
            float timer = 0f;
            
            while (delay > timer) {
                timer += Time.deltaTime;
                yield return null;
            }
            if (sprinting) {
                PlaySound(footstepRunSFX);
            }
            else if(_speedMultiplier != standMultiplier) {
                PlaySound(footstepWalkSFX);
            }
            yield return null;
        }
    }

    public virtual void PlaySound(SFX sfx = null) {
        if (sfx) {
            if (sfx.outputMixerGroup) 
                oneShotAudioSource.outputAudioMixerGroup = sfx.outputMixerGroup;   
            
                oneShotAudioSource.PlayOneShot(sfx.Get());
        }
    }

    void OnDrawGizmos () {
		Gizmos.color = Color.yellow;
        Vector3 coneOffset = new Vector3(0, 1.5f, 0);
        if (Application.isPlaying)
            Gizmos.color = fovCone.detecting ? Color.green : fovCone.inRange ? Color.yellow : Color.red;;
        Gizmos.DrawRay(transform.position + coneOffset, Quaternion.AngleAxis(fovCone.viewAngle/2, Vector3.up) * transform.forward * fovCone.dist);
        Gizmos.DrawRay(transform.position + coneOffset, Quaternion.AngleAxis(-fovCone.viewAngle/2, Vector3.up) * transform.forward * fovCone.dist);
    }

    void OnApplicationQuit() {
        ResetShaders();
    }

    public void ResetShaders() {
        Debug.Log("reset shaders");
        foreach (FullscreenEffect fx in infectionEffects) {
            foreach (EffectProperty prop in fx.properties) {
                fx.material.SetFloat(prop.shaderProperty, prop.range.x);
            }
        }
        foreach (EffectProperty prop in bloodEffect.properties) 
            bloodEffect.material.SetFloat(prop.shaderProperty, prop.range.x);

        fpsCam.m_Lens.FieldOfView = baseFOV;
        fpsCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = baseFOVAmplitude;
        fpsCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 1;
    }
}
