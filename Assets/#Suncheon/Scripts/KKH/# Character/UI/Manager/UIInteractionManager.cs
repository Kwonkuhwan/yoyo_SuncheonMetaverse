using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Voice.Unity;
using Suncheon.Player;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Suncheon.UI
{
    public class UIInteractionManager : MonoBehaviour
    {
        protected static UIInteractionManager instance;
        public static UIInteractionManager Instance => instance;

        protected YesBtnDelegate yesBtnDelegate;
        protected NoBtnDelegate noBtnDelegate;

        protected ClubViteYesBtnDelegate clubviteYesBtnDelegate;
        protected ClubViteNoBtnDelegate clubviteNoBtnDelegate;

        private PhotonView pv;

        [SerializeField] private Camera camera_UI;


        #region Panel
        [Header("Panel")]
        public Canvas ui_Canvas;                  // UI_Player Canvas
        public GameObject ui_Btn;
        public GameObject ui_Joystick;
        public GameObject ui_Chat;
        public GameObject ui_ChannelInfo;
        public GameObject ui_Mission;             // 미니게임 선택창
        public GameObject ui_MiniGamePopUp;       // 미니게임 선택창
        public GameObject ui_Announcement;        // 공지
        public GameObject ui_Map;                 // 맵
        public GameObject ui_PlayerInteraction;   // 상호작용(다른 캐릭터)
        public GameObject ui_SystemPopUp;         // 시스템 공지 팝업창
        public GameObject ui_YesNoPopUp;          // 확인 / 취소 팝업창
        public GameObject ui_LibPopUp;            // 도서관 내부 이동 팝업창
        public GameObject ui_ClubPopUp;
        public GameObject ui_ClubCreatePopUp;
        public GameObject ui_Setting;             // 세팅창
        public bool Play_Game;                    // 미니게임 실행 여부
        #endregion

        #region CharCustom
        [Header("Custom")]
        [SerializeField] protected GameObject go_CharCustom;
        #endregion

        [Space(10)]

        #region Button
        [Header("Button")]
        [SerializeField] protected Button btn_Mission;
        [SerializeField] protected Button btn_MiniGame;
        [SerializeField] protected Button btn_CharDecoration;
        [SerializeField] protected Button btn_Setting;
        [SerializeField] protected Button btn_MyLibrary;
        [SerializeField] protected Button btn_Club;
        [SerializeField] protected Button btn_Vehicle;
        [SerializeField] protected Button btn_VoiceChat;
        [SerializeField] protected Button btn_CharInteraction;
        [SerializeField] protected Button btn_Map;
        #endregion

        [Space(10)]

        #region 동작 확인
        [Header("Active")]
        [SerializeField] protected bool isVoiceChatOn;
        [SerializeField] protected bool isCharInteractionOn;
        #endregion

        [SerializeField] protected bool isUIInteraction;

        public bool IsUIInteraction
        {
            get => isUIInteraction;
            set
            {
                isUIInteraction = value;
            }
        }

        public ChatMode chatMode;

        public string SendWhisperNickName = "";

        protected Stack<GameObject> openPopups = new Stack<GameObject>();
        public Stack<GameObject> OpenPopUps => openPopups;
        protected Queue<GameObject> pendingPopus = new Queue<GameObject>();
        public Queue<GameObject> PendingPopus => pendingPopus;

        virtual protected void Awake()
        {
            if (instance == null || instance != this)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            if (camera_UI == null) camera_UI = GameObject.FindGameObjectWithTag("UI_Player_Camera").GetComponent<Camera>();
            if (ui_Canvas == null) ui_Canvas = transform.root.GetComponent<Canvas>();

            if (btn_Mission != null) btn_Mission.onClick.AddListener(() => Btn_MissionClick());
            if (btn_MiniGame != null) btn_MiniGame.onClick.AddListener(() => Btn_MiniGameClick());
            if (btn_CharDecoration != null) btn_CharDecoration.onClick.AddListener(() => Btn_CharDecorationClick());
            if (btn_Setting != null) btn_Setting.onClick.AddListener(() => Btn_SettingClick());
            if (btn_MyLibrary != null) btn_MyLibrary.onClick.AddListener(() => Btn_MyLibraryClick());
            if (btn_Club != null) btn_Club.onClick.AddListener(() => Btn_ClubClick());
            if (btn_Vehicle != null) btn_Vehicle.onClick.AddListener(() => Btn_VehicleClick());

            if (btn_VoiceChat != null) btn_VoiceChat.onClick.AddListener(() => Btn_VoiceChatClick());
            isVoiceChatOn = false;

            if (btn_CharInteraction != null) btn_CharInteraction.onClick.AddListener(() => Btn_CharInteractionClick());
            isCharInteractionOn = false;

            if (btn_Map != null) btn_Map.onClick.AddListener(() => Btn_MapClick());

            chatMode = ChatMode.Normal;
        }

        private void Start()
        {
            ui_Canvas.gameObject.SetActive(true);
            StartCoroutine(RenderUI());
        }

        IEnumerator RenderUI()
        {
            yield return new WaitForSeconds(0.5f);
            ui_Btn.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            ui_Chat.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            ui_ChannelInfo.SetActive(true);
        }

        virtual public void Update()
        {
            if (!Play_Game && Input.GetKeyDown(KeyCode.Escape))
            {
                if (openPopups.Count > 0)
                {
                    CloseLastOpenedPopUp();
                }
                else
                {
                    Btn_SettingClick();
                }
            }

            foreach (GameObject obj in openPopups)
            {
                UTILS.Log($"{obj.name}");
            }
        }

        // 팝업 열기
        virtual public void OpenPopUp(GameObject popupObj)
        {
            if (popupObj != null)
            {
                popupObj.SetActive(true);
                openPopups.Push(popupObj);
            }

            isUIInteraction = true;
            if (InteractionManager.Inst != null)
            {
                InteractionManager.Inst.Ray_Off();
            }
        }

        // 팝업 닫기
        virtual public void ClosePopUp(GameObject popupObj)
        {
            if (popupObj != null)
            {
                popupObj.SetActive(false);
                openPopups.Pop();
            }

            if (pendingPopus.Count > 0)
            {
                OpenPopUp(pendingPopus.Dequeue());
            }

            if (openPopups.Count > 0)
            {
                isUIInteraction = true;
                if (InteractionManager.Inst != null)
                {
                    InteractionManager.Inst.Ray_Off();
                }
            }
            else
            {
                isUIInteraction = false;
                if (InteractionManager.Inst != null)
                {
                    InteractionManager.Inst.Ray_On();
                }
            }
        }

        // 가장 최근에 열린 팝업 닫기
        virtual public void CloseLastOpenedPopUp()
        {
            if (openPopups.Count > 0)
            {
                ClosePopUp(openPopups.Peek());
            }

            if (openPopups.Count > 0)
            {
                isUIInteraction = true;
            }
            else
            {
                isUIInteraction = false;
            }
        }

        virtual public GameObject GetLastOpenedPopUp()
        {
            if (openPopups.Count > 0)
            {
                return openPopups.Peek();
            }

            return null;
        }

        // 열린 팝업 전체 닫기
        virtual public void CloseAllOpenPopUp()
        {
            while (openPopups.Count > 0)
            {
                ClosePopUp(openPopups.Peek());
            }

            isUIInteraction = false;
        }

        // 예약된 팝업 추가
        virtual public void ReservePopUp(GameObject popupobj)
        {
            if (popupobj != null)
            {
                pendingPopus.Enqueue(popupobj);
            }
        }

        virtual public void Btn_MissionClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            UTILS.Log("MissionClick");
            if (!GameManager.Instance.IsGuest)
            {
                if (!ui_Mission.activeInHierarchy)
                {
                    OpenPopUp(ui_Mission);
                }
                else
                {
                    ClosePopUp(ui_Mission);
                }
            }
            else
            {
                ShowSystemPopUp("Guest 계정은 해당 서비스를 이용할 수 없습니다.");
            }
        }

        // 미니게임 버튼 클릭
        virtual public void Btn_MiniGameClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            UTILS.Log("MiniGameClick");
            if (!ui_MiniGamePopUp.activeInHierarchy)
            {
                OpenPopUp(ui_MiniGamePopUp);
            }
            else
            {
                ClosePopUp(ui_MiniGamePopUp);
            }
        }

        // 캐릭터 꾸미기 버튼 클릭
        virtual public void Btn_CharDecorationClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            UTILS.Log("CharDecorationClick");

            if (NetworkManager.Instance.Go_Player.GetComponent<PlayerAnimManager>().IsVehicle)
            {
                ShowSystemPopUp("탈것 이용중에는 해당 서비스를 이용할 수 없습니다.");
                return;
            }

            if (go_CharCustom != null)
            {
                if (go_CharCustom.activeInHierarchy)
                {
                    ClosePopUp(go_CharCustom);
                }
                else
                {
                    OpenPopUp(go_CharCustom);
                }
                NetworkManager.Instance.Go_Player.GetComponent<PlayerAnimManager>().AllAnimTriggerReset();
            }
        }

        // 설정 버튼 클릭
        virtual public void Btn_SettingClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            UTILS.Log("SettingClick");
            if (ui_Setting.activeInHierarchy)
            {
                ClosePopUp(ui_Setting);
            }
            else
            {
                OpenPopUp(ui_Setting);
            }
        }

        // 개인서재 버튼 클릭
        virtual public void Btn_MyLibraryClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            UTILS.Log("MyLibraryClick");

            if (!GameManager.Instance.IsGuest)
            {
                NetworkManager.Instance.user_id = GameManager.Instance.loginData.user_id;
                NetworkManager.Instance.user_no = GameManager.Instance.loginData.user_no;

                yesBtnDelegate = new YesBtnDelegate(MyLibraryYesButtonClick);
                noBtnDelegate = new NoBtnDelegate(MyLibraryNoButtonClick);

                ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
                ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
                ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp("개인서재로 이동하시겠습니까?");
            }
            else
            {
                ShowSystemPopUp("Guest 계정은 해당 서비스를 이용할 수 없습니다.");
            }
        }

        virtual public void MyLibraryYesButtonClick()
        {
            NetworkManager.Instance.SetSpawnerPos(SpawnerPos.개인서재);
            PhotonNetwork.LeaveRoom();
            UTILS.LoadingSceneLoad("04_Myroom");
        }

        virtual public void MyLibraryNoButtonClick()
        {
            ClosePopUp(ui_YesNoPopUp);
        }

        // 동아리 버튼클릭
        virtual public void Btn_ClubClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            UTILS.Log("ClubClick");
            if (!GameManager.Instance.IsGuest)
            {
                if (!pv)
                {
                    pv = NetworkManager.Instance.Go_Player.GetPhotonView();
                }

                JoinedClubCheck();
            }
            else
            {
                ShowSystemPopUp("Guest 계정은 해당 서비스를 이용할 수 없습니다.");
            }
        }

        // 가입된 동아리 검색
        private void JoinedClubCheck()
        {
            Response_JoinedGroupListResultData resultData = pv.gameObject.GetComponent<PlayerManager>().joinedClubData;

            if (resultData == null || resultData.clubName == null || string.IsNullOrEmpty(resultData.clubName))
            {
                OpenPopUp(ui_ClubCreatePopUp);
            }
            else
            {
                ui_ClubPopUp.GetComponent<UI_ClubPopUp>().ShowPopUp($"{PhotonNetwork.NickName}님의 동아리\n{resultData.clubName}이(가) 있습니다.");
            }
        }

        // 탈것 버튼 클릭
        virtual public void Btn_VehicleClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            UTILS.Log("VehicleClick");
            if (!GameManager.Instance.IsGuest)
            {
                if (!pv)
                {
                    pv = NetworkManager.Instance.Go_Player.GetPhotonView();
                }

                PlayerManager playerManager = pv.gameObject.GetComponent<PlayerManager>();
                PlayerAnimManager playerAnimManager = pv.gameObject.GetComponent<PlayerAnimManager>();
                if (!playerAnimManager.IsVehicle)
                {
                    playerManager.VehicleOnOff(true);
                    playerAnimManager.SetBookSit(true);
                    btn_Vehicle.GetComponent<ButtonControl>().OnChangeSprite(true);
                }
                else
                {
                    playerManager.VehicleOnOff(false);
                    playerAnimManager.SetBookSit(false);
                    btn_Vehicle.GetComponent<ButtonControl>().OnChangeSprite(false);
                }
            }
            else
            {
                ShowSystemPopUp("Guest 계정은 해당 서비스를 이용할 수 없습니다.");
            }
        }

        // 음성채팅 버튼 클릭
        virtual public void Btn_VoiceChatClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            isVoiceChatOn = !isVoiceChatOn;
            btn_VoiceChat.transform.GetComponent<ButtonControl>().OnChangeSprite(isVoiceChatOn);

            if (isVoiceChatOn)
            {
                NetworkManager.Instance.Go_Player.GetComponentInChildren<Speaker>().enabled = true;
            }
            else
            {
                NetworkManager.Instance.Go_Player.GetComponentInChildren<Speaker>().enabled = false;
            }
            UTILS.Log($"VoiceChatClick {isVoiceChatOn}");
        }

        // 캐릭터 인터렉션(애니메이션) 버튼 클릭
        virtual public void Btn_CharInteractionClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            Animation anim = btn_CharInteraction.transform.GetComponent<Animation>();
            isCharInteractionOn = !isCharInteractionOn;
            if (isCharInteractionOn)
            {
                anim.clip = anim["Anim_CharInteractionFadeIn"].clip;
            }
            else
            {
                anim.clip = anim["Anim_CharInteractionFadeOut"].clip;
            }
            anim.Play();

            UTILS.Log($"CharInteractionClick {isCharInteractionOn}");
        }

        // 맵 버튼 클릭
        virtual public void Btn_MapClick()
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().Close_UI();
            if (!ui_Map.activeInHierarchy)
            {
                OpenPopUp(ui_Map);
            }
            else
            {
                ClosePopUp(ui_Map);
            }
        }

        public Vector2 GetCanvasLocalPos(Vector3 pos)
        {
            RectTransform rt = ui_Canvas.GetComponent<RectTransform>();
            Vector2 locationPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, pos, camera_UI, out locationPos);

            return locationPos;
        }

        // 플레이어 인터렉션(상호작용) On
        virtual public void PlayerInteractionOn(string playerName, Vector3 pos)
        {
            ui_PlayerInteraction.GetComponent<UI_PlayerInteraction>().SetName(playerName);

            ui_PlayerInteraction.transform.localPosition = GetCanvasLocalPos(pos);

            CanvasGroup cg = ui_PlayerInteraction.GetComponent<CanvasGroup>();
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        // 플레이어 인터렉션(상호작용) Off
        virtual public void PlayerInteractionOff()
        {
            CanvasGroup cg = ui_PlayerInteraction.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        // 시스템 팝업 On
        virtual public void ShowSystemPopUp(string info)
        {
            ui_SystemPopUp.GetComponent<UI_SystemPopUp>().ShowPopUp(info);
        }

        // 확인 취소 팝업 On
        virtual public void ShowYesNoPopUp(string info)
        {
            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp(info);
        }

        // 클럽 초대 팝업 On
        virtual public void ShowClubViteYerNoPopUp(string info, string clubName, int clubSeq)
        {
            clubviteYesBtnDelegate = new ClubViteYesBtnDelegate(ClubViteYesButtonClick);
            clubviteNoBtnDelegate = new ClubViteNoBtnDelegate(ClubViteNoButtonClick);

            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(clubviteYesBtnDelegate, clubSeq);
            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(clubviteNoBtnDelegate);

            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp(info);
        }

        private void ClubViteYesButtonClick(int clubSeq)
        {
            Request_JoinClub request_JoinClub = new Request_JoinClub(clubSeq);

            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.joinClubUrl}", request_JoinClub, (jsonData) =>
            {
                Response_JoinClub response_JoinClub = JsonUtility.FromJson<Response_JoinClub>(jsonData);
                
                // 동아리 가입에 성공했다면
                if (response_JoinClub.rtnCode == "000")
                {
                    StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.joinedClubInfoUrl}", (jsonData) =>
                    {
                        JArray jArray = JArray.Parse(jsonData);
                        if (jArray == null || jArray.First == null) { return; }
                        else
                        {
                            Response_JoinedGroupListResultData _resultData = JsonUtility.FromJson<Response_JoinedGroupListResultData>(jArray.First.ToString());
                            if (_resultData == null) return;
                            NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().SetJoinedClubInfo(_resultData);
                        }
                    }));
                }
                else
                {
                    UTILS.Log(response_JoinClub.rtnMsg);
                }
            }));            

            ClosePopUp(ui_YesNoPopUp);
        }

        private void ClubViteNoButtonClick()
        {
            ClosePopUp(ui_YesNoPopUp);
        }
    }
}