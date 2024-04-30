using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    // Player and playerPrefab detection related variables
    GameObject player;
    PlayerController playerController;
    Animator playerAnimations;
    [SerializeField] bool playerSpotted;
    [SerializeField] float visionRange;

    // Health related varibles
    string enemyType;
    EnemyHealth healthStats;
    bool isDead = false;

    // Movement related variables
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float moveAcceleration;
    float velocityX;
    float velocityY;

    // Action availability variables
    bool isBusy;
    bool grounded;
    string[] nonActionLockedStates =
    {
        "DarkKnightWalk", "DarkKnightIdle"
    };

    // Combat related variables
    [SerializeField] string combatState;
    [SerializeField] float defensiveCooldown;
    [SerializeField] float offensiveCooldown;
    float inStateFor;
    bool isPanicking;
    float preferredCombatDistance;
    bool isPlayerToLeft;
    bool isParrying;
    bool isBlocking;
    float hitFrameDelay;

    // Enemy rendering related variables
    SpriteRenderer spriteRenderer;
    [SerializeField] int attackFrameRate;
    Animator animator;
    [SerializeField] AnimationClip darkKnightBasicSlash;
    [SerializeField] AnimationClip darkKnightParry;

    // Collision checks
    [SerializeField] GameObject groundCheck;

    // Hit checks
    [SerializeField] GameObject leftHitCheck;
    [SerializeField] GameObject rightHitCheck;

    // Layermasks
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask playerLayer;

    // Start is called before the first frame update
    void Start()
    {
        // Finds playerPrefab in scene, if an exception occurs, Writes "Player not found" in console
        if (player == null)
        {
            FindPlayer();
        }
        
        

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        healthStats = GetComponent<EnemyHealth>();
        

        inStateFor = 5f;
        isPanicking = false;

        enemyType = healthStats.enemyType;


        playerController = player.GetComponent<PlayerController>();
        playerAnimations = player.GetComponent<Animator>();

        if (enemyType == "Basic")
        {
            maxMoveSpeed = 4;
            moveAcceleration = 8;
            preferredCombatDistance = 2.75f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            playerSpotted = false;
        }

        

        if (enemyType == "Basic" && player != null)
        {
            BasicEnemyUpdate();
        }

    }

    void FixedUpdate()
    {
        if (player == null)
        {
            playerSpotted = false;
        }
        if (player == null)
        {
            FindPlayer();
        }
        if (enemyType == "Basic" && player != null)
        {
            BasicEnemyFixedUpdate();
        }

        velocityX = Mathf.Clamp(velocityX, -maxMoveSpeed, maxMoveSpeed);
        transform.position += new Vector3(velocityX, velocityY, 0) * Time.fixedDeltaTime;

        grounded = Physics2D.OverlapBox(groundCheck.transform.position, new Vector2(1.3f, 0), 0f, ground);
        if (grounded && velocityY < 0)
        {
            velocityY = 0f;
            
        }
        else if (!grounded)
        {
            velocityY += -9.81f * Time.fixedDeltaTime;
        }

        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        foreach (string state in nonActionLockedStates)
        {
            if (animatorStateInfo.IsName(state))
            {
                isBusy = false;
                break;
            }
            else
            {
                isBusy = true;
            }
        }

        if (isBusy)
        {
            SlowDown();
        }

        if (healthStats.health < 1)
        {
            Die();
        }

    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    // Standard Update call dedicated to the "Basic" enemy type
    private void BasicEnemyUpdate()
    {

        if (playerSpotted)
        {


        }
    }
    float dodgeCooldown = 0;

    // FixedUpdate call dedicated to the "Basic" enemy type
    private void BasicEnemyFixedUpdate()
    {

        float playerDistance = player.transform.position.x - transform.position.x;
        if (Mathf.Abs(playerDistance) < visionRange && !playerSpotted)
        {
            playerSpotted = true;
            combatState = "Cautious";
        }

        if (offensiveCooldown > 0)
        {
            offensiveCooldown -= Time.fixedDeltaTime;
        }

        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        hitFrameDelay = darkKnightBasicSlash.length / darkKnightBasicSlash.frameRate * 4;
        

        animator.SetFloat("SpeedWalkingTowards", velocityX);
        animator.SetFloat("MoveSpeed", Mathf.Abs(velocityX) / maxMoveSpeed);


        if (playerSpotted)
        {
            isPlayerToLeft = playerDistance < 0f;
            spriteRenderer.flipX = isPlayerToLeft;

            if (healthStats.health > 33.3f)
            {
                inStateFor -= Time.fixedDeltaTime;
            }
            
            if (defensiveCooldown > 0)
            {
                defensiveCooldown -= Time.fixedDeltaTime;
            }
            if (healthStats.health < 33.3f && !isPanicking)
            {
                isPanicking = true;
                int panicState = Random.Range(1, 3);
                switch (panicState)
                {
                    case 1:
                        {
                            offensiveCooldown = 0.5f;
                            combatState = "Aggressive";
                                break;
                        }
                    case 2:
                        {
                            combatState = "Defensive";
                                break;
                        }
                }


            }



            switch (combatState)
            {




                /* Enemy Is Aggressive.
                 * An Aggressive enemy will move in close, to a range where dodging is impossible,
                 * it will have a near complete disregard for survival, with the exception of an occasional parry.
                 * An Aggressive enemy is just as fast as the playerPrefab, and accelerates substantially faster, cannot be outrun
                 * On occasion an attack against an Aggressive enemy will be met with a parry.
                 * An Aggressive enemy will not stay aggressive for long and will almost immediately switch back to being Cautious or Defensive
                 */
                case "Aggressive":
                    {
                        maxMoveSpeed = 5;
                        moveAcceleration = 12;
                        preferredCombatDistance = 2f;


                        if (offensiveCooldown <= 0)
                        {
                            Debug.Log("StartAttack has been called");
                            StartAttack();
                        }

                        // If enemy is within preferred range of distance from the playerPrefab when aggressive, it will attempt to keep that distance
                        if (Mathf.Abs(Mathf.Abs(playerDistance) - preferredCombatDistance) < 0.25f)
                        {
                            SlowDown();
                        }

                        // If enemy is farther than preferred distance from the playerPrefab it will attempt to close it
                        else if (Mathf.Abs(playerDistance) > preferredCombatDistance && !isBusy)
                        {
                            // Moves the enemy based on which side it is on in relation to the playerPrefab
                            if (isPlayerToLeft)
                            {
                                MoveLeft();
                            }
                            else
                            {
                                MoveRight();
                            }
                        }

                        // If enemy is closer than preferred distance from the playerPrefab it will attempt to make distance
                        if (Mathf.Abs(playerDistance) < preferredCombatDistance && !isBusy)
                        {
                            // Moves the enemy based on which side it is on in relation to the playerPrefab
                            if (isPlayerToLeft)
                            {
                                MoveRight();
                            }
                            else
                            {
                                MoveLeft();
                            }
                        }

                        // This changes the state of the enemy to one of the states it is not in whenever the "inStateFor" timer runs out
                        if (inStateFor <= 0f && !isBusy)
                        {
                            int nextState = Random.Range(0, 2);
                            if (nextState == 0)
                            {
                                combatState = "Cautious";
                                inStateFor = 2f;
                            }
                            else
                            {
                                combatState = "Defensive";
                            inStateFor = 3.5f;
                            }
                        }
                            break;
                    }










                /* Enemy Is Defensive
                 * When Defensive the enemy will be at a closer range where it is barely within attacking range
                 * A Defensive enemy is slow, with a slow acceleration, can be outrun
                 * Any attempted attacks against a Defensive enemy will be met with a parry stunning the playerPrefab for just under a second or a block, mitigating all damage 
                 * A Defensive enemy can change to any state, with the exception being it immediately becoming aggressive when landing a successful parry
                 */
                case "Defensive":
                    {
                        maxMoveSpeed = 3.5f;
                        moveAcceleration = 8;
                        preferredCombatDistance = 2.75f;

                        if (offensiveCooldown <= 0)
                        {
                            int rand = Random.Range(1, 4);
                            if (rand == 1)
                            {
                                Debug.Log("StartAttack has been called");
                                StartAttack();
                            }
                        }

                        // If enemy is within preferred range of distance from the playerPrefab when defensive, it will attempt to keep that distance
                        if (Mathf.Abs(Mathf.Abs(playerDistance) - preferredCombatDistance) < 0.25f)
                        {
                            SlowDown();
                        }

                        // If enemy is farther than preferred distance from the playerPrefab it will attempt to close it
                        else if (Mathf.Abs(playerDistance) > preferredCombatDistance && !isBusy)
                        {
                            // Moves the enemy based on which side it is on in relation to the playerPrefab
                            if (isPlayerToLeft)
                            {
                                MoveLeft();
                            }
                            else
                            {
                                MoveRight();
                            }
                        }

                        // If enemy is closer than preferred distance from the playerPrefab it will attempt to make distance
                        if (Mathf.Abs(playerDistance) < preferredCombatDistance && !isBusy)
                        {
                            // Moves the enemy based on which side it is on in relation to the playerPrefab
                            if (isPlayerToLeft)
                            {
                                MoveRight();
                            }
                            else
                            {
                                MoveLeft();
                            }
                        }

                        // Randomly chooses an int between 0 - 10 (including 0, 10), and blocks, parries or does nothing depending on the number
                        if (playerAnimations.GetCurrentAnimatorStateInfo(0).IsName("RapierSlash") &&  defensiveCooldown <= 0 && !isBusy)
                        {
                            defensiveCooldown = 1;
                            int defensiveManeuver = Random.Range(0, 11);
                    
                            if (defensiveManeuver >= 8)
                            {
                                Invoke("Parry", 0f);
                            }
                            else if (defensiveManeuver > 4 && defensiveManeuver <= 7)
                            {
                                Invoke("Guard", 0f);
                            }
                        }


                        // This changes the state of the enemy to one of the states it is not in whenever the "inStateFor" timer runs out
                        if (inStateFor <= 0f && !isBusy)
                        {
                            int nextState = Random.Range(0, 2);
                            if (nextState == 0)
                            {
                                offensiveCooldown = 0.5f;
                                combatState = "Aggressive";
                                inStateFor = 2;
                            }
                            else
                            {
                                combatState = "Cautious";
                                inStateFor = 2f;
                            }
                        }
                            break;
                    }








                /* 
                 * Enemy Is Cautious 
                 * When Cautious the enemy will try to keep itself at a safer distance
                 * A Defensive enemy is slighty slower than the playerPrefab, but accelerates at the same speed, can be outrun (if only just)
                 * Any attempted attacks at a Cautious enemy will cause it to backstep and dodge the attack
                 * A Cautious enemy can can change to any state
                 */
                case "Cautious":
                    {
                        maxMoveSpeed = 4.5f;
                        moveAcceleration = 10;
                        preferredCombatDistance = 3.5f;

                        // If enemy is within preferred range of distance from the playerPrefab when cautious, it will attempt to keep that distance
                        if (Mathf.Abs(Mathf.Abs(playerDistance) - preferredCombatDistance) < 0.25f)
                        {
                            SlowDown();
                        }

                        // If the enemy is farther than the preferred distance from the playerPrefab, it will attempt to close the distance
                        else if (Mathf.Abs(playerDistance) > preferredCombatDistance && !isBusy)
                        {
                            // Moves the enemy based on which side it is on in relation to the playerPrefab
                            if (isPlayerToLeft)
                            {
                                MoveLeft();
                            }
                            else
                            {
                                MoveRight();
                            }
                        }

                        // If enemy is closer than preferred distance from the playerPrefab it will attempt to make distance
                        if (Mathf.Abs(playerDistance) < preferredCombatDistance && !isBusy)
                        {
                            // Moves the enemy based on which side it is on in relation to the playerPrefab
                            if (isPlayerToLeft)
                            {
                                MoveRight();
                            }
                            else
                            {
                                MoveLeft();
                            }
                        }


                        dodgeCooldown -= Time.fixedDeltaTime; // Countdown to make the enemy not spam dodge

                        // This will cause the enemy to dodge when the playerPrefab uses the attack that the "RapierSlash" animation is attached to
                        if (playerAnimations.GetCurrentAnimatorStateInfo(0).IsName("RapierSlash") && dodgeCooldown <= 0f && !isBusy)
                        {
                            dodgeCooldown = 2;
                            if (isPlayerToLeft)
                            {
                                DodgeRight();

                            }
                            else
                            {
                                DodgeLeft();
                            }
                        }

                        // This changes the state of the enemy to one of the states it is not in whenever the "inStateFor" timer runs out
                        if (inStateFor <= 0f && !isBusy)
                        {
                            int nextState = Random.Range(0, 2);
                            if (nextState == 0)
                            {
                                offensiveCooldown = 0.5f;
                                combatState = "Aggressive";
                                inStateFor = 2;
                            }
                            else
                            {
                                combatState = "Defensive";
                                inStateFor = 3.5f;
                            }
                    
                        }
                            break;
                    }
            }
        }
        
        
        



    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




































    void FindPlayer()
    {
        try
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Debug.Log("Player Found");
                playerController = player.GetComponent<PlayerController>();
                playerAnimations = player.GetComponent<Animator>();
            }
        }
        catch
        {
        }
    }





    private void StartAttack()
    {
        offensiveCooldown = 3;
        animator.SetTrigger("BasicSlash");
        Invoke("AttackHitCheck", hitFrameDelay / 0.8f);
        Debug.Log("Attack delay: " + hitFrameDelay);
    }

    private void AttackHitCheck()
    {
        switch (isPlayerToLeft)
        {
            case true:
                {
                    Debug.Log("Attack Check Successful");
                    Collider2D playerHitbox = Physics2D.OverlapCircle(leftHitCheck.transform.position, 1.25f, playerLayer);
                    if (playerHitbox != null && !isDead)
                    {
                        Debug.Log("PlayerHit");
                        PlayerHealth playerHealth = playerHitbox.GetComponent<PlayerHealth>();
                        playerHealth.OnPlayerReceiveDamage("Normal", gameObject);
                    }
                    break;
                }
            case false:
                {
                    Debug.Log("Attack Check Successful");
                    Collider2D playerHitbox = Physics2D.OverlapCircle(rightHitCheck.transform.position, 1.25f, playerLayer);
                    if (playerHitbox != null && !isDead)
                    {
                        Debug.Log("PlayerHit");
                        PlayerHealth playerHealth = playerHitbox.GetComponent<PlayerHealth>();
                        playerHealth.OnPlayerReceiveDamage("Normal", gameObject);
                    }
                    break;
                }
        }
    }



    // Will gradually slow the enemy to a halt when called to prevent the enemy from moving back and forth around the preferred range like a pendulum
    void SlowDown()
    {
        velocityX += -velocityX * 5f * Time.fixedDeltaTime;
        
    }

    // Will move the enemy left
    void MoveLeft()
    {
        velocityX += -moveAcceleration * Time.fixedDeltaTime;



    }

    // Will move the enemy right
    void MoveRight()
    {
        velocityX += moveAcceleration * Time.fixedDeltaTime;
    }

    // When called, immediately sets the velocity to make the enemy instantly start moving left
    void DodgeLeft()
    {
        velocityX = -2.5f;
    }

    // When called, immediately sets the velocity to make the enemy instantly start moving right
    void DodgeRight()
    {
        velocityX = 2.5f;
    }

    // Will start blocking to prevent oncoming attacks
    void Guard()
    {
        isBlocking = true;
        Invoke("EndGuard", 1.5f);
    }

    // Will end the block, allowing other actions
    void EndGuard()
    {
        isBlocking = false;
    }

    // Will start parrying, blocking oncoming damage, and stunning the playerPrefab
    void Parry()
    {
        isParrying = true;
        animator.SetTrigger("Parry");
        Invoke("EndParry", 0.8f);
    }

    // Ends the parry
    void EndParry()
    {
        isParrying = false;
    }

    // Will return the information about a damage source as is, unless if parried or blocked
    public void DefenceCheck(int damage, string damageType)
    {
        int frames = Mathf.CeilToInt(darkKnightParry.length * darkKnightParry.frameRate);
        float parryAttackDelay = darkKnightParry.length / frames * 6;

        // Will return normal damage values and types if it wasn't blocked
        if (!isBlocking && !isParrying)
        {
            EnemyHealth health = GetComponent<EnemyHealth>();
            health.ReceiveDamage(damage, damageType);
        }

        // Will cause the playerPrefab to stagger
        else if (isParrying)
        {
            inStateFor = 2;
            offensiveCooldown = 0.25f;
            combatState = "Aggressive";
            Invoke("AttackHitCheck", parryAttackDelay);
            playerController.Invoke("Staggered", 0.08f);
        }

        // Returns the damage taken as 0, and type as blocked unless if attack is "Unblockable", in which case damage and damageType stay the same
        else if (isBlocking)
        {
            EnemyHealth health = GetComponent<EnemyHealth>();
            damage = 0;
            damageType = "Blocked";
            health.ReceiveDamage(damage, damageType);
        }

        

    }

    public void Staggered()
    {
        Debug.Log("Staggered");
        animator.SetTrigger("Stagger");
    }



    private void Die()
    {

        isDead = true;
        animator.SetTrigger("Die");
        Destroy(gameObject, 3f);
    }


    private void OnDestroy()
    {
        
    }
}
