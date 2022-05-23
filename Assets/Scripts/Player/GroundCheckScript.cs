using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckScript : MonoBehaviour
{
    public float groundCheckWidth;
    public float groundCheckHeight;
    public LayerMask groundLayer;
    
    public float cayoteTime = 0.2f;
    private float cayoteTimer;
    
    private bool isGrounded;
    
    private void Update()
    {

    }
    
    //Getter boolean for if the player is on the ground
    public bool IsGrounded
    {
        get
        {
            isGrounded = Physics2D.OverlapBox(transform.position, new Vector2(groundCheckWidth, groundCheckHeight), 0, groundLayer);
            return isGrounded;
        }
    }

    //Draws a circle in the editor to see the ground check radius on selected
    private void OnDrawGizmosSelected()
    { 
        Gizmos.color =  (IsGrounded) ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(groundCheckWidth, groundCheckHeight, 0));
    }
}
