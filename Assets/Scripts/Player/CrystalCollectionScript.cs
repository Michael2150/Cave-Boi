using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalCollectionScript : MonoBehaviour
{
    [SerializeField] AudioClip pickUpSFX;

    void OnTriggerEnter2D()
    {
        FindObjectOfType<GameSession>().handlePlayerCollectCoin();
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
