using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    [SerializeField] AudioClip hitSFX;
    [SerializeField] float fireballSpeed = 5;

    private new Rigidbody2D rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = AngleAndMagToVector(rigidbody.transform.eulerAngles.z,fireballSpeed);
    }

    private Vector2 AngleAndMagToVector (float angle, float magnitude)
    {
        angle = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
    }

    void Update()
    {
        
    }

    #region Hit script

    void OnTriggerEnter2D()
    {
        if (touchingPlayer())
        {
            playHitSFX();
            FindObjectOfType<GameSession>().getPlayer().playerDie();
        }

        Destroy(gameObject);
    }

    void playHitSFX()
    {
        if (hitSFX != null)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = hitSFX;
            audio.Play();
        }
    }

    private bool touchingPlayer()
    {
        return rigidbody.IsTouchingLayers(LayerMask.GetMask("Player"));
    }

    #endregion
}
