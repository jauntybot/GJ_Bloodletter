using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class EnemyPathfinding : MonoBehaviour {

    [Header("References")]
    EnemyDirector director;
    NavMeshAgent agent;
    Bloodletter bloodletter;
    [SerializeField] AudioSource audioSource, sfxSource;
    [SerializeField] SFX idleSFX, chaseStingSFX;

    public enum EnemyState { Lurking, Roaming, Ambling, Tracking, Chasing };
    [Header("State Machine")]
    public EnemyState state;
    [SerializeField] float stateChangeNRGReq;

    public enum EnemyLookDir { Static, Search, Lock };
    public EnemyLookDir lookDir;
    [Range(0,100)]
    public float energyLevel;

    public List<EnemyBehavior> behaviors;



    [Header("Detection Variables")] [Range(0,100)]
    public float detectionLevel;
    [SerializeField] List<DetectionCone> detectionCones;
    [SerializeField] LayerMask viewMask;
    [SerializeField] float detectionDrainRate;
    public bool playerLock;
    [SerializeField] bool scanning;


    [Header("Nav Variables")]
    [SerializeField] Transform pointOfInterest;
    [SerializeField] BloodPool currentPool;
    [SerializeField] LayerMask bloodPoolMask;
    bool seenByPlayer;

    [Header("ROAM VARS")]
    [SerializeField] Vector2 roamDist;
    [SerializeField] float roamDur;
    [SerializeField] float roamNRGReq;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        //audioSource = GetComponent<AudioSource>();
        bloodletter = Bloodletter.instance;
        director = EnemyDirector.instance;
        
        StartCoroutine(Pathfind());
        StartCoroutine(LookAt());
        StartCoroutine(PassiveDetection());

        audioSource.clip = idleSFX.Get();
        audioSource.Play();
    }

    public IEnumerator Pathfind() {
        while (true) {
            switch (state) {
                case EnemyState.Lurking:
                    yield return StartCoroutine(Lurk());
                break;
                case EnemyState.Roaming:
                    yield return StartCoroutine(RandomRoam());
                break;
                case EnemyState.Tracking:
                    yield return StartCoroutine(TrackBlood());
                break;
                case EnemyState.Chasing:
                    yield return StartCoroutine(Chase());
                break;
            }

            yield return null;
        }
    }

    public IEnumerator LookAt() {
        while (true) {
            switch(lookDir) {
                case EnemyLookDir.Static:
                    agent.angularSpeed = 120f;
                break;
                case EnemyLookDir.Search:
                    agent.angularSpeed = 0f;
                    if (!scanning) yield return StartCoroutine(ScanArea());
                break;
                case EnemyLookDir.Lock:
                    agent.angularSpeed = 0f;
                break;
            }

            yield return null;
        }

    }

    public IEnumerator PassiveDetection() {
        while (true) {
// INCREMENT DETECTION LEVEL
            bool detecting = false;
            foreach (DetectionCone cone in detectionCones) {
                if (Vector3.Distance(transform.position, bloodletter.transform.position) < cone.dist) {
                    Vector3 dir = (bloodletter.transform.position - transform.position).normalized;
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
                else {
                        cone.inRange = false;
                        cone.detecting = false;
                    }
            }    

            if (!detecting && detectionLevel > 0) 
                detectionLevel -= detectionDrainRate;
    
// CHANGE BEHAVIOR STATE BOOLS BY DETECTION LEVEL
        // if ()
            
            yield return null;
        }
    }

    IEnumerator Idle() {
        agent.ResetPath();
        yield return null;
    }


    IEnumerator Lurk() {
        agent.ResetPath();
        yield return null;

    }

    IEnumerator Teleport() {
        agent.ResetPath();
        yield return null;
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
        scanning = true;
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


        scanning = false;
    }

    IEnumerator RandomRoam() {
        float timer = 0;
        while (timer <= roamDur) {
            Vector3 pos = Quaternion.AngleAxis(Random.Range(-detectionCones[0].viewAngle, detectionCones[0].viewAngle)/2, Vector3.up) * transform.forward * Random.Range(roamDist.x, roamDist.y);
            agent.SetDestination(transform.position + pos);


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
                    timer += Time.deltaTime;
                    yield return null;
                }
            }

            energyLevel -= roamNRGReq;

            yield return null;
        }
        yield return StartCoroutine(AmbleToPOI(pointOfInterest.position));
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

    IEnumerator Chase() {
        agent.SetDestination(bloodletter.transform.position);
        
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
        //if (!CanSeePlayer()) state = EnemyState.Tracking;
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