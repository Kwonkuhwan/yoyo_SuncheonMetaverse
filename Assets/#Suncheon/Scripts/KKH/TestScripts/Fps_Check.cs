using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

public class Fps_Check : MonoBehaviour
{
    [SerializeField] private TMP_Text text_Fps;
    [SerializeField] private TMP_Text text_Memory;
    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        text_Fps.text = text;
        text_Memory.text = $"{GBCul(Profiler.usedHeapSizeLong)}";
    }

    private double GBCul(long bytelon)
    {
        double result = ((((double)bytelon / 1024) / 1024) / 1024);
        return Math.Round(result, 3);
    }
}
