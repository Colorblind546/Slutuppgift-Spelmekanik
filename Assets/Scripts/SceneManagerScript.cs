using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{

    [SerializeField] int sceneToBeLoaded;

    [SerializeField] Vector2 triggerSize;
    [SerializeField] LayerMask playerLayer;

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
        if (Physics2D.OverlapBox(transform.position, triggerSize, 0f, playerLayer))
        {
            SwitchToScene(sceneToBeLoaded);
        }
    }


    public void SwitchToScene(int sceneIndex)
    {
        Debug.Log("Loading Scene");
        SceneManager.LoadScene(sceneIndex);
    }
}
