using Photon.Pun;
using Suncheon.Player;
using Suncheon.WebData;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Suncheon.UI
{
    public class UI_ClubCreatePopUp : MonoBehaviour
    {
        [Header("그룹 이름")]
        [SerializeField] private TMP_InputField input_ClubName;

        [Header("그룹 규모")]
        [SerializeField] private Button btn_SamllGroup;
        [SerializeField] private TMP_Text text_SamllGroup;
        [SerializeField] private Button btn_MiddleGroup;
        [SerializeField] private TMP_Text text_MiddleGroup;
        [SerializeField] private bool isSamllGroup = true;

        [Space(10)]

        [Header("그룹 인원")]
        [SerializeField] private Button btn_Min;
        [SerializeField] private Button btn_Max;
        [SerializeField] private Button btn_PlusCnt;
        [SerializeField] private Button btn_MinusCnt;

        [SerializeField] private int nMemberMin = 1;
        [SerializeField] private int nMemberMax = 5;
        [SerializeField] private int nMemberCnt = 1;
        [SerializeField] private TMP_InputField input_MemberCnt;

        [Space(10)]

        [Header("동아리방 색")]
        [SerializeField] private Button btn_Color01;
        [SerializeField] private Button btn_Color02;
        [SerializeField] private Button btn_Color03;

        [SerializeField] private Image image_SampleClub;

        [SerializeField] private Sprite[] sprite_SamllGroups;
        [SerializeField] private Sprite[] sprite_MiddleGroups;

        [SerializeField] private int nColorNum = 1;

        [Space(10)]

        [Header("동아리 생성")]
        [SerializeField] private Button btn_ClubCreate;

        private void Awake()
        {
            // 그룹 규모
            if (btn_SamllGroup != null) btn_SamllGroup.onClick.AddListener(() => SamllGroupBtnClick());
            if (btn_MiddleGroup != null) btn_MiddleGroup.onClick.AddListener(() => MiddleGroupBtnClick());

            // 그룹 인원수
            if (btn_Min != null) btn_Min.onClick.AddListener(() => MinBtnClick());
            if (btn_Max != null) btn_Max.onClick.AddListener(() => MaxBtnClick());
            if (btn_PlusCnt != null) btn_PlusCnt.onClick.AddListener(() => PlusCntBtnClick());
            if (btn_MinusCnt != null) btn_MinusCnt.onClick.AddListener(() => MinusCntBtnClick());
            if (input_MemberCnt != null) input_MemberCnt.text = nMemberCnt.ToString();

            // 동아리방 색
            if (btn_Color01 != null) btn_Color01.onClick.AddListener(() => Color01BtnClick());
            if (btn_Color02 != null) btn_Color02.onClick.AddListener(() => Color02BtnClick());
            if (btn_Color03 != null) btn_Color03.onClick.AddListener(() => Color03BtnClick());

            // 동아리 생성
            if (btn_ClubCreate != null) btn_ClubCreate.onClick.AddListener(() => ClubCreateBtnClick());
        }

        private void OnEnable()
        {
            Response_JoinedGroupListResultData joinedClubData = NetworkManager.Instance.Go_Player.GetPhotonView().gameObject.GetComponent<PlayerManager>().joinedClubData;
            if (joinedClubData == null)
            {
                input_ClubName.text = string.Empty;
                input_MemberCnt.text = "0";
                Color01BtnClick();
            }
            else
            {
                input_ClubName.text = joinedClubData.clubName;
                nMemberCnt = joinedClubData.userLimit;
                input_MemberCnt.text = nMemberCnt.ToString();

                if (joinedClubData.clubTemplateCode == "1")
                {
                    Color01BtnClick();
                }
                else if (joinedClubData.clubTemplateCode == "2")
                {
                    Color02BtnClick();
                }
                else if (joinedClubData.clubTemplateCode == "3")
                {
                    Color03BtnClick();
                }

                if (joinedClubData.clubType == "S")
                {
                    SamllGroupBtnClick();
                    
                    isSamllGroup = true;
                }
                else
                {
                    MiddleGroupBtnClick();
                    isSamllGroup = false;
                }
            }
        }

        private void SamllGroupBtnClick()
        {
            UTILS.Log("SamllGroupBtnClick");
            isSamllGroup = true;
            nMemberMax = 5;

            Color color = Color.white;
            btn_SamllGroup.GetComponent<Image>().color = color;
            text_MiddleGroup.color = color;
            color.a = 0;
            btn_MiddleGroup.GetComponent<Image>().color = color;
            text_SamllGroup.color = color;

            SampleClubImageChange();

            if(nMemberCnt > nMemberMax)
            {
                MaxBtnClick();
            }
        }

        private void MiddleGroupBtnClick()
        {
            UTILS.Log("MiddleGroupBtnClick");
            isSamllGroup = false;
            nMemberMax = 8;

            Color color = Color.white;
            btn_MiddleGroup.GetComponent<Image>().color = color;
            text_SamllGroup.color = color;
            color.a = 0;
            btn_SamllGroup.GetComponent<Image>().color = color;
            text_MiddleGroup.color = color;

            SampleClubImageChange();
        }

        private void MinBtnClick()
        {
            nMemberCnt = nMemberMin;
            input_MemberCnt.text = nMemberCnt.ToString();
        }

        private void MaxBtnClick()
        {
            if (isSamllGroup)
            {
                nMemberMax = 5;
            }
            else
            {
                nMemberMax = 8;
            }

            nMemberCnt = nMemberMax;
            input_MemberCnt.text = nMemberCnt.ToString();
        }

        private void MinusCntBtnClick()
        {
            nMemberCnt--;

            if (nMemberCnt < nMemberMin)
            {
                nMemberCnt = nMemberMin;
            }

            input_MemberCnt.text = nMemberCnt.ToString();
        }

        private void PlusCntBtnClick()
        {
            nMemberCnt++;

            if (nMemberCnt > nMemberMax)
            {
                nMemberCnt = nMemberMax;
            }

            input_MemberCnt.text = nMemberCnt.ToString();
        }

        private void Color01BtnClick()
        {
            nColorNum = 1;
            btn_Color01.GetComponent<ButtonControl>().OnChangeSprite(true);
            btn_Color02.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_Color03.GetComponent<ButtonControl>().OnChangeSprite(false);
            SampleClubImageChange();
        }
        private void Color02BtnClick()
        {
            nColorNum = 2;
            btn_Color01.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_Color02.GetComponent<ButtonControl>().OnChangeSprite(true);
            btn_Color03.GetComponent<ButtonControl>().OnChangeSprite(false);
            SampleClubImageChange();
        }
        private void Color03BtnClick()
        {
            nColorNum = 3;
            btn_Color01.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_Color02.GetComponent<ButtonControl>().OnChangeSprite(false);
            btn_Color03.GetComponent<ButtonControl>().OnChangeSprite(true);
            SampleClubImageChange();
        }

        private void SampleClubImageChange()
        {
            image_SampleClub.sprite = isSamllGroup ? sprite_SamllGroups[nColorNum - 1] : sprite_MiddleGroups[nColorNum - 1];
        }

        private void ClubCreateBtnClick()
        {
            if (string.IsNullOrEmpty(input_ClubName.text) || nMemberCnt < nMemberMin || nMemberCnt > nMemberMax)
            {
                UIInteractionManager.Instance.ShowSystemPopUp("동아리 설정 값을 확인해주세요.");
                return;
            }

            NetworkManager.Instance.ClubName = input_ClubName.text;
            StartCoroutine(CoroutineClubCreate());
        }

        IEnumerator CoroutineClubCreate()
        {
            yield return null;

            Response_JoinedGroupListResultData joinedClubData = NetworkManager.Instance.Go_Player.GetPhotonView().gameObject.GetComponent<PlayerManager>().joinedClubData;
            if (joinedClubData == null || string.IsNullOrEmpty(joinedClubData.clubName))
            {
                Request_CreateClubMng request_CreateClubMng = new Request_CreateClubMng(input_ClubName.text, isSamllGroup ? "S" : "M", int.Parse(input_MemberCnt.text), nColorNum);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.createClubUrl}", request_CreateClubMng, (jsonData) =>
                {
                    Response_ReturnMsg response_CreateClub = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                    if (response_CreateClub == null) return;

                    if(response_CreateClub.rtnCode == "999")
                    {
                        UIInteractionManager.Instance.ShowSystemPopUp($"{response_CreateClub.rtnMsg}");
                    }
                    else
                    {
                        NetworkManager.Instance.SetSpawnerPos(isSamllGroup ? SpawnerPos.소그룹동아리 : SpawnerPos.중그룹동아리);
                        UTILS.LoadingSceneLoad("03_Inside");
                    }
                }));
            }
            else
            {
                Request_UpdateClubMng request_UpdateClubMng = new Request_UpdateClubMng(joinedClubData.groupSeq.ToString(), input_ClubName.text, isSamllGroup ? "S" : "M", int.Parse(input_MemberCnt.text), nColorNum);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.updateClubInfoUrl}", request_UpdateClubMng, (jsonData) =>
                {
                    Response_ReturnMsg response_UpdateClubMng = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                    if (response_UpdateClubMng == null) return;

                    if(response_UpdateClubMng.rtnMsg == "000")
                    {
                        UIInteractionManager.Instance.ShowSystemPopUp($"{response_UpdateClubMng.rtnMsg}");
                    }
                    else
                    {
                        NetworkManager.Instance.SetSpawnerPos(isSamllGroup ? SpawnerPos.소그룹동아리 : SpawnerPos.중그룹동아리);
                        UTILS.LoadingSceneLoad("03_Inside");
                    }
                }));
            }            
        }
    }
}
