using Photon.Pun;
using Suncheon.UI;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Suncheon.Player.Customization;
using System.Collections;

namespace Suncheon.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("포톤 관련")]
        [SerializeField] private PhotonView pv;
        public PhotonView Pv => pv;

        [Space(10)]
        public Response_LoginResultData loginData;

        [Space(10)]
        [Header("캐릭터 관련")]
        public bool IsGuest = false;

        #region 캐릭터 기본
        [SerializeField] private GameObject goCharBody;

        [SerializeField] private CharacterSpawner characterSpawner;
        public CharacterSpawner CharSpawner => characterSpawner;

        [SerializeField] private UIInteractionManager uiInteractionManager;
        public UIInteractionManager UIInteraction => uiInteractionManager;
        #endregion

        #region 캐릭터 아바타 관련
        [SerializeField] private string strAvatarInfo;
        public string AvatarInfo
        {
            get
            {
                return strAvatarInfo;
            }
            set
            {
                strAvatarInfo = value;
            }
        }

        [SerializeField] private bool isCharacterData;
        public bool IsCharacterData => isCharacterData;
        #endregion

        [SerializeField] private string clubName = string.Empty;
        public string ClubName
        {
            get { return clubName; }
            set { clubName = value; }
        }

        [Space(10)]
        [Header("탈것")]
        [SerializeField] private GameObject go_Vehicle;

        [Space(10)]
        [Header("동아리")]
        public Response_JoinedGroupListResultData joinedClubData;
        public string club_FileURL;
        public string club_MeetingName;
        public bool isClub_Meeting = false;

        [Space(10)]
        [Header("차단 목록")]
        public Response_MyBlockList blockPlayerList;

        [Space(10)]
        [Header("미션 정보")]
        public Response_MissionInfo missionInfo;

        [Space(10)]
        [Header("업적 정보")]
        public AchievementCompInfo achievementCompInfo = new AchievementCompInfo();
        public Response_AchievementInfo achievementInfo;

        [Space(10)]
        [Header("보물 찾기 정보")]
        public Response_Treasure treasureInfo;
        [SerializeField] private bool isTreasureLoad;
        public bool IsTreasureLoad => isTreasureLoad;

        [Space(10)]
        [Header("채팅 서버 연결")]
        public PlayerChatManager playerChatManager;

        [Space(10)]
        [Header("닉네임")]
        public GameObject ui_NickName;
        public PlayerNameController Canvas_PlayerUI;

        [Space(10)]
        [Header("토그 박스")]
        public GameObject ui_TalkBox;
        public TMP_Text text_Talk;

        [Space(10)]
        [Header("LookAt Point")]
        public Transform tr_LoockAtPos;

        PlayerMoveManager moveManager;

        private void Awake()
        {
            if (pv == null)
            {
                pv = GetComponent<PhotonView>();
            }

            if (ui_TalkBox.activeInHierarchy)
            {
                ui_TalkBox.SetActive(false);
            }

            moveManager = GetComponent<PlayerMoveManager>();
        }

        void Start()
        {
            if (pv.IsMine)
            {
                loginData = GameManager.Instance.loginData;
                pv.RPC("SyncPlayerName", RpcTarget.AllBuffered, pv.Controller.NickName);
                IsGuest = GameManager.Instance.IsGuest;

                if (uiInteractionManager == null)
                {
                    try
                    {
                        uiInteractionManager = GameObject.Find("UIManager").GetComponent<UIInteractionManager>();
                    }
                    catch
                    {
                        uiInteractionManager = null;
                    }
                }

                if (!GameManager.Instance.IsGuest)
                {
                    // 가입된 클럽 정보 조회
                    UpdateJoinedClub();

                    // 차단된 회원들 조회
                    UpdateUserBlockList();

                    // 미션정보 조회
                    UpdateMission();

                    // 보물찾기 정보 조회
                    UpdateTresure();                    
                }
                // 아바타 정보 조회
                UpdateAvatarInfo();

                // 채팅 서버 연결
                playerChatManager.ChatServerConnect();
            }
        }

        private void Update()
        {
            // 본인이 아닐때 동작
            //if (!pv.IsMine)
            //{
            //    Canvas_PlayerUI.transform.LookAt(NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().tr_LoockAtPos);
            //    // 닉네임 UI 회전
            //    //ui_NickName.transform.LookAt(NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().tr_LoockAtPos);
            //    //ui_NickName.transform.eulerAngles += new Vector3(0.0f, 180.0f, 0.0f);

            //    // 말풍선 회전
            //    //ui_TalkBox.transform.LookAt(NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().tr_LoockAtPos);
            //    //ui_TalkBox.transform.eulerAngles += new Vector3(0.0f, 180.0f, 0.0f);
            //}

            if(pv.IsMine)
            {
                if (joinedClubData != null && joinedClubData.nickname == PhotonNetwork.NickName)
                {
                    if (isClub_Meeting)
                    {
                        pv.RPC("RPCClubMeetingShare", RpcTarget.Others, club_MeetingName, club_FileURL);
                    }
                }
            }
        }

        /// <summary>
        /// 미션 정보 업데이트
        /// </summary>
        public void UpdateMission()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userMissionInfo}", (jsonData) =>
            {
                JArray jArray = null;
                try
                {
                    jArray = JArray.Parse(jsonData);
                }
                catch
                {
                    jArray = null;
                }

                if (jArray == null) return;

                Response_AchievementInfo response_AchievementInfo = null;
                try
                {
                    response_AchievementInfo = JsonUtility.FromJson<Response_AchievementInfo>(jArray.First.ToString());
                }
                catch
                {
                    response_AchievementInfo = null;
                }

                if (response_AchievementInfo == null) return;

                SetMissionInfo(response_AchievementInfo);
            }));

            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userRewardInfo}", (jsonData) =>
            {
                JArray jArray = null;
                try { jArray = JArray.Parse(jsonData); }
                catch { jArray = null; }
                if (jArray == null) return;

                SetMissionComp(jArray);
            }));
        }

        /// <summary>
        /// 보물찾기 정보 업데이트
        /// </summary>
        public void UpdateTresure()
        {
            // DB의 미션정보 불러오기 호출
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userTreasure}", (jsonData) =>
            {
                UTILS.Log($"UpdateTresure : {jsonData}");
                Response_Treasure reponse_Treasure = null;
                try
                {
                    // DB에서 응답받은 JsonData를 미션정보로 파싱
                    reponse_Treasure = JsonUtility.FromJson<Response_Treasure>(jsonData);
                }
                catch
                {
                    reponse_Treasure = null;
                }
                if (reponse_Treasure == null) return;

                // 보물찾기 정보 설정
                SetTresure(reponse_Treasure);
            }));
        }

        /// <summary>
        /// 아바타 정보 업데이트
        /// </summary>
        public void UpdateAvatarInfo()
        {
            // 아바타 정보 불러오기
            Request_AvatarLoad request_AvatarLoad = new Request_AvatarLoad(pv.Controller.NickName);

            // DB의 미션정보 불러오기 호출
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.avatarLoadUrl}", request_AvatarLoad, (jsonData) =>
            {
                Response_AvatarLoad _resultData = null;
                JArray jArray = JArray.Parse(jsonData);
                if (jArray == null || jArray.First == null)
                {
                    _resultData = null;
                }
                else
                {
                    try
                    {
                        // DB에서 응답받은 JsonData를 미션정보로 파싱
                        _resultData = new Response_AvatarLoad(jArray.First.ToString());
                    }
                    catch
                    {
                        _resultData = null;
                    }
                }

                if (pv.IsMine)
                {
                    // 아바타 정보 적용
                    AvatarInfoLoad(_resultData);
                }
            }));
        }

        /// <summary>
        /// 아바타 정보 불러오기
        /// </summary>
        /// <param name="resultData"></param>
        private void AvatarInfoLoad(Response_AvatarLoad resultData)
        {
            if (resultData == null || resultData.response_AvatarLoadResultDatas == null || string.IsNullOrEmpty(resultData.response_AvatarLoadResultDatas.avatar))
            {
                // 필요한 정보가 없다면 아바타 커스텀 마이징 실행 O
                isCharacterData = false;
            }
            else
            {
                // 필요한 정보가 있다면 아바타 정보 업데이트
                strAvatarInfo = resultData.response_AvatarLoadResultDatas.avatar;
                // 필요한 정보가 있다면 아바타 커스텀 마이징 실행 X
                isCharacterData = true;
            }

            // 게스트가 아닐경우
            if (!IsGuest)
            {
                if (!isCharacterData)
                {
                    // 캐릭터 커스텀 마이징 실행
                    uiInteractionManager.Btn_CharDecorationClick();
                }
                else
                {
                    // Photon RPC를 통해 아바타 동기화
                    pv.RPC("RPCSetAvatarInfo", RpcTarget.AllBuffered, strAvatarInfo);
                    //CharacterSpawn(strAvatarInfo);
                }
            }
            // 게스트일 경우
            else
            {
                // 아바타 동기화 X
                pv.RPC("RPCSetAvatarInfo", RpcTarget.AllBuffered, "");
            }

            pv.RPC("RPCSetGuestInfo", RpcTarget.AllBuffered, IsGuest);
        }

        /// <summary>
        /// 동아리 정보 업데이트
        /// </summary>
        public void UpdateJoinedClub()
        {
            // 가입된 동아리 정보 불러오기
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.joinedClubInfoUrl}", (jsonData) =>
            {
                // 배열로 구성되어있어 JArray로 파싱 진행
                JArray jArray = JArray.Parse(jsonData);

                if (jArray == null || jArray.First == null) { return; }
                else
                {
                    Response_JoinedGroupListResultData _resultData = null;
                    try
                    {
                        // jArray의 첫번째 데이터를 사용해서 가입된 동아리 정보 파싱
                        _resultData = JsonUtility.FromJson<Response_JoinedGroupListResultData>(jArray.First.ToString());
                    }
                    catch
                    {
                        _resultData = null;
                    }

                    if (_resultData == null) return;

                    // 동아리 정보 설정
                    SetJoinedClubInfo(_resultData);
                }
            }));
        }

        /// <summary>
        /// 동아리 정보 설정
        /// </summary>
        /// <param name="_resultData"></param>
        public void SetJoinedClubInfo(Response_JoinedGroupListResultData _resultData)
        {
            joinedClubData = _resultData;
        }

        /// <summary>
        /// 차단 회원 정보 업데이트
        /// </summary>
        public void UpdateUserBlockList()
        {
            // 회원 차단 목록 불러오기
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userBlockListUrl}", (jsonData) =>
            {
                JArray jArray = JArray.Parse(jsonData);
                if (jArray == null || jArray.Count == 0) { return; }
                else
                {
                    Response_MyBlockList resultData = new Response_MyBlockList();
                    foreach (var item in jArray)
                    {
                        Response_MyBlockListResultData result = null;
                        try
                        {
                            // jArray의 데이터들을 파싱
                            result = JsonUtility.FromJson<Response_MyBlockListResultData>(item.ToString());

                        }
                        catch
                        {
                            result = null;
                        }

                        // 정보가 있다면 resultData에 추가
                        resultData.response_MyBlockListResults.Add(result);
                    }

                    // 차단 회원 목록 적용
                    SetUserBlockList(resultData);
                }
            }));
        }

        /// <summary>
        /// 차단 회원 목록 적용
        /// </summary>
        /// <param name="_resultData"></param>
        private void SetUserBlockList(Response_MyBlockList _resultData)
        {
            blockPlayerList = _resultData;
        }

        /// <summary>
        /// 미션 정보 적용
        /// </summary>
        /// <param name="_resultData"></param>
        private void SetMissionInfo(Response_AchievementInfo _resultData)
        {
            achievementInfo = _resultData;
        }

        private void SetMissionComp(JArray jArray)
        {
            foreach (var jobj in jArray)
            {
                Response_RewardInfo reawrdData = JsonUtility.FromJson<Response_RewardInfo>(jobj.ToString());
                if (reawrdData.reward.Equals("all"))
                {
                    achievementCompInfo.isAllComp = true;
                }
                else if (reawrdData.reward.Equals("game"))
                {
                    achievementCompInfo.isMiniGameComp = true;
                }
                else if (reawrdData.reward.Equals("lib"))
                {
                    achievementCompInfo.isLibVisiteComp = true;
                }
                else if (reawrdData.reward.Equals("comm"))
                {
                    achievementCompInfo.isMyRoomComp = true;
                }
                else if (reawrdData.reward.Equals("treasure"))
                {
                    achievementCompInfo.isTreasureComp = true;
                }
            }
        }

        /// <summary>
        /// 보물찾기 정보 적용
        /// </summary>
        /// <param name="_resultData"></param>
        private void SetTresure(Response_Treasure _resultData)
        {
            treasureInfo = _resultData;
            isTreasureLoad = true;
        }

        /// <summary>
        /// 캐릭터 스폰
        /// </summary>
        /// <param name="avatarInfo"></param>
        public void CharacterSpawn(string avatarInfo)
        {
            // 아바타 정보에 맞춰 캐릭터 스폰
            characterSpawner.SpawnSavedCharacter(avatarInfo);

            // 본인일때 실행
            if (pv.IsMine)
            {
                // 처음 스폰될때
                if (!GameManager.Instance.IsInitSpawn)
                {
                    // 맵 UI 켜기
                    //uiInteractionManager.Btn_MapClick();

                    // 처음 스폰 플래그 갱신
                    GameManager.Instance.IsInitSpawn = true;
                }

                // OX게임 위치로 스폰시킬때
                if (NetworkManager.Instance.SpPos == SpawnerPos.OX퀴즈)
                {
                    // OX게임 시작
                    OXGameStart();
                }
            }
        }

        /// <summary>
        /// 탈것 켜기/끄기
        /// </summary>
        /// <param name="isOn"></param>
        public void VehicleOnOff(bool isOn)
        {
            if (isOn)
            {
                // 캐릭터 로컬 위치를 y축을 1올린다.
                goCharBody.transform.localPosition = new Vector3(0, 1, 0);
                // 탈것 오브젝트를 켜기
                go_Vehicle.SetActive(true);
                // 이동속도 설정
                moveManager.use_vehicle = true;
            }
            else
            {
                // 캐릭터 로컬 위치를 원상태 복원
                goCharBody.transform.localPosition = Vector3.zero;
                // 탈것 오브젝트 끄기
                go_Vehicle.SetActive(false);
                // 이동속도 설정
                moveManager.use_vehicle = false;
            }
        }

        /// <summary>
        /// OX 게임 시작
        /// </summary>
        public void OXGameStart()
        {
            Transform oxGamePos = null;
            // 스폰될 위치들을 모두 찾는다.
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
            foreach (GameObject sp in spawners)
            {
                // 스폰될 위치에 OX퀴즈가 있다면
                if (sp.GetComponent<Spawner>().SpawnerPos == SpawnerPos.OX퀴즈)
                {
                    // OX게임의 스폰 위치를 저장
                    oxGamePos = sp.transform;
                    break;
                }
            }
            // 플레이어를 이동시킨다.
            QuizManager.instance.SetPlayerQuiz(oxGamePos);
        }

        /// <summary>
        /// 말풍선 띄우기
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public IEnumerator ShowTalkBox(string msg)
        {
            // ui가 있다면
            if (ui_TalkBox != null)
            {
                // ui를 활성화 시킨다
                ui_TalkBox.SetActive(true);
                // 말풍선 텍스트를 적용
                text_Talk.text = msg;
                yield return new WaitForSeconds(3.0f);
                // 3초뒤에 ui 비활성화
                ui_TalkBox.SetActive(false);
            }
            else
            {
                yield return null;
            }
        }
    }
}
