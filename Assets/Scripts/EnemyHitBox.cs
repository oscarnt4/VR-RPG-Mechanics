using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    Health health;

    public void SetHealth(Health health){
        this.health = health;
    }

    public void OnRaycastHit(IHarmful weapon)
    {
        health.TakeDamage(weapon.GetDamage(), this.GetComponent<Rigidbody>(), weapon.GetDirection(), weapon.GetForce());
    }
}
