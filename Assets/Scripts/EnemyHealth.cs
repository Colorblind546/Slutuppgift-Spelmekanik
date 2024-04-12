using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] string enemyType;
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


    public void ReceiveDamage(int damage, string damageType)
    {
        Debug.Log("Hit");
        spriteRenderer.color = Color.red;
    }
}
