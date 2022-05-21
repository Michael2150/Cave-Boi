using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpawnerScript : MonoBehaviour
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] float fireballSpawnOffsetSeconds;
    [SerializeField] float fireballPerXSeconds;
    [SerializeField] float fireballDirection;

    // Start is called before the first frame update
    void Start()
    {
        if (fireballPrefab != null)
            InvokeRepeating("createFireball", fireballPerXSeconds, fireballPerXSeconds);  //calls createFireball() every fireballPerXSeconds second(s)
    }

    private void createFireball()
    {
        Vector3 newFirePos = new Vector3(transform.position.x, transform.position.y, 0);
        GameObject newFireball = Instantiate(fireballPrefab, newFirePos, Quaternion.Euler(0, 0, fireballDirection));
        newFireball.transform.parent = gameObject.transform;
    }
}
