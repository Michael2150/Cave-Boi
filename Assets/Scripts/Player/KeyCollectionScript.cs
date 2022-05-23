using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollectionScript : MonoBehaviour
{
    [SerializeField] AudioClip pickUpSFX;

    void OnTriggerEnter2D()
    {
        FindObjectOfType<GameSession>().handlePlayerCollectKey();
        playPickUpSFX();
        Destroy(gameObject);
    }

    void playPickUpSFX()
    {
        if (pickUpSFX != null)
        {
            AudioSource.PlayClipAtPoint(pickUpSFX, Camera.main.transform.position);
        }
    }
}
