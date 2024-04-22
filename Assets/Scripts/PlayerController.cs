using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Collision and HitDetection \/ \/ \/
    /// </summary>
    [SerializeField] GameObject groundCheck;
    [SerializeField] GameObject[] rapierSlashHitCheck;
    /// <summary>
    /// Collision and HitDetection /\ /\ /\
    /// </summary>

    // Layermasks
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask enemy;

    // All animation states that do not lock standard actions such as attacking, walking or jumping
    string[] nonActionLockedStates =
    {
        "Idle", "WalkBlendTree"
    };

    // Player rendering related variables
    public Animator playerAnimations;
    SpriteRenderer spriteRenderer;
    [SerializeField] AnimationClip rapierSlashAnim;

    // Booleans for whether or not some types of actions can be performed
    [SerializeField] bool actionsAvailable = true;
    [SerializeField] bool inAir;

    // Direction related variables
    [SerializeField] bool isHoldingLeftOrRight;
    [SerializeField] string directionFacing;
    [SerializeField] bool rotationLock;
    [SerializeField] bool toggleMouse;
    [SerializeField] int leftRightInt;
    float movingForwards;

    // Movement related variables
    [SerializeField] private float moveVelocityX;
    [SerializeField] private float moveVelocityY;
    [SerializeField] float moveAcceleration;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpHeight;


    // Events
    UnityEvent slashAttackEvent = new UnityEvent();
    public UnityEvent jumpEvent = new UnityEvent();
    public UnityEvent moveLeftEvent = new UnityEvent();
    public UnityEvent moveRightEvent = new UnityEvent();

    // Player is dead
    static public bool playerIsDead;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimations = GetComponent<Animator>();

        moveLeftEvent.AddListener(HoldingLeft);
        moveRightEvent.AddListener(HoldingRight);
        slashAttackEvent.AddListener(RapierSlash);
        jumpEvent.AddListener(Jump);

        moveVelocityX = 0f;
        moveVelocityY = 0f;

    }

    /// <summary>
    /// Handles Inputs To Be Acted Upon In FixedUpdate
    /// </summary>
    private void Update()
    {
        /*
         * Handles inputs or lack there of
         */
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J)) && actionsAvailable && !inAir)
        {
            slashAttackEvent.Invoke();
        }
        rotationLock = Input.GetKey(KeyCode.L);
        if (Input.GetKeyDown(KeyCode.Space) && actionsAvailable && !inAir)
        {
            jumpEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            toggleMouse = !toggleMouse;
        }
        if (Input.GetKey(KeyCode.W))
        {

        }
        if (Input.GetKey(KeyCode.A) && actionsAvailable && !inAir)
        {
            moveLeftEvent.Invoke();
        }
        if (Input.GetKey(KeyCode.S))
        {

        }
        if (Input.GetKey(KeyCode.D) && actionsAvailable && !inAir)
        {
            moveRightEvent.Invoke();
        }
        playerAnimations.SetBool("IsWalking", Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));
        isHoldingLeftOrRight = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && actionsAvailable;
        /*
         * Handles inputs or lack there of
         */



        moveVelocityX = Mathf.Clamp(moveVelocityX, -moveSpeed, moveSpeed);
    }

    /// <summary>
    /// Handles Most Things For Consistency Such As Movement And Collisions
    /// </summary>
    private void FixedUpdate()
    {
        // Collision detection \/ \/ \/
        inAir = !Physics2D.OverlapBox(groundCheck.transform.position, new Vector2(1.3f, 0f), 0f, ground);

        // Collision detection /\ /\ /\

        if (inAir)
        {
            moveVelocityY += -9.81f * Time.fixedDeltaTime;
        }
        else if (!inAir && moveVelocityY < 0f)
        {
            moveVelocityY = 0f;
        }


        AnimatorStateInfo animatorState = playerAnimations.GetCurrentAnimatorStateInfo(0);
        foreach (string state in nonActionLockedStates)
        {
            if (animatorState.IsName(state))
            {
                actionsAvailable = true;
                break;
            }
            else
            {
                actionsAvailable = false;
            }
        }


        // Checks whether player is moving towards the direction they are facing, whether that be the mouse, or direction lock on keyboard
        float movingTowardsMouse;
        float mouseToPlayerVector = Input.mousePosition.x - Camera.main.WorldToScreenPoint(transform.position).x;
        float mouseIsLeftOrRight = mouseToPlayerVector / Mathf.Abs(mouseToPlayerVector);
        if (((mouseIsLeftOrRight > 0 && actionsAvailable && toggleMouse) || (moveVelocityX > 0 && !rotationLock && !toggleMouse)) && actionsAvailable)
        {
            spriteRenderer.flipX = false;
            directionFacing = "Right";
        }
        else if (((mouseIsLeftOrRight < 0 && actionsAvailable && toggleMouse) || (moveVelocityX < 0 && !rotationLock && !toggleMouse)) && actionsAvailable)
        {
            spriteRenderer.flipX = true;
            directionFacing = "Left";
        }
        if (!isHoldingLeftOrRight && !inAir)
        {
            NotHoldingLeftOrRight();
        }


        float totalVelocityX = moveVelocityX;
        float totalVelocityY = moveVelocityY;
        transform.position += new Vector3(totalVelocityX, totalVelocityY, 0) * Time.fixedDeltaTime;
        movingTowardsMouse = (transform.position.x - movingForwards) * mouseIsLeftOrRight;
        movingForwards = transform.position.x;
        playerAnimations.SetFloat("SpeedHeadingTowards", movingTowardsMouse);
        playerAnimations.SetFloat("VelocityY", totalVelocityY);
        playerAnimations.SetBool("IsAirborne", inAir);


        if (directionFacing == "Left")
        {
            leftRightInt = 0;
        }
        else if (directionFacing == "Right")
        {
            leftRightInt = 1;
        }

    }


    void RapierSlash()
    {
        playerAnimations.SetTrigger("RapierSlashAttack");

        int frameCount = Mathf.CeilToInt(rapierSlashAnim.length * rapierSlashAnim.frameRate);
        Invoke("RapierSlashHit", rapierSlashAnim.length / frameCount * 1);
    }
    private void RapierSlashHit()
    {
        Debug.Log("Slash");
        try
        {
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(rapierSlashHitCheck[leftRightInt].transform.position, 1f, enemy);
            foreach (Collider2D enemy in enemiesHit)
            {
                try
                {
                    GameObject enemyGameObj = enemy.gameObject;
                    EnemyHealth enemyHealth = enemyGameObj.GetComponent<EnemyHealth>();
                    enemyHealth.BeenHit(10, "Slash");
                }
                catch
                {
                    Debug.Log("NoHits");
                }
            }
        }
        catch
        {
        }
    }
    /// <summary>
    /// When Called, increases/decreses moveVelocityX to make the character mover right or left
    /// </summary>
    private void HoldingLeft()
    {
        moveVelocityX += -moveAcceleration * Time.deltaTime;
        if (moveVelocityX > 0)
        {
            moveVelocityX += -moveAcceleration * Time.deltaTime;
        }
    }
    private void HoldingRight()
    {
        moveVelocityX += moveAcceleration * Time.deltaTime;
        if (moveVelocityX < 0)
        {
            moveVelocityX += moveAcceleration * Time.deltaTime;
        }
    }
    private void NotHoldingLeftOrRight()
    {
        if (moveVelocityX < 0)
        {
            moveVelocityX += moveSpeed * 2.5f * Time.fixedDeltaTime;
        }
        if (moveVelocityX > 0)
        {
            moveVelocityX += -moveSpeed * 2.5f * Time.fixedDeltaTime;
        }
        if (Mathf.Abs(moveVelocityX) < 0.1f)
        {
            moveVelocityX = 0f;
        }
    }
    /// <summary>
    /// When called, decreases/increases player moveVelocityX to bring it closer to 0, when within 0,1 of 0, sets it to 0
    ///  <summary>



    public void Jump()
    {
        moveVelocityY += jumpHeight;
    }


    public void BeenParried()
    {
        playerAnimations.SetTrigger("Parried");
    }
}

