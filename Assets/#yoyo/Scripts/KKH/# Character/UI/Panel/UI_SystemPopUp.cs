using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Suncheon.UI
{
    public class UI_SystemPopUp : MonoBehaviour
    {
        [SerializeField] private TMP_Text text_Info;
        [SerializeField] private Animation anim;

        public void ShowPopUp(string info)
        {
            text_Info.text = info;
            anim.Play();
        }
    }
}