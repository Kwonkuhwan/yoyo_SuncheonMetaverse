using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;


namespace Suncheon.UI
{
    public class UI_MapTeleportButton : MonoBehaviour
    {        
        [SerializeField] private GameObject panel_PopUp;
        [SerializeField] private LibName libName;
        [SerializeField] private ButtonControl buttonControl;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => OnClick());
            buttonControl = GetComponent<ButtonControl>();
        }

        public void OnClick()
        {
            buttonControl.OnBtnImageFadeOut();
            buttonControl.OnBackGroundFadeOut();

            UI_MapPopUp uI_PopUpManager = panel_PopUp.GetComponent<UI_MapPopUp>();
            uI_PopUpManager.SetInfoData(libName);
            UIInteractionManager.Instance.OpenPopUp(uI_PopUpManager.gameObject);
        }
    }
}
