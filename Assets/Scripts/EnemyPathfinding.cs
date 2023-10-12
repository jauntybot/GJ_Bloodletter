using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

[RequireComponent(typeof(AudioSource))]
public class EnemyPathfinding : MonoBehaviour {

    [Header("References")]
    [HideInInspector]
    public EnemyDirector director;
    PlayableDirector cutsceneDirector;
    NavMeshAgent agent;
    [HideInInspector]
    public Bloodletter bloodletter;
    [SerializeField] AudioSource audioSource, sfxSource;
    [SerializeField] SFX idleSFX, chaseStingSFX, killStingSFX;

    public enum EnemyState { Lurking, Roaming, Ambling, Tracking, Chasing };
    [Header("State Machine")]
    public EnemyState state;


    [Header("Detection Variables")] [Range(0,100)]
    public float detectionLevel;
    public float detectionDelta;
    public List<DetectionCone> detectionCones;
    [SerializeField] LayerMask viewMask;
    [SerializeField] float detectionDrainRate;


    
    [Header("Nav Variables")] [Range(0,100)]
    public float energyLevel;
    public float energyRegenRate, energyRegenDelay, energyDrainRate;
    [SerializeField] Transform pointOfInterest;
    [SerializeField] BloodPool currentPool;
    [SerializeField] LayerMask bloodPoolMask;
    

    [Header("Kill Variables")]
    public float killRadius;



    void Start() {
        agent = GetComponent<NavMeshAgent>();
        //audioSource = GetComponent<AudioSource>();
        bloodletter = Bloodletter.instance;
        director = EnemyDirector.instance;
        cutsceneDirector = GetComponent<PlayableDirector>();
        
        // StartCoroutine(Pathfind());
        // StartCoroutine(LookAt());
        StartCoroutine(PassiveDetection());

        audioSource.clip = idleSFX.Get();
        audioSource.Play();
    }


    public IEnumerator PassiveDetection() {
        StartCoroutine(DetectionDelta());
        while (true) {
// INCREMENT DETECTION LEVEL
            bool detecting = false;
            foreach (DetectionCone cone in detectionCones) {
                if (Vector3.Distance(transform.position, bloodletter.transform.position) < cone.dist) {
                    Vector3 dir = (bloodletter.transform.position - transform.position).normalized;
                    if (cone.coneShape == DetectionCone.ConeShape.Sphere) {
                        if (!Physics.Linecast(transform.position, bloodletter.transform.position, viewMask)) {
                            if (detectionLevel < 100)
                                detectionLevel += bloodletter.exposureLevel/1000 * cone.detectionMultiplier;
                            detecting = true;
                            cone.detecting = true;
                        } else {
                            cone.inRange = false;
                            cone.detecting = false;
                        }
                    } else {
                        float angleDelta = Vector3.Angle(transform.forward, dir);
                        if (angleDelta < cone.viewAngle / 2f) {
                            if (!Physics.Linecast(transform.position, bloodletter.transform.position, viewMask)) {
                                if (detectionLevel < 100)
                                    detectionLevel += bloodletter.exposureLevel/1000 * cone.detectionMultiplier;
                                detecting = true;
                                cone.detecting = true;
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

            if (!detecting && detectionLevel > 0) 
                detectionLevel -= detectionDrainRate;
    
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

    IEnumerator AmbleToPOI(Vector3 pos) {
        NavMeshHit hit;
        
        NavMesh.SamplePosition(pos, out hit, 1.0f, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
        yield return null;

        if (agent.hasPath) {
            Debug.Log(agent.path.corners.Length);
            agent.SetDestination(agent.path.corners.Length > 5 ? agent.path.corners[4] : agent.path.corners[agent.path.corners.Length - 1]);
        }
// WAIT FOR PATH TO FINISH
        bool finished = false;
        if (agent.hasPath) {
            float distance = agent.remainingDistance;
            while (!finished) {
                if (!agent.pathPending) {
                    if (agent.remainingDistance <= distance - Random.Range(2, 10) || agent.remainingDistance <= agent.stoppingDistance) {
                        finished = true;
                    }
                }
                yield return null;
            }
        }
    }


    IEnumerator ScanArea() {
        //scanning = true;
        for (int i = 0; i <= 6; i++) {
            print("new dir");
            float timer = 0;
            float rnd = Random.Range(20, 100);
            print(rnd);
            if (Random.Range(0, 1) != 0) rnd = -rnd;
            Quaternion rot = Quaternion.AngleAxis(rnd, Vector3.up);
            while (timer < 1f) {
                timer += Time.deltaTime;
                transform.rotation = rot;
                yield return null;
            }    
        }
        yield return null;


        //scanning = false;
    }

    IEnumerator TrackBlood() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionCones[2].dist, bloodPoolMask);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.GetComponent<BloodPool>()) {
                BloodPool bp = hitCollider.GetComponent<BloodPool>();
                if (currentPool == null || bp.age < currentPool.age) {
                    currentPool = bp;
                }
            }
        }
        if (currentPool)
            agent.SetDestination(currentPool.transform.position);
        else {
            state = EnemyState.Roaming;
            yield return null;
        }

// WAIT FOR PATH TO FINISH
        bool finished = false;
        if (agent.hasPath) {
            while (!finished) {
                if (!agent.pathPending) {
                    if (agent.remainingDistance <= agent.stoppingDistance) {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                            finished = true;
                        }
                    }
                }
                yield return null;
            }
        }

// DELAY TO INSPECT BLOOD POOL
        if (currentPool)
            yield return new WaitForSecondsRealtime(2.5f);
    }

    public IEnumerator KillPlayer() {
        GameManager.instance.KillPlayer();
        
        cutsceneDirector.Play();
        bloodletter.bloodLevel = 0f;
        PlaySound(killStingSFX);

        yield return null;
    }


    void OnDrawGizmos () {
		Gizmos.color = Color.yellow;
        foreach (DetectionCone cone in detectionCones) {
            if (Application.isPlaying)
                Gizmos.color = cone.detecting ? Color.green : cone.inRange ? Color.yellow : Color.red;;
            if (cone.coneShape == DetectionCone.ConeShape.Cone) {
                Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(cone.viewAngle/2, Vector3.up) * transform.forward * cone.dist);
                Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(-cone.viewAngle/2, Vector3.up) * transform.forward * cone.dist);
            } else 
        		Gizmos.DrawWireSphere(transform.position, cone.dist);

        }
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
    public float viewAngle, detectionMultiplier;
    public enum ConeShape { Cone, Sphere };
    public ConeShape coneShape;

    public bool detecting, inRange;




}