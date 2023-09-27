using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodletter : MonoBehaviour {


// INTERACTION VARIABLES
    [Header("Controller")]	
	public Camera cam;
	public LayerMask interactionMask;

    [Header("Stats")]
    public float walkSpeed, moveSpeed;
    public float speedMultiplier;
    [Range(0,100)]
    public float bloodLevel;
    [Range(0,100)]
    public float infectionLevel, staminaLevel;
    public float infectionPotency, infectionSpeed;
    [SerializeField]
    bool sprinting, staminaRegen, bloodletting, bloodRegen;

    [SerializeField]
    float curTick, tickDur;
    [SerializeField]
    bool tick;
    
    [Header("Prefabs")]
    [SerializeField] GameObject bloodDecal;

    void Start() {
        Init();
    }

    public void Init() {
        StartCoroutine(InfectionSpread());
        StartCoroutine(BloodletterTick());
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


            infectionLevel += infectionPotency * infectionSpeed;
            yield return null;
        }
    }

    public IEnumerator Sprint() {
        sprinting = true;
        speedMultiplier = 2f;
        while (staminaLevel > 0 && sprinting) {
            while (!tick) {
                if (!Input.GetButton("Run")) {
                    staminaRegen = false;
                    break;
                }
                yield return null;
            }
            staminaLevel -= 5f;
            if (!Input.GetButton("Run")) {
                    staminaRegen = false;
                    break;
            }
            yield return null;
        }
        speedMultiplier = 1f;
        sprinting = false;
    }

    public IEnumerator RegainStamina() {
        staminaRegen = true;
// DELAY START OF STAMINA REGEN
        float regenDelay = 3f;
        float regen = 0;
        while (regen < regenDelay) {
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
            staminaLevel += 3;
            yield return null;
        }
        staminaRegen = false;
    }

    public IEnumerator Bloodlet() {
        bloodletting = true;
        while (bloodletting && bloodLevel > 0) {
            while (!tick) {
                if (!Input.GetButton("Bloodlet")) {
                    bloodletting = false;
                    break;
                }
                yield return null;
            } 
            bloodLevel -= 2.2f * speedMultiplier;
// DECAL LOGIC
            GameObject decal = Instantiate(bloodDecal);
            decal.transform.position = transform.position;

            yield return null;
        }
        bloodletting = false;
    }

    public IEnumerator RegainBlood() {
        bloodRegen = true;
// DELAY START OF STAMINA REGEN
        float regenDelay = 3f;
        float regen = 0;
        while (regen < regenDelay) {
            regen += Time.deltaTime;
            if (Input.GetButton("Bloodlet")) {
                bloodRegen = false;
                break;
            }
            yield return null;
        }
// REGENERATE STAMINA
        while (bloodRegen && bloodLevel < 100) {
            while (!tick) {
                if (Input.GetButton("Bloodlet")) {
                    bloodRegen = false;
                    break;
                }
                yield return null;
            }
            bloodLevel += 0.5f;
            yield return null;
        }
        bloodRegen = false;
    }

    
    public IEnumerator TransfuseBlood(TransfusionSite site) {
        site.transfusing = true;
        while (site.bloodContent > 0 && site.attached) {
            while (!tick) yield return null;
            Debug.Log("Transfusing");
            bloodLevel += 1.5f;
            site.bloodContent -= 1.5f;
            if (!site.attached) {
                site.transfusing = false;
                break;
            }
            yield return null;
        }
// USED ALL BLOOD
        if (site.bloodContent <= 0) {
            
        }
        site.transfusing = false;
    }

    public void FixedUpdate() {
// MOUSE INPUT
// INTERACTION INPUT
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
// CLICKED ON AN INTERACTABLE
			if (Physics.Raycast(ray, out hit, 100f, interactionMask)) {
				Interactable target = hit.collider.GetComponent<Interactable>();
				if (Vector3.Distance(target.transform.position, transform.position) <= target.interactRadius) {
					target.OnInteract(transform);
				}
			}
		}

// MOVE INPUT
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        if (inputX != 0 || inputY != 0) {
            if (!sprinting) speedMultiplier = 1f;
// SPRINT INPUT
            if (Input.GetButton("Run")) {
                if (staminaLevel > 0 && !sprinting) StartCoroutine(Sprint());
            } else if (staminaLevel < 100 && !staminaRegen) StartCoroutine(RegainStamina());
            
            moveSpeed = walkSpeed * speedMultiplier;
        } else if (!sprinting) speedMultiplier = 0.5f;

// BLOODLET INPUT
        if (Input.GetButton("Bloodlet")) {
            if (bloodLevel > 0 && !bloodletting) StartCoroutine(Bloodlet());
        } else if (bloodLevel < 100 && !bloodRegen) StartCoroutine(RegainBlood());

    }
}
