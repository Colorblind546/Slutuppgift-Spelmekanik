using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [SerializeField] int energy;

    


    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<PlayerController>().jumpEvent.AddListener(Jumped);
    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    void Jumped()
    {

    }
}
