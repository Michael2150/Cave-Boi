using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    [SerializeField] Text noKeyText; 

    void Start()
    {
        noKeyText.gameObject.SetActive(false);
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D()
    {
        if (touchingPlayer())
        {
            if (! FindObjectOfType<GameSession>().handlePlayerTouchDoor())
            {
                //Show sign for key
                noKeyText.gameObject.SetActive(true);
                StartCoroutine(removeNoKeyText());
            }
        }
    }
    IEnumerator removeNoKeyText()
    {
        yield return new WaitForSecondsRealtime(4f);
        if (touchingPlayer())
        {
            StartCoroutine(removeNoKeyText());
        } else
        {
            noKeyText.gameObject.SetActive(false);
        }
    }

    private bool touchingPlayer()
    {
        return rigidbody.IsTouchingLayers(LayerMask.GetMask("Player"));
    }
}
