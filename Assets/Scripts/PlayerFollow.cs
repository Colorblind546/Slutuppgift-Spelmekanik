using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{

    GameObject player;
    [SerializeField] float offsetY;

    LayerMask playerLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            FindPlayer();
        }


        if (player != null)
        {
            transform.position = player.transform.position - (Vector3.forward * 10) + (Vector3.up * offsetY);
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
