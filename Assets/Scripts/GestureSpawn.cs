using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureSpawn : MonoBehaviour
{
    public List<GameObject> objects;

    public void Spawn(string objectName, Vector3 spawnLocation)
    {
        foreach (GameObject _object in objects)
        {
            if (_object.name.Contains(objectName))
            {
                Instantiate(_object, spawnLocation, Camera.main.transform.rotation * _object.transform.rotation);
            }
        }
    }
}
