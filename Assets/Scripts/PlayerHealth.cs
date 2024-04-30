using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int shield;

    // Player rendering related variables
    SpriteRenderer playerRenderer;


    // PlayerController ralated variables
    PlayerController playerController;

    PlayerEnergy playerEnergy;

    // Start is called before the first frame update
    void Start()
    {
        playerEnergy = GetComponent<PlayerEnergy>();
        playerController = GetComponent<PlayerController>();
        playerRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -50)
        {
            OnPlayerReceiveDamage("Unblockable", null);
        }


        if (health <= 0)
        {
            Die();
        }
    }

    public void RechargeArmour()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (shield < 3 && playerEnergy.energy == 100)
            {
                shield++;
                playerEnergy.energy = 0;
            }
        }
        
    }

    public void OnPlayerReceiveDamage(string damageType, GameObject attackOrigin)
    {
        EnemyController enemyController = attackOrigin.GetComponent<EnemyController>();
        

        if (playerController.isBlocking && playerEnergy.UseEnergy(20) && damageType != "Unblockable")
        {
            playerRenderer.color = Color.cyan;
            Invoke("ResetColor", 0.2f);
            playerController.BlockedAnAttack();
        }
        else if (playerController.isParrying)
        {
            enemyController.Staggered();
            playerEnergy.RechargeEnergy(100);
        }
        else if (shield <= 0)
        {
            health--;
            playerRenderer.color = Color.red;
            Invoke("ResetColor", 0.1f);
        }
        else if (shield > 0)
        {
            shield--;
            playerRenderer.color = Color.red;
            Invoke("ResetColor", 0.1f);
        }
        


    }

    public void ResetColor()
    {
        playerRenderer.color = Color.white;
    }

    private void Die()
    {
        Destroy(gameObject, 0);
    }

}
