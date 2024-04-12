using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    public int energy;

    


    // Start is called before the first frame update
    void Start()
    {

    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RechargeEnergy(int rechargeAmount)
    {

    }

    public bool UseEnergy(int energyNeeded)
    {
        bool canUse = energy > energyNeeded;
        if (canUse)
        {
            energy -= energyNeeded;
        }
        return canUse;
    }
}
