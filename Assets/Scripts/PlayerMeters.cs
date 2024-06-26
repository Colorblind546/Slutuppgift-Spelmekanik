using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeters : MonoBehaviour
{
    [SerializeField] GameObject energyMeter;
    Vector3 energyMeterBasePos;
    [SerializeField] GameObject shieldMeter;
    Vector3 shieldMeterBasePos;
    [SerializeField] GameObject healthMeter;
    Vector3 healthMeterBasePos;

    GameObject player;
    PlayerEnergy playerEnergy;
    PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        energyMeterBasePos = energyMeter.transform.localPosition;
        shieldMeterBasePos = shieldMeter.transform.localPosition;
        healthMeterBasePos = healthMeter.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        if (player != null)
        {
            energyMeter.transform.localPosition = energyMeterBasePos + new Vector3(((playerEnergy.energy / 100f) * 3f) / 2f - 1.5f, 0, 0);
            energyMeter.transform.localScale = new Vector3((playerEnergy.energy / 100f) * 3f, 0.5f, 1);

            shieldMeter.transform.localPosition = shieldMeterBasePos + new Vector3(playerHealth.shield / 2f - 1.5f, 0, 0);
            shieldMeter.transform.localScale = new Vector3(playerHealth.shield, 0.4f, 1);

            healthMeter.transform.localPosition = healthMeterBasePos + new Vector3(((playerHealth.health) * 3) / 2 - 1f, 0, 0);
            healthMeter.transform.localScale = new Vector3((playerHealth.health) * 3f, 0.5f, 1);

        }
        

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerEnergy = player.GetComponent<PlayerEnergy>();
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }
}
