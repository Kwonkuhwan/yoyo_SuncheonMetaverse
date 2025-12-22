using Photon.Pun;
using Photon.Realtime;
using Suncheon.UI;
using Suncheon.WebData;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Suncheon.Player
{
    public class PlayerPhotonManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] private PlayerAnimManager playerAnimManager;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PhotonView pv;
        [SerializeField] private string playerName;
        [SerializeField] private TMP_Text text_playerNickName;
        [SerializeField] private string avatarInfo;
        [SerializeField] private bool isGuest;

        IEnumerator showTalkBox;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
        }

        [PunRPC]
        private void SyncPlayerName(string nickName)
        {
            playerName = nickName;
            text_playerNickName.text = nickName;
        }

        [PunRPC]
        private void RPCSetAvatarInfo(string _avatarInfo)
        {
            avatarInfo = _avatarInfo;
            playerManager.CharacterSpawn(avatarInfo);
        }

        [PunRPC]
        private void RPCSetGuestInfo(bool _guestInfo)
        {
            isGuest = _guestInfo;
            playerManager.IsGuest = isGuest;
        }

        //[PunRPC]
        //private void RPCVehicleOnOff(bool isTrigger)
        //{
        //    IsVehicle = isTrigger;
        //    playerManager.VehicleOnOff(IsVehicle);
        //}

        [PunRPC]
        private void RPCShowTalkBox(string sender, string chat)
        {
            bool isblock = false;

            foreach (Response_MyBlockListResultData list in NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().blockPlayerList.response_MyBlockListResults)
            {
                if (sender == list.nickname)
                {
                    isblock = true;
                    break;
                }
            }

            UTILS.Log($"{isblock}");

            if(isblock) { return; }

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.GetPhotonView().Controller.NickName == sender)
                {
                    if (showTalkBox != null)
                    {
                        StopCoroutine(showTalkBox);
                        showTalkBox = null;
                    }
                    showTalkBox = player.GetComponent<PlayerManager>().ShowTalkBox(chat);
                    StartCoroutine(showTalkBox);
                    break;
                }
            }
        }

        [PunRPC]
        private void RPCReceivedNormalMessage(string sender, string msg)
        {
            UTILS.Log($"{sender} | {msg}");

            ChatManager.Instance.ReceivedNormalMessage(sender, msg);
        }

        [PunRPC]
        private void RPCReceivedWhisperingMessage(string sender, string receiver, string msg)
        {
            UTILS.Log($"{receiver} | {msg}");
            ChatManager.Instance.ReceivedWhisperingMessage(sender, receiver, msg);
        }

        [PunRPC]
        private void RPCReceivedSystemMessage(string msg)
        {
            UTILS.Log($"{"[공지]"} | {msg}");
            ChatManager.Instance.ReceivedSystemMessage(msg);
        }

        [PunRPC]
        private void RPCPrivateShowYesNoPopUp(string sender, string receiver, string info)
        {
            UTILS.Log($"{sender} | {receiver} | {info}");

            StartCoroutine(ShowYesNoPopUp(sender, receiver, info));
        }

        IEnumerator ShowYesNoPopUp(string sender, string receiver, string info)
        {
            yield return null;
            if (PhotonNetwork.NickName == receiver)
            {
                UIInteractionManager.Instance.ShowYesNoPopUp($"{info}");
            }
        }

        [PunRPC]
        private void RPCClubVitePopUp(string sender, string receiver, string clubName, int clubSeq)
        {
            UTILS.Log($"RPCClubVitePopUp : {sender} | {receiver} | {clubName}");
            StartCoroutine(ShowClubYesNoPopUp(receiver, $"[{sender}]님이 [{clubName}]에 당신을 초대합니다!", clubName, clubSeq));
        }


        IEnumerator ShowClubYesNoPopUp(string receiver, string info, string clubName, int clubSeq)
        {
            yield return null;
            if (PhotonNetwork.NickName == receiver)
            {
                UIInteractionManager.Instance.ShowClubViteYerNoPopUp($"{info}", clubName, clubSeq);
            }
        }

        [PunRPC]
        public void RPCLibKickPlayer(string receiver/*쫒겨나는 인간*/)
        {
            StartCoroutine(LibKickPlayer(receiver));
        }

        IEnumerator LibKickPlayer(string receiver)
        {
            yield return null;
            if (PhotonNetwork.NickName == receiver) // 이 오브젝트가 강퇴 대상인지 확인
            {
                NetworkManager.Instance.Return_To_Robby();
                //NetworkManager.Instance.OnLeaveRoom();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // 로컬 플레이어의 닉네임을 전송합니다.
                stream.SendNext(playerName);
                stream.SendNext(avatarInfo);
                stream.SendNext(isGuest);
            }
            else
            {
                // 다른 플레이어의 닉네임을 수신합니다.
                playerName = (string)stream.ReceiveNext();
                avatarInfo = (string)stream.ReceiveNext();
                isGuest = (bool)stream.ReceiveNext();

                text_playerNickName.text = playerName;
            }
        }

        [PunRPC]
        public void RPCClubMeetingShare(string meetingName, string url)
        {
            if (pv.IsMine)
            {
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().club_MeetingName = meetingName;
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().club_FileURL = url;
            }
        }
    }
}