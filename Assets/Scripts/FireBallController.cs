using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour, IHarmful
{

    [Header("Stats")]
    [SerializeField] float speed = 1f;
    [SerializeField] float damage = 40f;
    [SerializeField] float explosionForce = 40f;
    [SerializeField] string damageType = "fire";

    [Header("VFX")]
    [SerializeField] GameObject explosionVFX;

    Vector3 velocity;
    RaycastHit hit;

    void Start()
    {
        velocity = this.transform.forward * speed;
    }

    void Update()
    {
        //Motion physics
        velocity += Physics.gravity * Time.deltaTime;
        this.transform.position += velocity * Time.deltaTime;
        HitDetection();
    }

    void HitDetection()
    {
        //Find motion direction
        Vector3 direction = velocity.normalized;

        //Cast ray in direction of motion
        if (Physics.Raycast(this.transform.position, direction, out hit, 0.5f))
        {
            //Check if ray hits an enemy
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                //Perform hit actions to hitbox
                EnemyHitBox hitBox = hit.collider.GetComponent<EnemyHitBox>();
                hitBox.OnRaycastHit(this);
            }

            //Generate particle effects
            GameObject explosion = Instantiate(explosionVFX, hit.point, Quaternion.Euler(hit.normal));
            explosion.GetComponent<ParticleSystem>().Play();
            Destroy(explosion, explosion.GetComponent<AudioSource>().clip.length);
            Destroy(this.gameObject);
        }
    }

    //For retrieving damage amount (IHarmful)
    public float GetDamage()
    {
        return damage;
    }

    //For retrieving damage type (IHarmful)
    public string GetDamageType()
    {
        return damageType;
    }

    public Vector3 GetDirection()
    {
        return velocity.normalized;
    }

    public float GetForce()
    {
        return explosionForce;
    }
}