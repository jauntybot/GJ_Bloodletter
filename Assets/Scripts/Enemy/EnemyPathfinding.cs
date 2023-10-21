using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.VFX;

[RequireComponent(typeof(EnemyDirector))]
[RequireComponent(typeof(KiwiBT.BehaviourTreeRunner))]
public class EnemyPathfinding : MonoBehaviour {

    public static EnemyPathfinding instance;
    void Awake() {
        if (EnemyPathfinding.instance) Destroy(EnemyPathfinding.instance.gameObject);
        EnemyPathfinding.instance = this;
    }

    public EnemyDirector director;
    NavMeshAgent agent;
    [HideInInspector] public Bloodletter bloodletter;
    KiwiBT.BehaviourTreeRunner btRunner;
    
    [Header("References")]
    [SerializeField] AudioSource audioSource, sfxSource;
    [SerializeField] List<GameObject> gfx;
    [SerializeField] SFX idleSFX, chaseStingSFX, killStingSFX;
    [SerializeField] Material larvaMat;

    public enum EnemyState { Lurking, Roaming, Ambling, Tracking, Chasing };
    [Header("State Machine")]
    public EnemyState state;
    public bool hidden;
    [SerializeField] float visibleTerror;
    [SerializeField] float hideDur;


    [Header("Detection Variables")] [Range(0,100)]
    public float detectionLevel;
    public bool detecting;
    public float detectionDelta;
    public List<DetectionCone> detectionCones;
    [SerializeField] float detectionDrainRate;


    
    [Header("Nav Variables")] 
    public Vector2 speedRange;
    [Range(0,100)] public float energyLevel;
    public float energyRegenRate, energyRegenDelay, energyDrainRate;
    public List<BloodPool> bloodPools = new List<BloodPool>();
    public BloodPool currentPool;
    public LayerMask bloodPoolMask;
    

    [Header("Kill Variables")]
    public float killRadius;
    public bool attacking;



    void Start() {
        agent = GetComponent<NavMeshAgent>();
        //audioSource = GetComponent<AudioSource>();
        bloodletter = Bloodletter.instance;
        director = GetComponent<EnemyDirector>();

        btRunner = GetComponent<KiwiBT.BehaviourTreeRunner>();
        btRunner.Init();

        ChangeState(EnemyState.Lurking);
        StartCoroutine(PassiveDetection());
    }

    public void ChangeState(EnemyState _state) {
        if (director.downtimeCo != null)
            director.StopCoroutine(director.downtimeCo);
        switch (_state) {
            default:
            case EnemyState.Lurking:
                if (state == EnemyState.Lurking)
                    director.hostilityLevel += 10f;
                director.downtimeThreshold = Random.Range(10f, 30f);
            break;
            case EnemyState.Roaming:
                director.downtimeThreshold = Random.Range(30f, 60f);
            break;
        }
        state = _state;
        director.downtimeCo = director.StartCoroutine(director.Downtime());
    }


    public IEnumerator PassiveDetection() {
        StartCoroutine(DetectionDelta());
        float delta = 0;
        while (true) {
// INCREMENT DETECTION LEVEL
            detecting = false;
            if (state != EnemyState.Lurking) {
                foreach (DetectionCone cone in detectionCones) {
                    if (Vector3.Distance(transform.position, bloodletter.transform.position) < cone.dist) {
                        Vector3 dir = (bloodletter.transform.position - transform.position).normalized;
                        if (cone.coneShape == DetectionCone.ConeShape.Sphere) {
                            if (!Physics.Linecast(transform.position, bloodletter.transform.position, cone.viewMask)) {
                                delta = bloodletter.exposureLevel/1000 * cone.detectionMultiplier;
                                if (delta >= cone.deltaThreshold) {
                                    if (detectionLevel < 100)
                                        detectionLevel += delta;
                                    if (!detecting) {
                                        detecting = true;
                                        director.downtimeTimer -= 5;
                                        director.downtimeTimer = Mathf.Clamp(director.downtimeTimer, 0, 100);
                                    }
                                    cone.detecting = true;
                                }
                            } else {
                                cone.inRange = false;
                                cone.detecting = false;
                            }
                        } else {
                            float angleDelta = Vector3.Angle(transform.forward, dir);
                            if (angleDelta < cone.viewAngle / 2f) {
                                if (!Physics.Linecast(transform.position, bloodletter.transform.position, cone.viewMask)) {
                                    delta = bloodletter.exposureLevel/1000 * cone.detectionMultiplier;
                                    if (delta >= cone.deltaThreshold) {
                                        if (detectionLevel < 100)
                                            detectionLevel += delta;
                                        if (!detecting) {
                                            detecting = true;
                                            director.downtimeTimer -= 5;
                                            director.downtimeTimer = Mathf.Clamp(director.downtimeTimer, 0, 100);
                                        }
                                        cone.detecting = true;
                                    }
                                } else cone.detecting = false;
                                cone.inRange = true;
                            } 
                            else {
                                cone.inRange = false;
                                cone.detecting = false;
                            }
                        }
                    }
                    else {
                            cone.inRange = false;
                            cone.detecting = false;
                        }
                }    
            }

            if (!detecting && detectionLevel > 0) 
                detectionLevel -= detectionDrainRate;
    
            larvaMat.SetFloat("_VertexResolution", Mathf.Lerp(6, 32,Mathf.InverseLerp(0, 60, bloodletter.enemyTerror)));

            yield return null;
        }
    }

