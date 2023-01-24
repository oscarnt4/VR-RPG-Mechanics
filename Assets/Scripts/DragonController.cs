using UnityEngine;
using UnityEngine.AI;
using Unity.XR.CoreUtils;
using System.Collections.Generic;

public class DragonController : MonoBehaviour
{
    private enum State
    {
        Roaming,
        ChaseTarget,
        Attacking,
    }

    [Header("Target")]
    [SerializeField] Transform targetPosition;
    [SerializeField] float viewDistance = 40f;
    [SerializeField] float stopChasingDistance = 60f;

    [Header("Roaming")]
    [SerializeField] float stopDistance = 1f;
    [SerializeField] float roamingRadius = 30f;

    [Header("Attack")]
    [SerializeField] float attackRange = 12f;
    [SerializeField] float stopAttackRange = 24f;

    NavMeshAgent navMeshAgent;
    Vector3 roamingPostion;
    Animator animator;
    State state;
    FireBreathController fireBreath;

    int isWalkingHash;
    int isBreathingFireHash;
    bool isBreathingFire;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        roamingPostion = GetRandomRoamingPosition(roamingRadius);
        animator = GetComponent<Animator>();
        fireBreath = GetComponentInChildren<FireBreathController>();
    }

    void Start()
    {
        state = State.Roaming;
        isWalkingHash = Animator.StringToHash("isWalking");
        isBreathingFireHash = Animator.StringToHash("isBreathingFire");
    }

    void Update()
    {
        switch (state)
        {
            default:
            case State.Roaming:
                bool destinationReached = SetDestination(roamingPostion);
                if (destinationReached)
                {
                    //Reached roaming position
                    roamingPostion = GetRandomRoamingPosition(roamingRadius);
                }
                FindTarget();
                break;
            case State.ChaseTarget:
                if (Vector3.Distance(targetPosition.position, this.transform.position) < attackRange)
                {
                    //Target is within attack range
                    SetDestination(this.transform.position);
                    state = State.Attacking;
                }
                else if (Vector3.Distance(targetPosition.position, this.transform.position) > stopChasingDistance)
                {
                    //Target is out of chasing range
                    state = State.Roaming;
                }
                else
                {
                    //Chase target
                    SetDestination(targetPosition.position);
                }
                break;
            case State.Attacking:
                if (!isBreathingFire)
                {
                    animator.SetBool(isBreathingFireHash, true);
                    isBreathingFire = true;
                }
                if (Vector3.Distance(targetPosition.position, this.transform.position) > stopAttackRange)
                {
                    animator.SetBool(isBreathingFireHash, false);
                    isBreathingFire = false;
                    state = State.ChaseTarget;
                }
                break;
        }
    }


    public bool SetDestination(Vector3 destination)
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        //Check stopping distance
        if (Vector3.Distance(destination, this.transform.position) > stopDistance)
        {
            navMeshAgent.destination = destination;
            //Activate walking animation
            if (!isWalking) animator.SetBool(isWalkingHash, true);
            return navMeshAgent.isStopped = false;
        }
        else
        {
            //Deactivate walking animation
            if (isWalking) animator.SetBool(isWalkingHash, false);
            return navMeshAgent.isStopped = true;
        }
    }

    private Vector3 GetRandomRoamingPosition(float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            //Generate a random point within a sphere radius
            Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * radius;
            //Find the nearest NavMesh position to the random point
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return this.transform.position;
    }

    private void FindTarget()
    {
        if (Vector3.Distance(this.transform.position, targetPosition.position) < viewDistance)
        {
            //Player is within viewable range
            state = State.ChaseTarget;
        }
    }

    public void StartFireBreath()
    {
        StartCoroutine(fireBreath.FireBreathSequence());
    }
}
