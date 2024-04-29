using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public string enemyType;
    public int health;

    SpriteRenderer spriteRenderer;
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeenHit(int damage, string damageType)
    {
        EnemyController controller = GetComponent<EnemyController>();
        controller.DefenceCheck(damage, damageType);
    }

    public void ReceiveDamage(int damage, string damageType)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("DarkKnightStagger"))
        {
            damage *= 3; // Takes the recieved damage and triples it if staggered
        }
        if (damage != 0 && damageType != "Blocked")
        {
            Debug.Log("Hit");
            health -= damage;
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
        spriteRenderer.color = Color.white;
    }
}
