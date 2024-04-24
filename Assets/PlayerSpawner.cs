using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    [SerializeField] GameObject playerPrefab;
    GameObject activePlayer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P) && activePlayer == null)
        {
            activePlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }
        



    }
}
