using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] int armour;

    PlayerEnergy playerEnergy;

    // Start is called before the first frame update
    void Start()
    {
        playerEnergy = FindObjectOfType<PlayerEnergy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RechargeArmour()
    {
        if (armour < 3 && armour > 0 && playerEnergy.energy == 100)
        {
            armour++;
            playerEnergy.energy = 0;
        }
    }

    public void OnPlayerReceiveDamage(string damageType)
    {
        if (armour > 0)
        {
            armour--;
        }

    }
}
