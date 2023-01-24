using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] bool turnOnRagdoll = false;
    [SerializeField] Transform ragdollRoot;
    
    Animator animator;
    NavMeshAgent navMeshAgent;
    Rigidbody[] rigidbodies;

    bool ragdollActive = true;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rigidbodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();

        ActivateAnimator();
    }

    void Update()
    {
        //Turn on ragdoll effect
        if (turnOnRagdoll && !ragdollActive)
        {
            ActivateRagdoll();
        }

        //Turn on animator
        else if (!turnOnRagdoll && ragdollActive)
        {
            ActivateAnimator();
        }
    }

    public void ActivateRagdoll()
    {
        turnOnRagdoll = true;

        if (!ragdollActive)
        {
            animator.enabled = navMeshAgent.enabled = false;

            foreach (Rigidbody rigidbody in rigidbodies)
            {
                rigidbody.isKinematic = false;
            }

            ragdollActive = true;
        }
    }

    public void ActivateAnimator()
    {
        turnOnRagdoll = false;

        if (ragdollActive)
        {
            animator.enabled = navMeshAgent.enabled = true;

            foreach (Rigidbody rigidbody in rigidbodies)
            {
                rigidbody.isKinematic = true;
            }
            ragdollActive = false;
        }
    }
}