    public IEnumerator DetectionDelta() {
        float timer;
        float prevDetection;
        while (true) {
            timer = 0;
            prevDetection = detectionLevel;
            while (timer < 0.5f) {
                timer += Time.deltaTime;
                yield return null;
            }
            detectionDelta = detectionLevel - prevDetection;
            yield return null;
        }
    }

    public float CalculateSpeed() {
        float _speed;
        _speed = Mathf.Lerp(speedRange.x, speedRange.y, Mathf.InverseLerp(0, 100, director.hostilityLevel));

        
        return _speed;
    }

    bool hiding;
    public void ToggleVisibility(bool state) {
        if (!hiding)
            StartCoroutine(HideAnimation(state));
    }

    public IEnumerator HideAnimation(bool state) {
        hiding = true;
        float fromVol = 1;
        float toVol = 0;
        if (state) {
            foreach(GameObject obj in gfx) 
                obj.SetActive(state);
            audioSource.clip = idleSFX.Get();
            //audioSource.Play();
            audioSource.volume = 0;
            fromVol = 0;
            toVol = 1;
        } else
            agent.enabled = state;
            
        float timer = 0;
        Vector3 startPos = transform.position, targetPos = transform.position + new Vector3(0, 3, 0) * (state ? 1 : -1);
        while (timer < hideDur) {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, timer/hideDur);
            audioSource.volume = Mathf.Lerp(fromVol, toVol, timer/hideDur);
            yield return null;
        }
        transform.position = targetPos;
        yield return null;
        hidden = !state;
        
        if (!state) {
            foreach(GameObject obj in gfx) 
                obj.SetActive(state);
            audioSource.Stop();   
        } else {
            agent.enabled = state;
            StartCoroutine(DampenAudio());
        }
        hiding = false;
    }


    public IEnumerator TerrorizePlayer() {
        attacking = true;
        PlaySound(killStingSFX);
        bloodletter.enemyTerror += 25;
        if (bloodletter.enemyTerror > 60) bloodletter.enemyTerror = 60;

        float timer = 0f;
        while (timer < 1f) {
            timer += Time.deltaTime;
            yield return null;
        }
        ChangeState(EnemyState.Lurking);
        yield return null;
        attacking = false;
    }

    public IEnumerator KillPlayer() {
        attacking = true;
        GameManager.instance.KillPlayer();
        bloodletter.Perish(transform);
        PlaySound(killStingSFX);

        yield return null;
        attacking = false;
    }


    void OnDrawGizmos () {
		Gizmos.color = Color.yellow;
        Vector3 coneOffset = new Vector3(0, 1.5f, 0);
        foreach (DetectionCone cone in detectionCones) {
            if (Application.isPlaying)
                Gizmos.color = cone.detecting ? Color.green : cone.inRange ? Color.yellow : Color.red;;
            if (cone.coneShape == DetectionCone.ConeShape.Cone) {
                Gizmos.DrawRay(transform.position + coneOffset, Quaternion.AngleAxis(cone.viewAngle/2, Vector3.up) * transform.forward * cone.dist);
                Gizmos.DrawRay(transform.position + coneOffset, Quaternion.AngleAxis(-cone.viewAngle/2, Vector3.up) * transform.forward * cone.dist);
            } else 
        		Gizmos.DrawWireSphere(transform.position, cone.dist);

        }
    }

    [SerializeField] AudioMixer mixer;
    bool dampen;
    public IEnumerator DampenAudio() {
        dampen = true;
        while (!hidden) {
            float dist = Vector3.Distance(transform.position, bloodletter.transform.position);
            if (dist < detectionCones[2].dist) {
                    mixer.SetFloat("EnvirVol", Mathf.Log10(0.001f + bloodletter.terrorProximity.Evaluate(dist/detectionCones[2].dist)) * 20);
                    mixer.SetFloat("PlayerVol", Mathf.Log10(0.001f + bloodletter.terrorProximity.Evaluate(dist/detectionCones[2].dist)) * 20);
            } else {
                mixer.SetFloat("EnvirVol", Mathf.Log10(1) * 20);
                mixer.SetFloat("PlayerVol", Mathf.Log10(1) * 20);
            }



            yield return null;
        }
        mixer.SetFloat("EnvirVol", Mathf.Log10(1) * 20);
        mixer.SetFloat("PlayerVol", Mathf.Log10(1) * 20);
        dampen = false;

    }

    public virtual void PlaySound(SFX sfx = null, bool loop = false) {
        sfxSource.loop = loop;
        if (sfx) {
            if (sfx.outputMixerGroup) 
                sfxSource.outputAudioMixerGroup = sfx.outputMixerGroup;   

            if(!loop)
                sfxSource.PlayOneShot(sfx.Get());
            else
            {
                sfxSource.clip = sfx.Get();
                sfxSource.Play();
                
            }
        }
    }
}

[System.Serializable]
public class DetectionCone {

    public string name;

    [Header("Cone Properties")]
    public float dist;
    public float viewAngle, detectionMultiplier, deltaThreshold;
    public enum ConeShape { Cone, Sphere };
    public ConeShape coneShape;
    public LayerMask viewMask;
    public bool detecting, inRange;




}