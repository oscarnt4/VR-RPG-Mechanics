using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHarmful
{
    public float GetDamage();
    public Vector3 GetDirection();
    public float GetForce();
    public string GetDamageType();
}
