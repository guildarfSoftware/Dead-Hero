using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Time : MonoBehaviour
{
    [SerializeField] Text timeText;
    float elapsedTime = 0;
    private float oneSecondCounter;

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        oneSecondCounter += Time.deltaTime;

        if (oneSecondCounter > 1)
        {
            oneSecondCounter = 0;
            UpdateText(elapsedTime);
        }
    }

    private void UpdateText(float elapsedTime)
    {
        TimeSpan elapsedTimeSpan = TimeSpan.FromSeconds(elapsedTime);

        string formatedTime = $"{elapsedTimeSpan.Minutes}:{elapsedTimeSpan.Seconds:00}";

        timeText.text = formatedTime;
    }
}
