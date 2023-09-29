using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour {

    [Header("References")]
    NavMeshAgent agent;
    Bloodletter bloodletter;

    public enum EnemyState { Lurking, Roaming, Tracking, Chasing };
    [Header("State Machine")]
    public EnemyState state;


    [Header("Tracking Vars")]
    [SerializeField] float viewAngle;
    [SerializeField] float viewDist;
    [SerializeField] LayerMask viewMask;
    [SerializeField] float trackingRadius;
    [SerializeField] BloodPool currentPool;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        bloodletter = Bloodletter.instance;
        
        StartCoroutine(Pathfind());
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

    IEnumerator Lurk() {
        yield return null;

    }

    IEnumerator RandomRoam() {
        agent.SetDestination(Random.insideUnitSphere * trackingRadius);



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
    }

    IEnumerator TrackBlood() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, trackingRadius);
        Debug.Log(hitColliders.Length);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.GetComponent<BloodPool>()) {
                Debug.Log("Collider is bp");
                BloodPool bp = hitCollider.GetComponent<BloodPool>();
                if (currentPool == null || bp.age < currentPool.age) {
                    currentPool = bp;
                }
            }
        }
        if (currentPool)
            agent.SetDestination(currentPool.transform.position);

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
        yield return new WaitForSecondsRealtime(1f);
    }

    IEnumerator Chase() {
        yield return null;

    }

    void OnDrawGizmos () {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, trackingRadius);
        Gizmos.color = CanSeePlayer() ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, (transform.forward * viewDist));
	}

}
