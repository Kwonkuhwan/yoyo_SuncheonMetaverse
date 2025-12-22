using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Suncheon.Player;
using Suncheon.WebData;
using Newtonsoft.Json.Linq;

namespace Suncheon.UI
{
    public class UI_ClubPopUp : MonoBehaviour
    {
        [SerializeField] private GameObject ui_ClubCreatePopUp;
        [SerializeField] private TMP_Text text_Info;
        [SerializeField] private Button btn_ExistingClubUse;
        [SerializeField] private Button btn_NewClubCreate;

        private YesBtnDelegate yesBtnDelegate;
        private NoBtnDelegate noBtnDelegate;

        private void Awake()
        {
            if (btn_ExistingClubUse != null) btn_ExistingClubUse.onClick.AddListener(() => ExstingClubUseClick());
            if (btn_NewClubCreate != null) btn_NewClubCreate.onClick.AddListener(() => NewClubCreateClick());

            yesBtnDelegate = new YesBtnDelegate(YesButtonClick);
            noBtnDelegate = new NoBtnDelegate(NoButtonClick);
        }

        public void ShowPopUp(string info)
        {
            text_Info.text = info;
            UIInteractionManager.Instance.OpenPopUp(gameObject);
        }

        private void ExstingClubUseClick()
        {
            // [24.02.16] [수정] KKH : 기존 동아리 선택시 바로 내부씬 동아리실로 이동
            Response_JoinedGroupListResultData joinClubData = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().joinedClubData;
            if(joinClubData != null)
            {
                UTILS.Log("ExstingClubUseClick");
                NetworkManager.Instance.ClubName = joinClubData.clubName;
                NetworkManager.Instance.SetSpawnerPos(joinClubData.clubType == "S" ? SpawnerPos.소그룹동아리 : SpawnerPos.중그룹동아리);
                UTILS.LoadingSceneLoad("03_Inside");
            }
            else
            {
                UI_SystemPopUp uI_SystemPopUp = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().UIInteraction.ui_SystemPopUp.GetComponent<UI_SystemPopUp>();
                uI_SystemPopUp.ShowPopUp("현재 가입된 동아리가 없습니다.");
            }            
        }

        private void NewClubCreateClick()
        {
            NewClubCreate();
        }

        private void NewClubCreate()
        {
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            UIInteractionManager.Instance.ShowYesNoPopUp($"신규 동아리 생성 시\n기존 동아리는 사라집니다.\n계속하시겠습니까?");
        }

        private void YesButtonClick()
        {
            UIInteractionManager.Instance.ClosePopUp(UIInteractionManager.Instance.ui_YesNoPopUp);

            // [23.11.00] [작성] KKH : 기존 동아리 선택시 바로 내부씬 동아리실로 이동
            Response_JoinedGroupListResultData joinedClubData = NetworkManager.Instance.Go_Player.GetPhotonView().gameObject.GetComponent<PlayerManager>().joinedClubData;
            UI_SystemPopUp uI_SystemPopUp = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().UIInteraction.ui_SystemPopUp.GetComponent<UI_SystemPopUp>();

            // [23.11.00] [작성] KKH : 동아리장 비교
            if (joinedClubData.nickname == PhotonNetwork.NickName)
            {
                Request_ClubPlayerCnt requset_ClubPlayerCnt = new Request_ClubPlayerCnt(joinedClubData.groupSeq);
                // [23.11.00] [작성] KKH : 동아리 인원 체크
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.getClubPeopleInfoUrl}", requset_ClubPlayerCnt, (jsonData) =>
                {
                    JArray jArray = new JArray(jsonData);

                    // [23.11.00] [작성] KKH : 그룹원이 1명이라도 있다면 탈퇴 불가
                    if (jArray.Count > 1)
                    {
                        uI_SystemPopUp.ShowPopUp("그룹장(은)는 동아리 회원이 남아있을 경우\n동아리 탈퇴가 불가능합니다.");
                        UIInteractionManager.Instance.ClosePopUp(gameObject);
                        return;
                    }

                    // [23.11.00] [작성] KKH : 동아리 삭제
                    StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.deleteClubUrl}", $"groupSeq={joinedClubData.groupSeq}", (jsonData) =>
                    {
                        Response_ReturnMsg response_DeleteGroup = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                        if (response_DeleteGroup == null) return;

                        if (response_DeleteGroup.rtnCode == "000")
                        {
                            UIInteractionManager.Instance.OpenPopUp(UIInteractionManager.Instance.ui_ClubCreatePopUp);
                            NetworkManager.Instance.Go_Player.GetPhotonView().gameObject.GetComponent<PlayerManager>().joinedClubData = null;
                            UIInteractionManager.Instance.ClosePopUp(gameObject);
                            //NewClubCreate();
                        }
                        else
                        {
                            UTILS.Log($"NewClubCreate rtnCode : {response_DeleteGroup.rtnCode} {response_DeleteGroup.rtnMsg}");
                            uI_SystemPopUp.ShowPopUp($"{response_DeleteGroup.rtnMsg}");
                        }
                    }));
                }));
            }
            else
            {
                // [23.11.00] [작성] KKH : 동아리 탈퇴
                StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.quitClubUrl}", $"groupSeq={joinedClubData.groupSeq}", (jsonData) =>
                {
                    Response_ReturnMsg response_QuitGroup = null;
                    try
                    {
                        response_QuitGroup = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                    }
                    catch
                    {
                        response_QuitGroup = null;
                    }

                    if (response_QuitGroup == null) return;

                    if (response_QuitGroup.rtnCode == "000")
                    {
                        // [23.11.00] [작성] KKH : 정상 탈퇴 후 새 동아리 생성
                        UIInteractionManager.Instance.OpenPopUp(UIInteractionManager.Instance.ui_ClubCreatePopUp);
                        NetworkManager.Instance.Go_Player.GetPhotonView().gameObject.GetComponent<PlayerManager>().joinedClubData = null;
                        UIInteractionManager.Instance.ClosePopUp(gameObject);
                        //NewClubCreate();
                    }
                    else
                    {
                        UTILS.Log($"NewClubCreate rtnCode : {response_QuitGroup.rtnCode} {response_QuitGroup.rtnMsg}");
                        uI_SystemPopUp.ShowPopUp($"{response_QuitGroup.rtnMsg}");
                    }
                }));
            }
        }

        private void NoButtonClick()
        {
            UIInteractionManager.Instance.ClosePopUp(UIInteractionManager.Instance.ui_YesNoPopUp);
        }
    }
}
