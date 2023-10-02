using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class EnemyPathfinding : MonoBehaviour {

    [Header("References")]
    NavMeshAgent agent;
    Bloodletter bloodletter;
    [SerializeField] AudioSource audioSource, sfxSource;

    public enum EnemyState { Lurking, Roaming, Tracking, Chasing };
    [Header("State Machine")]
    public EnemyState state;
    [SerializeField] SFX idleSFX, chaseStingSFX;


    [Header("Tracking Vars")]
    [SerializeField] float viewAngle;
    [SerializeField] float viewDist, trackingRadius;
    [SerializeField] Vector2 roamDist;
    [SerializeField] Vector3 pointOfInterest;
    [SerializeField] LayerMask viewMask;
    [SerializeField] BloodPool currentPool;
    [SerializeField] LayerMask bloodPoolMask;
    bool seenByPlayer;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        //audioSource = GetComponent<AudioSource>();
        bloodletter = Bloodletter.instance;
        
        StartCoroutine(Pathfind());

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

    bool CanSeePlayer() {
        if (Vector3.Distance(transform.position, bloodletter.transform.position) < viewDist) {
            Vector3 dir = (bloodletter.transform.position - transform.position).normalized;
            float angleDelta = Vector3.Angle(transform.forward, dir);
            if (angleDelta < viewAngle /2f) {
                if (!Physics.Linecast(transform.position, bloodletter.transform.position, viewMask)) {
                    return true;
                }
            } 
        }
        return false;
    }

    bool SeenByPlayer() {
        if (Vector3.Distance(transform.position, bloodletter.transform.position) < viewDist) {
            Vector3 dir = (bloodletter.transform.position - transform.position).normalized;
            float angleDelta = Vector3.Angle(transform.forward, dir);
            if (angleDelta < viewAngle /2f) {
                if (!Physics.Linecast(transform.position, bloodletter.transform.position, viewMask)) {
                    return true;
                }
            } 
        }
        return false;
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

    
    IEnumerator RandomRoam() {
        Vector3 pos = Quaternion.AngleAxis(Random.Range(-viewAngle, viewAngle)/2, Vector3.up) * transform.forward * Random.Range(roamDist.x, roamDist.y);
        agent.SetDestination(transform.position + pos);


// WAIT FOR PATH TO FINISH
        bool finished = false;
        if (agent.hasPath) {
            while (!finished) {
                if (CanSeePlayer()) {
                    state = EnemyState.Chasing;
                    sfxSource.PlayOneShot(chaseStingSFX.Get());
                    break;
                }
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
    }

    IEnumerator TrackBlood() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, trackingRadius, bloodPoolMask);
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
        if (!CanSeePlayer()) state = EnemyState.Tracking;
        yield return null;
    }

    void OnDrawGizmos () {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, trackingRadius);
        if (Application.isPlaying)
            Gizmos.color = CanSeePlayer() ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(viewAngle/2, Vector3.up) * transform.forward * viewDist);
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(-viewAngle/2, Vector3.up) * transform.forward * viewDist);
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
