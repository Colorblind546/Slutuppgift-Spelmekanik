using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public string enemyType;
    [SerializeField] int health;

    SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.magenta;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeenHit(int damage, string damageType)
    {
        EnemyController controller = GetComponent<EnemyController>();
        controller.IsGuardingOrParrying(damage, damageType);
    }

    public void ReceiveDamage(int damage, string damageType)
    {
        if (damage != 0 && damageType != "Blocked")
        {
            Debug.Log("Hit");
            spriteRenderer.color = Color.red;
            Invoke("ResetColor", 0.2f);
        }
        else if (damage == 0 && damageType == "Blocked")
        {
            spriteRenderer.color = Color.cyan;
            Invoke("ResetColor", 0.2f);
        }
        
    }

    void ResetColor()
    {
        spriteRenderer.color = Color.magenta;
    }
}
