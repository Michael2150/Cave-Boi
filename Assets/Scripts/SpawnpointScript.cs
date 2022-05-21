using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointScript : MonoBehaviour
{
    [SerializeField] bool mainSpawnpoint;
    [SerializeField] bool active;
    [SerializeField] bool alreadyActivated;

    void OnTriggerEnter2D()
    {
        if (!alreadyActivated)
        {
            ArrayList spawnpoints = new ArrayList(FindObjectsOfType<SpawnpointScript>());
            foreach (SpawnpointScript sp in spawnpoints)
            {
                sp.setActive(false);
            }
            alreadyActivated = true;
            active = true;
        }
    }

    public Vector3 getPos()
    {
        return transform.position;
    }

    public bool isActive()
    {
        return active;
    }
    public void setActive(bool isActive)
    {
        active = isActive;
    }
    public bool isMainSpawnpoint()
    {
        return mainSpawnpoint;
    }
}
