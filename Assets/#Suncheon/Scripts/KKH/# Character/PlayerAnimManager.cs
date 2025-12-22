using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Suncheon.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suncheon.Player
{
    public class PlayerAnimManager : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private PhotonView pv;
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerManager playerManager;

        [Space(10)]

        [SerializeField] private bool isDacne;
        public bool IsDance => isDacne;
        [SerializeField] private bool isHello;
        public bool IsHello => isHello;
        [SerializeField] private bool isSmlie;
        public bool IsSmlie => isSmlie;
        [SerializeField] private bool isSad;
        public bool IsSad => isSad;

        [SerializeField] private bool isVehicle;
        public bool IsVehicle => isVehicle;

        [SerializeField] private bool isSitIdle;
        public bool IsSitIdle => isSitIdle;

        private static readonly int _Walk = Animator.StringToHash("Walk");
        private static readonly int _Idle = Animator.StringToHash("Idle");
        private static readonly int _Dance = Animator.StringToHash("Dance");
        private static readonly int _Hello = Animator.StringToHash("Hello");
        private static readonly int _Smlie = Animator.StringToHash("Smlie");
        private static readonly int _Sad = Animator.StringToHash("Sad");
        private static readonly int _Idle_Sit = Animator.StringToHash("Idle_Sit");

        void Start()
        {
            SetIdle();
        }

        public void SetWalk()
        {
            if (isVehicle) return;

            AllAnimTriggerReset();
            animator.SetTrigger(_Walk);
        }

        public void SetIdle()
        {
            if (isVehicle) return;

            AllAnimTriggerReset();
            animator.SetTrigger(_Idle);
        }


        public void SetBookSit(bool isOn)
        {
            AllAnimTriggerReset();
            if (!isOn)
            {
                isVehicle = false;
                SetIdle();
            }
            else
            {
                isVehicle = true;
            }

            animator.SetBool("BookSit", isVehicle);
        }

        public void SetIdle_Sit(bool isOn)
        {
            AllAnimTriggerReset();
            if(!isOn)
            {
                isSitIdle = false;
            }
            else
            {
                isSitIdle = true;
            }

            animator.SetBool(_Idle_Sit, isSitIdle);
        }

        public void SetDance()
        {
            if (isVehicle)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"탈것 이용시 캐릭터 감정표현이 불가능합니다.");
                return;
            }

            AllAnimTriggerReset();
            isDacne = true;

            animator.SetTrigger(_Dance);
        }

        public void SetHello()
        {
            if (isVehicle)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"탈것 이용시 캐릭터 감정표현이 불가능합니다.");
                return;
            }

            AllAnimTriggerReset();
            isHello = true;

            animator.SetTrigger(_Hello);
        }

        public void SetSmlie()
        {
            if (isVehicle)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"탈것 이용시 캐릭터 감정표현이 불가능합니다.");
                return;
            }

            AllAnimTriggerReset();
            isSmlie = true;

            animator.SetTrigger(_Smlie);
        }

        public void SetSad()
        {
            if (isVehicle)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"탈것 이용시 캐릭터 감정표현이 불가능합니다.");
                return;
            }

            AllAnimTriggerReset();
            isSad = true;

            animator.SetTrigger(_Sad);
        }

        public void AllAnimTriggerReset()
        {
            isDacne = false;
            isHello = false;
            isSmlie = false;
            isSad = false;
            isSitIdle = false;

            animator.ResetTrigger(_Idle);
            animator.ResetTrigger(_Walk);
            animator.ResetTrigger(_Idle);
            animator.ResetTrigger(_Dance);
            animator.ResetTrigger(_Hello);
            animator.ResetTrigger(_Smlie);
            animator.ResetTrigger(_Sad);
            animator.ResetTrigger(_Idle_Sit);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(IsDance);
                stream.SendNext(IsHello);
                stream.SendNext(IsSmlie);
                stream.SendNext(IsSad);
                stream.SendNext(IsVehicle);
                stream.SendNext(IsSitIdle);
            }
            else
            {
                isDacne = (bool)stream.ReceiveNext();
                isHello = (bool)stream.ReceiveNext();
                isSmlie = (bool)stream.ReceiveNext();
                isSad = (bool)stream.ReceiveNext();
                isVehicle = (bool)stream.ReceiveNext();
                isSitIdle = (bool)stream.ReceiveNext();

                if (isDacne || isHello || isSmlie || isSad) return;

                playerManager.VehicleOnOff(IsVehicle);
                SetBookSit(IsVehicle);
            }
        }
    }
}
