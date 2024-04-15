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
        Debug.Log("Hit");
        spriteRenderer.color = Color.red;
    }
}
