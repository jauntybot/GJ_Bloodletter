using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading;
using UnityEngine.Rendering.Universal;
using UnityEditor.UIElements;

[RequireComponent(typeof(AudioSource))]
public class Bloodletter : MonoBehaviour {


    public static Bloodletter instance;
    void Awake() {
        if (Bloodletter.instance) return;
        Bloodletter.instance = this;
    }

    AudioSource audioSource;


    [Header("Controller")]	
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] Transform cameraRoot;
    [SerializeField] Vector2 cameraHeight = new Vector2(0.5f, -0.5f);
    [SerializeField] AnimationCurve crouchCurve;
    [SerializeField] float crouchDur;
    public float walkSpeed;
    public float standMultiplier, sprintMultiplier;
    private float _moveSpeed;
    public float moveSpeed {
        get { return _moveSpeed; }
    }
    private float _speedMultiplier;
    [SerializeField] public float speedMultiplier {
        get { return _speedMultiplier; }
    }

    [SerializeField] float staminaDrainRate, staminaDelay, staminaRegenRate, sprintDelay, bloodDrainRate, bloodDelay, bloodRegenRate;

    [SerializeField] float tickDur;
    float curTick;
    [HideInInspector] public bool tick;
    
	public Camera cam;
	public LayerMask interactionMask;

    [Header("Rates")]
    [SerializeField] float bloodletSFXDelay;
    [SerializeField] float decalSprintDelay, decalWalkDelay, footstepDelay, footstepRunDelay, heavyBreathingDelay;
    
    
    [Header("Exposure")]
    [Range(0,100)]
    public float exposureLevel;
    [SerializeField] AnimationCurve exposureCurve;
    [SerializeField] float expBase, expStill, expCrouch, expWalk, expSprint, expBloodletting, expInfected;

    [Header("Cinemachine Values")]
    [SerializeField] float baseFOV;
    [SerializeField] float sprintFOVMod, bloodletFOVMod, baseFOVAmplitude, bloodletFOVAmplitude;
    [SerializeField] float FOV() {
        float fov = baseFOV;
        if (sprinting) fov += sprintFOVMod;
        if (bloodletting) fov += bloodletFOVMod;
        return fov;
    }

    [Header("Stats")]
    [Range(0,100)]
    public float bloodLevel;
    [Range(0,100)]
    public float infectionLevel, staminaLevel;
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
    [SerializeField] SFX bloodletSFX, footstepWalkSFX, footstepRunSFX, heavyBreathingSFX;

    void Start() {
        Init();
    }

    public void Init() {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(InfectionSpread());
        StartCoroutine(BloodletterTick());
        StartCoroutine(Exposure());
        StartCoroutine(FootstepCheck());
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

            bloodEffect.material.SetFloat(bloodEffect.properties[0].shaderProperty, bloodEffect.properties[0].range.x + bloodEffect.properties[0].curve.Evaluate(1 - (bloodLevel/100)) * (bloodEffect.properties[0].range.y - bloodEffect.properties[0].range.x));
            
            yield return null;
        }

    }

    public IEnumerator InfectionSpread() {
        while (true) {
            while (!tick) yield return null;
            infectionSpeed = bloodLevel/100;



            if (!bloodletting) {
                if (infectionPotency < potencyMax)
                    infectionPotency += potencyIncrement;
                if (infectionLevel < 100)
                    infectionLevel += infectionPotency * infectionSpeed;
            }
            yield return null;
        }
    }

    public IEnumerator Sprint() {
        sprinting = true;
// RAMP UP TO SPRINT SPEED AND FOV LERP
        float timer = 0;
        float curFOV = vCam.m_Lens.FieldOfView;
        while (timer < sprintDelay) {
            timer += Time.deltaTime;
            vCam.m_Lens.FieldOfView = Mathf.Lerp(curFOV, FOV(), timer/sprintDelay);
            _speedMultiplier = Mathf.Lerp(1, sprintMultiplier, timer / sprintDelay);
            if (!Input.GetButton("Run")) {
                sprinting = false;
                break;
            }
            yield return null;
        }
        vCam.m_Lens.FieldOfView = FOV();
        _speedMultiplier = sprintMultiplier;

// WHILE LOOP OF SPRINTING
        while (staminaLevel > 0 && sprinting) {
            while (!tick) {
                if (!Input.GetButton("Run")) {
                    sprinting = false;
                    break;
                }
                yield return null;
            }
            if (sprinting)
                staminaLevel -= staminaDrainRate;
            if (staminaLevel <= 25 && !heavyBreathing) {
                StartCoroutine(HeavyBreathing());
            }

            yield return null;
        }
        _speedMultiplier = 1f;
        sprinting = false;

// LERP FOV BACK
        timer = 0;
        float endFOV = vCam.m_Lens.FieldOfView;
        while (timer < sprintDelay) {
            timer += Time.deltaTime;
            vCam.m_Lens.FieldOfView = Mathf.Lerp(endFOV, FOV(), timer/sprintDelay);
            yield return null;
        }
        vCam.m_Lens.FieldOfView = FOV();
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
        while (staminaRegen && staminaLevel < 100) {
            while (!tick) {
                if (Input.GetButton("Run")) {
                    staminaRegen = false;
                    break;
                }
                yield return null;
            }
            if (staminaRegen)
                staminaLevel += staminaRegenRate;

            yield return null;
        }
        staminaRegen = false;
    }

    IEnumerator HeavyBreathing() {
        heavyBreathing = true;
        PlaySound(heavyBreathingSFX);
        while (staminaLevel <= 25) {
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
            StartCoroutine(BloodletLerpFOV(bloodletFOVAmplitude));
        } else {
            bloodletting = false;
            StartCoroutine(BloodletLerpFOV(baseFOVAmplitude));
        }
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
        StartCoroutine(BloodletLerpFOV(baseFOVAmplitude));
    }

    IEnumerator BloodletLerpFOV(float target) {
        float startAmp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain;
        float timer = 0;
        while (timer < bloodDelay) {
            timer += Time.deltaTime;
            vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Mathf.Lerp(startAmp, target, timer/bloodDelay);

            yield return null;
        }
        vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = target;
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

    public void Update() {
// MOUSE INPUT
// INTERACTION INPUT
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
// CLICKED ON AN INTERACTABLE
			if (Physics.Raycast(ray, out hit, 100f, interactionMask)) {
                Debug.Log(hit.collider.gameObject.name);
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
            
            _moveSpeed = walkSpeed * _speedMultiplier;
        } else if (!sprinting) _speedMultiplier = standMultiplier;
    }

    IEnumerator FootstepCheck()
    {
        while (true) {
            float delay = sprinting ? footstepRunDelay : footstepDelay;
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

    public virtual void PlaySound(SFX sfx = null, bool loop = false) {
        audioSource.loop = loop;
        if (sfx) {
            if (sfx.outputMixerGroup) 
                audioSource.outputAudioMixerGroup = sfx.outputMixerGroup;   

            if(!loop)
                audioSource.PlayOneShot(sfx.Get());
            else
            {
                audioSource.clip = sfx.Get();
                audioSource.Play();
                
            }
        }
    }
}
