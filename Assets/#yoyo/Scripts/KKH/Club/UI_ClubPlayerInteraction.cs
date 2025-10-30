using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Suncheon.WebData;
using Photon.Pun;

namespace Suncheon.UI
{
    public class UI_ClubPlayerInteraction : UI_PlayerInteraction
    {
        protected YesBtnDelegate yesBtnDelegate;
        protected NoBtnDelegate noBtnDelegate;

        [SerializeField] private Button btn_Kick;               // 동아리 내보내기

        protected override void Awake()
        {
            base.Awake();
            btn_CutOff.onClick.AddListener(() => Btn_CutOff());
            btn_Kick.onClick.AddListener(() => Btn_Kick());
        }


        void Btn_CutOff()
        {
            Btn_Kick();
        }

        void Btn_Kick()
        {
            UIInteractionManager_Inside inter = (UIInteractionManager_Inside)UIInteractionManager.Instance;
            inter.KickBtnClick(NetworkManager.Instance.user_name);
        }       
    }
}