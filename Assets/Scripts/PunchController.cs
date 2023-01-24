using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchController : MonoBehaviour
{
    [Header("Spells")]
    [SerializeField] List<GameObject> spellProjectiles;
    //[SerializeField] float projectileSpeed = 1f;
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        string objectName = other.gameObject.name;
        //Remove (Clone) tag from name
        if (objectName.ToLower().Contains("(clone)"))
            objectName = objectName.Remove(objectName.Length - 8);
        
        //Check if punching valid spell rune
        foreach (GameObject projectile in spellProjectiles)
        {
            if (projectile.name.Contains(objectName))
            {
                Destroy(other.gameObject);
                ShootProjectile(projectile);
            }
        }
    }

    private void ShootProjectile(GameObject projectile)
    {
        GameObject spawnedProjectile = Instantiate(projectile);
        spawnedProjectile.transform.position = this.transform.position;
        spawnedProjectile.transform.rotation = this.transform.rotation;
    }
}
