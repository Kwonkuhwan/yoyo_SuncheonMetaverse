using Suncheon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameSelect : MonoBehaviour
{
    [SerializeField] private Button btn_30Frame;
    [SerializeField] private Button btn_60Frame;

    [SerializeField] private Sprite sprite_On;
    [SerializeField] private Sprite sprite_Off;

    private void Awake()
    { 
#if UNITY_EDITOR || UNITY_WEBGL
        gameObject.SetActive(false);
#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        btn_30Frame.onClick.AddListener(() => SetFrame(true));
        btn_30Frame.onClick.AddListener(() => Set30FrameSprite());

        btn_60Frame.onClick.AddListener(() => SetFrame(false));
        btn_60Frame.onClick.AddListener(() => Set60FrameSprite());

        if (GameManager.Instance.is30Frame)
        {
            Set30FrameSprite();
        }
        else
        {
            Set60FrameSprite();
        }
#endif
    }

    private void Set30FrameSprite()
    {
        btn_60Frame.GetComponent<Image>().sprite = sprite_Off;
        btn_30Frame.GetComponent<Image>().sprite = sprite_On;
    }

    private void Set60FrameSprite()
    {
        btn_30Frame.GetComponent<Image>().sprite = sprite_Off;
        btn_60Frame.GetComponent<Image>().sprite = sprite_On;
    }

    private void SetFrame(bool is30Frame)
    {
        GameManager.Instance.SetFrame(is30Frame);
    }
}
