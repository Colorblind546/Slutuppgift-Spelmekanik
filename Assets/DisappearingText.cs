using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingText : MonoBehaviour
{
    new Renderer renderer;
    MeshRenderer meshRenderer;
    [SerializeField] float disappearTimer;

    [SerializeField] string textType;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        switch (textType)
        {
            case "Timer":
                {
                    if (renderer.isVisible)
                    {
                        disappearTimer -= Time.fixedDeltaTime;
                    }

                    if (disappearTimer <= 0)
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                }
            case "WhenDead":
                {
                    if (GameObject.FindGameObjectWithTag("Player") != null)
                    {
                        meshRenderer.enabled = false;
                    }
                    else
                    {
                        meshRenderer.enabled = true;
                    }
                    break;
                }

        }



        
    }
}
