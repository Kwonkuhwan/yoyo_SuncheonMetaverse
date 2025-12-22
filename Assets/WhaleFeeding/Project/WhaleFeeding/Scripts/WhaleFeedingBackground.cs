using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhaleFeedingBackground : MonoBehaviour
{
    private Material mat_BG;
    private float offsetvalue;
    private float speed = 0.1f;

    void Awake()
    {
        mat_BG = GetComponent<Image>().material;
        WhaleFeedingGame.GameStart += Init;
        WhaleFeedingGame.GameEnd += Init;
    }

    private void OnDestroy()
    {
        WhaleFeedingGame.GameStart -= Init;
        WhaleFeedingGame.GameEnd -= Init;
    }

    void Update()
    {
        offsetvalue += speed * Time.deltaTime;
        mat_BG.SetTextureOffset("_MainTex", new Vector2(offsetvalue, 0));
    }

    void Init()
    {
        offsetvalue = 0;
        mat_BG.SetTextureOffset("_MainTex", new Vector2(offsetvalue, 0));
    }
}
