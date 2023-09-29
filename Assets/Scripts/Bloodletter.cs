using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bloodletter : MonoBehaviour {


    public static Bloodletter instance;
    void Awake() {
        if (Bloodletter.instance) return;
        Bloodletter.instance = this;
    }

    AudioSource audioSource;

// INTERACTION VARIABLES
    [Header("Controller")]	
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
    

    [Header("Stats")]
    [Range(0,100)]
    public float bloodLevel;
    [Range(0,100)]
    public float infectionLevel, staminaLevel;
    public float infectionPotency, infectionSpeed, potencyIncrement;


    [Header("States")]
    [SerializeField]
    bool sprinting;
    [SerializeField]
    bool staminaRegen, heavyBreathing, bloodletting, bloodRegen;

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
        StartCoroutine((FootstepCheck()));
    }

    IEnumerator BloodletterTick() {
        while (true) {
            tick = false;
            curTick += Time.deltaTime;
            if (curTick > tickDur) {
                curTick = 0;
                tick = true;
            }
            yield return null;
        }

    }

    public IEnumerator InfectionSpread() {
        while (infectionLevel < 100) {
            while (!tick) yield return null;
            infectionSpeed = bloodLevel/100;

            infectionPotency += potencyIncrement;
            infectionLevel += infectionPotency * infectionSpeed;
            yield return null;
        }
    }

    public IEnumerator Sprint() {
        sprinting = true;
        float ramp = 0;
        while (ramp < sprintDelay) {
            ramp += Time.deltaTime;
            _speedMultiplier = Mathf.Lerp(1, sprintMultiplier, ramp / sprintDelay);
            if (!Input.GetButton("Run")) {
                sprinting = false;
                break;
            }
            yield return null;
        }
        _speedMultiplier = sprintMultiplier;
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
        } else bloodletting = false;
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
        while (bloodletting)
        {
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
