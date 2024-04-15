using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;
    Animator playerAnimations;
    [SerializeField] bool playerSpotted;
    [SerializeField] float visionRange;


    string enemyType;
    EnemyHealth healthStats;


    [SerializeField] float maxMoveSpeed;
    [SerializeField] float moveAcceleration;
    float velocityX;
    float velocityY;



    [SerializeField] string combatState;
    float inStateFor;
    float preferredCombatDistance;
    bool isPlayerToLeft;
    bool isParrying;
    bool isBlocking;


    SpriteRenderer spriteRenderer;



    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthStats = GetComponent<EnemyHealth>();

        inStateFor = 7.5f;
        
        enemyType = healthStats.enemyType;

        try
        {
            player = GameObject.Find("Player");
            Debug.Log("Player found");
        }
        catch
        {
            Debug.Log("Player not found");
        }

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
        if (enemyType == "Basic")
        {
            BasicEnemyUpdate();
        }


    }

    void FixedUpdate()
    {
        if (enemyType == "Basic")
        {
            BasicEnemyFixedUpdate();
        }

        velocityX = Mathf.Clamp(velocityX, -maxMoveSpeed, maxMoveSpeed);
        transform.position += new Vector3(velocityX, velocityY, 0) * Time.fixedDeltaTime;
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void BasicEnemyUpdate()
    {

        if (playerSpotted)
        {


        }
    }
    private void BasicEnemyFixedUpdate()
    {
        float playerDistance = player.transform.position.x - transform.position.x;
        if (Mathf.Abs(playerDistance) < visionRange && !playerSpotted)
        {
            playerSpotted = true;
            combatState = "Cautious";
        }
        if (playerSpotted)
        {
            isPlayerToLeft = playerDistance < 0f;
            spriteRenderer.flipX = isPlayerToLeft;

            inStateFor -= Time.fixedDeltaTime;

            /// Enemy Is Aggressive
            if (combatState == "Aggressive")
            {
                maxMoveSpeed = 5;
                moveAcceleration = 12;
                preferredCombatDistance = 2.25f;

                if (Mathf.Abs(Mathf.Abs(playerDistance) - preferredCombatDistance) < 0.25f)
                {
                    SlowDown();
                }
                else if (Mathf.Abs(playerDistance) > preferredCombatDistance)
                {
                    if (isPlayerToLeft)
                    {
                        MoveLeft();
                    }
                    else
                    {
                        MoveRight();
                    }
                }
                if (Mathf.Abs(playerDistance) < preferredCombatDistance)
                {
                    if (isPlayerToLeft)
                    {
                        MoveRight();
                    }
                    else
                    {
                        MoveLeft();
                    }
                }

                if (inStateFor <= 0f)
                {
                    int nextState = Random.Range(0, 2);
                    if (nextState == 0)
                    {
                        combatState = "Cautious";
                        inStateFor = 5;
                    }
                    else
                    {
                        combatState = "Defensive";
                    inStateFor = 5;
                    }
                }
            }





            /// Enemy Is Defensive
            if (combatState == "Defensive")
            {
                maxMoveSpeed = 4;
                moveAcceleration = 8;
                preferredCombatDistance = 2.75f;

                if (Mathf.Abs(Mathf.Abs(playerDistance) - preferredCombatDistance) < 0.25f)
                {
                    SlowDown();
                }
                else if (Mathf.Abs(playerDistance) > preferredCombatDistance)
                {
                    if (isPlayerToLeft)
                    {
                        MoveLeft();
                    }
                    else
                    {
                        MoveRight();
                    }
                }
                if (Mathf.Abs(playerDistance) < preferredCombatDistance)
                {
                    if (isPlayerToLeft)
                    {
                        MoveRight();
                    }
                    else
                    {
                        MoveLeft();
                    }
                }

                if (playerAnimations.GetCurrentAnimatorStateInfo(0).IsName("RapierSlash"))
                {

                    int defensiveManeuver = Random.Range(0, 11);
                    isParrying = defensiveManeuver > 8;
                    isBlocking = defensiveManeuver > 3 && defensiveManeuver <= 8;
                }

                if (inStateFor <= 0f)
                {
                    int nextState = Random.Range(0, 2);
                    if (nextState == 0)
                    {
                        combatState = "Aggressive";
                        inStateFor = 2;

                    }
                    else
                    {
                        combatState = "Cautious";
                        inStateFor = 5;
                    }
                }
            }





            /// Enemy Is Cautious
            if (combatState == "Cautious")
            {
                maxMoveSpeed = 5;
                moveAcceleration = 10;
                preferredCombatDistance = 3.5f;

                if (Mathf.Abs(Mathf.Abs(playerDistance) - preferredCombatDistance) < 0.25f)
                {
                    SlowDown();
                }
                else if (Mathf.Abs(playerDistance) > preferredCombatDistance)
                {
                    if (isPlayerToLeft)
                    {
                        MoveLeft();
                    }
                    else
                    {
                        MoveRight();
                    }
                }
                if (Mathf.Abs(playerDistance) < preferredCombatDistance)
                {
                    if (isPlayerToLeft)
                    {
                        MoveRight();
                    }
                    else
                    {
                        MoveLeft();
                    }
                }
                
                if (playerAnimations.GetCurrentAnimatorStateInfo(0).IsName("RapierSlash"))
                {
                    if (isPlayerToLeft)
                    {
                        DodgeRight();
                    }
                    else
                    {
                        DodgeLeft();
                    }
                }


                if (inStateFor <= 0f)
                {
                    int nextState = Random.Range(0, 2);
                    if (nextState == 0)
                    {
                        combatState = "Aggressive";
                        inStateFor = 2;
                    }
                    else
                    {
                        combatState = "Defensive";
                        inStateFor = 5;
                    }
                    
                }

            }
        }
        
        
        



    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////














































    void SlowDown()
    {
        velocityX += -velocityX * 5f * Time.fixedDeltaTime;
    }
    void MoveLeft()
    {
        velocityX += -moveAcceleration * Time.fixedDeltaTime;



    }
    void MoveRight()
    {
        velocityX += moveAcceleration * Time.fixedDeltaTime;



    }

    void DodgeLeft()
    {
        velocityX = -2.5f;
    }
    void DodgeRight()
    {
        velocityX = 2.5f;
    }

    void Guard()
    {

    }

    void Parry()
    {

    }


    public void IsGuardingOrParrying(int damage, string damageType)
    {
        if (!isBlocking && !isParrying)
        {
            EnemyHealth health = GetComponent<EnemyHealth>();
            health.ReceiveDamage(damage, damageType);
        }
        else if (isParrying)
        {

        }

        

    }


}
