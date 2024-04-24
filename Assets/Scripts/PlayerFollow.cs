using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{

    GameObject player;


    LayerMask playerLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position - (Vector3.forward * 10);
        }
        
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            FindPlayer();
        }
    }

    void FindPlayer()
    {
        try
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        catch
        {
        }
    }
}
