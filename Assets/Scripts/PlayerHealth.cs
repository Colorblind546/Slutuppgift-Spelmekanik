using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] int armour;

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
        if (health <= 0)
        {
            Die();
        }
    }

    public void RechargeArmour()
    {
        if (armour < 3 && armour > 0 && playerEnergy.energy == 100)
        {
            armour++;
            playerEnergy.energy = 0;
        }
    }

    public void OnPlayerReceiveDamage(string damageType, GameObject attackOrigin)
    {
        EnemyController enemyController = attackOrigin.GetComponent<EnemyController>();
        

        if (playerController.isBlocking && playerEnergy.UseEnergy(20))
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
        else if (armour <= 0)
        {
            health--;
            playerRenderer.color = Color.red;
            Invoke("ResetColor", 0.1f);
        }
        else if (armour > 0)
        {
            armour--;
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
        Destroy(gameObject, 3);
    }

}
