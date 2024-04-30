using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    TMP_Text fpsCount;
    float fpsRefreshCooldown;

    // Start is called before the first frame update
    void Start()
    {
        fpsCount = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        fpsRefreshCooldown -= Time.deltaTime;
        if (fpsCount != null && fpsRefreshCooldown <= 0f)
        {
            fpsCount.text = "FPS: " + Mathf.CeilToInt(1 / Time.deltaTime);
            fpsRefreshCooldown = 0.5f; // Cooldown So that framerate doesn't update every frame
        }
    }



}
