using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 300;
    [SerializeField] float currentHealth;

    Ragdoll ragdoll;
    UIHealthBar healthBar;
    Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        ragdoll = GetComponent<Ragdoll>();
        healthBar = GetComponentInChildren<UIHealthBar>();
        animator = GetComponent<Animator>();

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            EnemyHitBox hitBox = rigidbody.gameObject.AddComponent<EnemyHitBox>();
            hitBox.SetHealth(this);
        }
    }

    public void TakeDamage(float damageToTake, Rigidbody rigidbodyHit, Vector3 direction, float forceAmount)
    {
        animator.SetTrigger("isHit");

        currentHealth -= damageToTake;
        if (currentHealth <= 0f)
        {
            Die(rigidbodyHit, direction, forceAmount);
        }

        healthBar.SetHealthBarPercentage(currentHealth / maxHealth);
    }
    public void TakeDamage(float damageToTake)
    {
        TakeDamage(damageToTake, null, Vector3.zero, 0f);
    }

    private void Die(Rigidbody rigidbodyHit, Vector3 direction, float forceAmount)
    {
        //Destroy health bar
        Destroy(healthBar.GetComponentInParent<Canvas>().gameObject);
        //Activate ragdoll effects
        ragdoll.ActivateRagdoll();
        //Add force to ragdoll
        if (rigidbodyHit) rigidbodyHit.AddForce(direction * forceAmount);
        //Destroy ragdoll after delay
        Destroy(this.gameObject, 90f);
    }
    private void Die()
    {
        Die(null, Vector3.zero, 0f);
    }
}
