using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float climbingSpeed;
    [SerializeField] float swimmingSpeed;
    [SerializeField] bool playerEnabled = true;
    [SerializeField] bool playerPlaySound = true;

    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip deathSound;

    private Animator animator;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private Collider2D waterCollider;
    private float normalGravity;

    //Variables to handle the state of the player.
    private bool running;
    private bool wasRunning;
    private bool swimming;
    private bool wasSwimming;
    private float momentumJumpingInWater;
    private bool climbing;
    private bool wasClimbing;
    private bool feetOnGround = true;
    private float normalClimbingAnimationSpeed;
    private bool jumping;
    private bool wasJumping;
    private bool dead;
    private bool cannotDie;

    //Player stats
    [SerializeField] float maxOxygenLevel = 10f;
    [SerializeField] float oxygenLevel = 10f;
    [SerializeField] float loseOxygenPerSecond = 1f;
    [SerializeField] float gainOxygenPerSecond = 1f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        waterCollider = (Collider2D) GameObject.Find("Water").GetComponent("Collider2D");

        normalGravity = rigidbody.gravityScale;

        InvokeRepeating("updateOxygenLevels", 1f, 1f);  //calls the updateOxygenLevels() every 1 second
    }

    // Update is called once per frame
    void Update()
    {
        if (playerEnabled)
        {
            flipSprite();
            run();
            die();
            swim();
            climb();
            jump();
        }
    }

    #region Player Abilities Methods

    void run()
    {
        if (dead)
            return;

        float horizontalMovement = Input.GetAxis("Horizontal") * playerSpeed; //Get the horizontal velocity from input
        running = (!climbing) && (Mathf.Abs(horizontalMovement) > 0);

        if (running)
        {
            if (!wasRunning)
            {
                // === Start Running === 
                Debug.Log("Start Running");
                wasRunning = true;
                setAnimation(AnimationStates.RUNNING, true);
            }
            else
            {
                // === Running === 
                rigidbody.velocity = new Vector2(horizontalMovement, rigidbody.velocity.y); //Apply the velocity to player
            }
        }
        else if (wasRunning)
        {
            // === Stop Running ===
            Debug.Log("Stop Running");
            wasRunning = false;
            rigidbody.velocity = new Vector2(horizontalMovement, rigidbody.velocity.y);
            setAnimation(AnimationStates.RUNNING, false);
        } else if (!climbing)
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }
    }

    void jump()
    {
        if (dead)
            return;

        jumping = (!swimming && !climbing) && (playerTouchingGround() && feetOnGround && Input.GetButtonDown("Jump"));

        if (jumping)
        {
            if (!wasJumping)
            {
                // === Start Jumping === 
                Debug.Log("Start Jumping");
                wasJumping = true;
                setAnimation(AnimationStates.JUMPING, true);
                rigidbody.velocity += new Vector2(0, jumpSpeed);

                if (jumpSound != null)
                {
                    AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position);
                }
            }
        }
        else if (wasJumping)
        {
            // === Stop Jumping ===
            Debug.Log("Stop Jumping");
            wasJumping = false;
            setAnimation(AnimationStates.JUMPING, false);
        }
    }

    void climb()
    {
        if (dead)
            return;

        climbing = playerTouchingClimbing();

        if (climbing)
        {
            if (!wasClimbing)
            {
                // === Start Climbing === 
                Debug.Log("Start Climbing");
                wasClimbing = true;
                rigidbody.gravityScale = 0f;
                setAnimation(AnimationStates.CLIMBING, true);
                normalClimbingAnimationSpeed = animator.speed;
            }
            else
            {
                // === Already Climbing === 

                // Movement when Climbing
                float horizontalMovement = Input.GetAxis("Horizontal") * climbingSpeed; //Get the horizontal velocity from input
                float verticalMovement = Input.GetAxis("Vertical") * climbingSpeed; //Get the vertical velocity from input
                rigidbody.velocity = new Vector2(horizontalMovement, verticalMovement); //Apply the velocity to player

                animator.speed = playerMovingVert() ? normalClimbingAnimationSpeed : 0f;
            }
        }
        else if (wasClimbing)
        {
            // === Stop Climbing === 
            Debug.Log("Stop Climbing");
            wasClimbing = false;
            animator.speed = normalClimbingAnimationSpeed;
            setAnimation(AnimationStates.CLIMBING, false);
            rigidbody.gravityScale = normalGravity;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y + (jumpSpeed / 2));
        }
    }

    void swim()
    {
        if (dead)
            return;

        swimming = playerTouchingWater();

        if (swimming)
        {
            if (!wasSwimming)
            {
                if (playerUnderWater())
                {
                    // === Start Swimming === 
                    Debug.Log("Start Swimming");
                    wasSwimming = true;
                    momentumJumpingInWater = rigidbody.velocity.y/2;
                    rigidbody.gravityScale = normalGravity / 1.5f;
                }
            } else
            {
                // === Already Swimming === 

                // Movement when swimming
                float horizontalMovement = Input.GetAxis("Horizontal") * swimmingSpeed; //Get the horizontal velocity from input
                float verticalMovement = Input.GetAxis("Vertical") * swimmingSpeed; //Get the vertical velocity from input
                rigidbody.velocity = new Vector2(horizontalMovement, verticalMovement + momentumJumpingInWater); //Apply the velocity to player with a bit of boyency

                if (momentumJumpingInWater != 0)
                {
                    float dif = 0.1f;
                    if (momentumJumpingInWater > 0)
                        momentumJumpingInWater = Mathf.Max(0, momentumJumpingInWater - dif);
                    if (momentumJumpingInWater < 0)
                        momentumJumpingInWater = Mathf.Min(0, momentumJumpingInWater + dif);
                }
                    
            }
        }
        else if (wasSwimming)
        {
            // === Stop Swimming === 
            Debug.Log("Stop Swimming");
            wasSwimming = false;
            rigidbody.gravityScale = normalGravity;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y + (0.75f * jumpSpeed));
        }
    }

    //This method is called every 1 second
    void updateOxygenLevels()
    {
        if (oxygenLevel <= 0f)
        {
            playerDie();
        } else
        {
            if (oxygenLevel > 0f)
            {
                if (playerUnderWater())
                {
                    oxygenLevel -= loseOxygenPerSecond;
                }
                else if (oxygenLevel <= maxOxygenLevel)
                {
                    oxygenLevel = Mathf.Min(maxOxygenLevel, oxygenLevel + gainOxygenPerSecond);
                }
                FindObjectOfType<GameSession>().setPlayerOxygenLevel(oxygenLevel, maxOxygenLevel);
            }
        }
    }

    void die()
    {
        if (playerTouchingHazard())
        {
            if (!dead && !cannotDie)
            {
                playerDie();
            }
        }
    }
    public void playerDie()
    {
        if (!dead)
        {
            Debug.Log("Player died!");
            dead = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpSpeed);
            if (deathSound != null && playerPlaySound)
            {
                AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
            }
            removeAllAnimations();
            setAnimation(AnimationStates.DYING, true);
            FindObjectOfType<GameSession>().handlePlayerDeath();
        }
    }

    public void playerReset()
    {
        setAnimation(AnimationStates.DYING, false);
        dead = false;
        cannotDie = true;
        oxygenLevel = maxOxygenLevel;
        StartCoroutine(removeInvincibility());
    }
    IEnumerator removeInvincibility()
    {
        yield return new WaitForSecondsRealtime(1f);
        cannotDie = false;
    }

    public void playerWin()
    {
        cannotDie = true;
        rigidbody.velocity = Vector2.zero;
        if (winSound != null && playerPlaySound)
        {
            AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
        }
        removeAllAnimations();
        setAnimation(AnimationStates.WINNING,true);
        playerEnabled = false;
    }

    #endregion

    #region Visual & Animation methods

    //Code from weekly labs.
    private void flipSprite()
    {
        if (playerMovingHorz() && (Input.GetAxis("Horizontal") != 0)) //Player moving
        {
            float direction = Mathf.Sign(rigidbody.velocity.x);  // calculate direction of movement
            transform.localScale = new Vector2(direction, 1f);  // apply movement direction to sprite
        }
    }

    private void setAnimation(AnimationStates state, bool val)
    {
        string animationStateName = "";

        switch (state)
        {
            case AnimationStates.IDLE:
                animationStateName = "Idle";
                break;
            case AnimationStates.RUNNING:
                animationStateName = "Running";
                break;
            case AnimationStates.JUMPING:
                animationStateName = "Jumping";
                break;
            case AnimationStates.CLIMBING:
                animationStateName = "Climbing";
                break;
            case AnimationStates.SHOOTING:
                animationStateName = "Shooting";
                break;
            case AnimationStates.WINNING:
                animationStateName = "Winning";
                break;
            case AnimationStates.DYING:
                animationStateName = "Dying";
                break;
            default:
                Debug.LogError("State Not Found");
                break;
        }

        if (animator.GetBool(animationStateName) != val)
        {
            animator.SetBool(animationStateName, val);
        }
    }

    /// <summary>
    /// An enum holding the values of the animation states.
    /// To set the animation set the "AnimationState" int to one of these values.
    /// </summary>
    private enum AnimationStates
    {
        IDLE = 0,
        RUNNING = 1,
        JUMPING = 2,
        CLIMBING = 3,
        SHOOTING = 4,
        WINNING = 5,
        DYING = 6
    }

    #endregion

    #region Helper methods
    private bool playerMovingHorz()
    {
        float errorMargin = 0.001f;
        return Mathf.Abs(rigidbody.velocity.x) > errorMargin;
    }
    private bool playerMovingVert()
    {
        return Mathf.Abs(rigidbody.velocity.y) > 0;
    }
    private void removeAllAnimations()
    {
        foreach (AnimationStates a in System.Enum.GetValues(typeof(AnimationStates)))
        {
            setAnimation(a, false);
        }
    }
    private bool playerTouchingGround()
    {
        return rigidbody.IsTouchingLayers(LayerMask.GetMask("Foreground"));
    }
    private bool playerTouchingWater()
    {
        return rigidbody.IsTouchingLayers(LayerMask.GetMask("Water"));
    }
    private bool playerTouchingClimbing()
    {
        return rigidbody.IsTouchingLayers(LayerMask.GetMask("Climbing"));
    }
    private bool playerTouchingHazard()
    {
        return rigidbody.IsTouchingLayers(LayerMask.GetMask("Hazard"));
    }
    private bool playerUnderWater()
    {
        if (waterCollider != null)
        {
            //Found here to get the points around a collider https://answers.unity.com/questions/1355096/how-to-get-the-8-vertices-coordinates-of-a-box-col.html
            return waterCollider.OverlapPoint(new Vector2(collider.bounds.min.x, collider.bounds.min.y)) 
                && waterCollider.OverlapPoint(new Vector2(collider.bounds.min.x, collider.bounds.max.y)) 
                && waterCollider.OverlapPoint(new Vector2(collider.bounds.max.x, collider.bounds.min.y)) 
                && waterCollider.OverlapPoint(new Vector2(collider.bounds.max.x, collider.bounds.max.y));
        }
        return false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        feetOnGround = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        feetOnGround = true;
    }

    #endregion
}
