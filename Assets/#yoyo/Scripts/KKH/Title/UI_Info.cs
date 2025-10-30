using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Info : MonoBehaviour
{
    [SerializeField] Toggle toggle_Info;
    [SerializeField] Button btn_Info;

    private void OnEnable()
    {
        toggle_Info.isOn = false;
        btn_Info.interactable = false;
    }

    private void Awake()
    {
        toggle_Info.onValueChanged.AddListener(delegate { ToggleInfoClick(); });
    }

    private void ToggleInfoClick()
    {
        if (toggle_Info.isOn)
        {
            btn_Info.interactable = true;
        }
        else
        {
            btn_Info.interactable = false;
        }
    }
}
