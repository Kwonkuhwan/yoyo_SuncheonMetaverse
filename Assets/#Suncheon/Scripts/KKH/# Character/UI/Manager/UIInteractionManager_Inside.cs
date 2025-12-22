using Newtonsoft.Json.Linq;
using Photon.Pun;
using Suncheon.Player;
using Suncheon.WebData;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class UIInteractionManager_Inside : UIInteractionManager
    {
        [Space(10)]
        [Header("동아리 관련")]
        #region 동아리관련
        [Header("Panel")]
        [SerializeField] protected GameObject ui_ClubMembers;
        [SerializeField] protected GameObject ui_ClubMeetingSetting;

        public GameObject ui_ClubInvitationCode;
        public GameObject ui_ClubPlayerInteraction;

        [Header("Button")]
        [SerializeField] protected Button btn_ClubInvitationCode;
        [SerializeField] protected Button btn_ClubMembers;
        [SerializeField] protected Button btn_MeetingSetting;
        [SerializeField] protected Button btn_MeetingStart;
        #endregion

        [SerializeField] private Transform[] tr_clubSitPos_10;
        [SerializeField] private Transform[] tr_clubSitPos_15;

        [Space(10)]
        [Header("나가기")]
        [SerializeField] protected Button btn_Exit;

        UI_YesNoPopUp popUp;

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;

            //if (btn_ClubInvitationCode != null) btn_ClubInvitationCode.onClick.AddListener(() => Btn_ClubInvitationCodeClick());
            if (btn_ClubMembers != null) btn_ClubMembers.onClick.AddListener(() => Btn_ClubMembersClick());
            if (btn_MeetingSetting != null) btn_MeetingSetting.onClick.AddListener(() => Btn_MeetingSettingClick());
            if (btn_MeetingStart != null) btn_MeetingStart.onClick.AddListener(() => Btn_MeetingStartClick());
            if (btn_Exit != null) btn_Exit.onClick.AddListener(() => Btn_ExitClick());
        }

        private void ResultClipBoardReturn(bool isResult)
        {
            if(isResult)
            {
                StartCoroutine(ClubInvitationCodeShow());
            }
        }

        private IEnumerator ClubInvitationCodeShow()
        {
            ui_ClubInvitationCode.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            ui_ClubInvitationCode.SetActive(false);
        }

        public void Btn_ClubInvitationCodeClick()
        {
            UTILS.Log($"Btn_ClubInvitationCodeClick");
            UTILS.Log($"{NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().joinedClubData.clubInviteCode}");

            UTILS.CopyToClipBoard(NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().joinedClubData.clubInviteCode);
            StartCoroutine(ClubInvitationCodeShow());
        }

        public void Btn_ClubMembersClick()
        {
            ui_ClubMembers.GetComponent<UI_ClubMembers>().ShowPopUp();
        }

        public void Btn_MeetingSettingClick()
        {
            UTILS.Log($"Btn_MeetingSettingClick");
            ui_ClubMeetingSetting.GetComponent<UI_ClubMeetingSetting>().ShowPopUp();
        }

        public void Btn_MeetingStartClick()
        {
            TMP_Text text = btn_MeetingStart.GetComponentInChildren<TMP_Text>();
            
            if (NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().isClub_Meeting)
            {
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().isClub_Meeting = false;
                NetworkManager.Instance.Go_Player.GetComponent<PlayerAnimManager>().SetIdle();

                NetworkManager.Instance.Go_Player.GetComponent<CapsuleCollider>().center = new Vector3(0.0f, 0.8f, 0.0f);
                NetworkManager.Instance.Go_Player.GetComponent<CapsuleCollider>().height = 1.6f;

                if (text != null) text.text = "회의 시작";
            }
            else
            {
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().isClub_Meeting = true;

                Response_JoinedGroupListResultData joinedClubData = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().joinedClubData;

                Request_ClubPlayerCnt requset_ClubPlayerCnt = new Request_ClubPlayerCnt(joinedClubData.groupSeq);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.getClubPeopleInfoUrl}", requset_ClubPlayerCnt, (jsonData) =>
                {
                    JArray jArray = JArray.Parse(jsonData);
                    Response_ClubPlayerCnt response_ClubPlayerCnt = new Response_ClubPlayerCnt();

                    NetworkManager.Instance.Go_Player.GetComponent<CapsuleCollider>().center = new Vector3(0.0f, 0.7f, 0.0f);
                    NetworkManager.Instance.Go_Player.GetComponent<CapsuleCollider>().height = 0.8f;

                    if (joinedClubData.nickname == PhotonNetwork.NickName)
                    {
                        if (joinedClubData.clubType == "S")
                        {
                            NetworkManager.Instance.Go_Player.transform.position = tr_clubSitPos_10[0].position;
                        }
                        else
                        {
                            NetworkManager.Instance.Go_Player.transform.position = tr_clubSitPos_15[0].position;
                        }

                        NetworkManager.Instance.Go_Player.GetComponent<PlayerAnimManager>().SetIdle_Sit(true);
                        return;
                    }

                    foreach (JObject jObject in jArray)
                    {
                        response_ClubPlayerCnt.response_ClubPlayerCntResultData.Add(JsonUtility.FromJson<Response_ClubPlayerCntResultData>(jObject.ToString()));
                    }

                    for (int i = 0; i < response_ClubPlayerCnt.response_ClubPlayerCntResultData.Count; i++)
                    {
                        int index = i + 1;

                        if (response_ClubPlayerCnt.response_ClubPlayerCntResultData[i].nickname == PhotonNetwork.NickName)
                        {                   
                            if (joinedClubData.clubType == "S")
                            {
                                if (index == 1 || index == 3 || index == 5)
                                {
                                    NetworkManager.Instance.Go_Player.transform.Rotate(new Vector3(0, 180, 0));
                                }
                                NetworkManager.Instance.Go_Player.transform.position = tr_clubSitPos_10[index].position;
                            }
                            else
                            {
                                NetworkManager.Instance.Go_Player.GetComponent<CapsuleCollider>().center = new Vector3(0.0f, 0.7f, 0.0f);
                                NetworkManager.Instance.Go_Player.GetComponent<CapsuleCollider>().height = 0.8f;

                                if (index == 1 || index == 3 || index == 5 || index == 7)
                                {
                                    NetworkManager.Instance.Go_Player.transform.Rotate(new Vector3(0, 180, 0));
                                }
                                NetworkManager.Instance.Go_Player.transform.position = tr_clubSitPos_15[index].position;
                            }
                            NetworkManager.Instance.Go_Player.GetComponent<PlayerAnimManager>().SetIdle_Sit(true);
                        }
                    }
                }));

                if (text != null) text.text = "회의 종료";
            }
            UTILS.Log($"Btn_MeetingStartClick");
        }

        public void ClubPlayerInteractionOn(string playerName, Vector3 pos)
        {
            ui_ClubPlayerInteraction.GetComponent<UI_ClubPlayerInteraction>().SetName(playerName);

            ui_ClubPlayerInteraction.transform.localPosition = GetCanvasLocalPos(pos);

            CanvasGroup cg = ui_ClubPlayerInteraction.GetComponent<CanvasGroup>();
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        public void ClubPlayerInteractionOff()
        {
            CanvasGroup cg = ui_ClubPlayerInteraction.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        private void KickYesButtonClick()
        {
            PlayerManager playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.kickClubUrl}", $"regId={NetworkManager.Instance.user_id}&groupSeq={playerManager.joinedClubData.groupSeq}", (jsonData) =>
            {
                Response_ReturnMsg response_KickGroup = null;
                try
                {
                    response_KickGroup = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    response_KickGroup = null;
                }
                if (response_KickGroup == null) return;

                UIInteractionManager_Inside uiinter = (UIInteractionManager_Inside)UIInteractionManager.Instance;
                uiinter.ui_SystemPopUp.GetComponent<UI_SystemPopUp>().ShowPopUp(response_KickGroup.rtnMsg);

                Player_Remove();

                uiinter.ui_ClubMembers.GetComponent<UI_ClubMembers>().ListUpdate();

                ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().HidePopUp();
            }));
        }

        private void KickNoButtonClick()
        {
            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().HidePopUp();
        }

        public void KickBtnClick(string kickName)
        {
            yesBtnDelegate = new YesBtnDelegate(KickYesButtonClick);
            noBtnDelegate = new NoBtnDelegate(KickNoButtonClick);
            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);

            string strInfo = $"{kickName}님을(를) 동아리에서 내보내시겠습니까?";

            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp(strInfo);
        }

        private void ExitYesButtonClick()
        {
            if (NetworkManager.Instance.LibPos == LibName.None)
            {
                NetworkManager.Instance.SetSpawnerPos(SpawnerPos.오천그린광장);
            }

            NetworkManager.Instance.OnLeaveRoom();
            UTILS.LoadingSceneLoad("02_Garden_Scene");
        }

        private void ExitNoButtonClick()
        {
            //popUp = FindAnyObjectByType<UI_YesNoPopUp>();
            //popUp.gameObject.SetActive(false);

            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().HidePopUp();

            return;
        }

        public void Btn_ExitClick()
        {
            yesBtnDelegate = new YesBtnDelegate(ExitYesButtonClick);
            noBtnDelegate = new NoBtnDelegate(ExitNoButtonClick);

            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            string strInfo = string.Empty;
            if(NetworkManager.Instance.SpPos == SpawnerPos.소그룹동아리 || NetworkManager.Instance.SpPos == SpawnerPos.중그룹동아리)
            {
                strInfo = "동아리에서 나가시겠습니까?\n광장으로 이동됩니다.";
            }
            else
            {
                strInfo = "광장으로 나가시겠습니까?";
            }

            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp(strInfo);           
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals("02_Garden_Scene"))
            {
            }
            else if (scene.name.Equals("04_Myroom"))
            {
            }
            else if (scene.name.Equals("03_Inside") || scene.name.Equals("05_AVRoom"))
            {
                if (btn_MyLibrary != null) btn_MyLibrary.gameObject.SetActive(false);
                if (btn_MiniGame != null) btn_MiniGame.gameObject.SetActive(false);
                if (btn_Vehicle != null) btn_Vehicle.gameObject.SetActive(false);
                if (btn_Club != null) btn_Club.gameObject.SetActive(false);

                if(NetworkManager.Instance.SpPos == SpawnerPos.소그룹동아리 || NetworkManager.Instance.SpPos == SpawnerPos.중그룹동아리)
                {
                    if (btn_ClubMembers != null) btn_ClubMembers.gameObject.SetActive(true);
                    if (btn_MeetingSetting != null) btn_MeetingSetting.gameObject.SetActive(true);
                    if (btn_MeetingStart != null) btn_MeetingStart.gameObject.SetActive(true);
                    //if (btn_ClubInvitationCode != null) btn_ClubInvitationCode.gameObject.SetActive(true);
                }
                else
                {
                    if (btn_ClubMembers != null) btn_ClubMembers.gameObject.SetActive(false);
                    if (btn_MeetingSetting != null) btn_MeetingSetting.gameObject.SetActive(false);
                    if (btn_MeetingStart != null) btn_MeetingStart.gameObject.SetActive(false);
                    //if (btn_ClubInvitationCode != null) btn_ClubInvitationCode.gameObject.SetActive(false);
                }
            }
        }

        public void Player_Remove()
        {
            PhotonView pv = NetworkManager.Instance.Go_Player.GetPhotonView();
            pv.RPC("RPCLibKickPlayer", RpcTarget.Others, NetworkManager.Instance.user_name);
        }
    }
}