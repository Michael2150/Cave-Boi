using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPickupScript : MonoBehaviour
{
    [SerializeField] AudioClip pickUpSFX;

    void OnTriggerEnter2D()
    {
        FindObjectOfType<GameSession>().handlePlayerPickupExtraLife();
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
