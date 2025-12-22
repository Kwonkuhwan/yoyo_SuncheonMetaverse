using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Suncheon.UI
{
    public class UI_MapInteraction : MonoBehaviour
    {
        [SerializeField] private GameObject go_Char;
        [SerializeField] private Button[] btn_Libs;
        [SerializeField] private Transform[] lib_Positions;

        private void Awake()
        {

        }

        private void Start()
        {
            go_Char = GameObject.FindWithTag("Player");
        }

        private void OnTeleportBtnClick()
        {

        }
    }
}