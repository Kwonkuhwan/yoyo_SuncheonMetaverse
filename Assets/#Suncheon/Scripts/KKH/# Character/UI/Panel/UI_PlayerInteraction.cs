using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using Suncheon.Player;
using Suncheon.WebData;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class UI_PlayerInteraction : MonoBehaviour
    {
        protected YesBtnDelegate yesBtnDelegate;
        protected NoBtnDelegate noBtnDelegate;

        #region UnityButton
        [SerializeField] private Button btn_Whispering;         // 귓속말
        [SerializeField] private Button btn_LibVisit;           // 서재 방문
        [SerializeField] private Button btn_ClubInVate;         // 동아리 초대
        [SerializeField] protected Button btn_CutOff;             // 차단
        #endregion

        [SerializeField] private PlayerManager playerManager;

        [SerializeField] private TMP_Text text_ChatMode;

        [SerializeField] private TMP_Text text_Name;

        [SerializeField] private float rayCastMaxDistance = 100.0f;

        [SerializeField] private PhotonView interactionPV;
        [SerializeField] private bool isCutOff = false;

        // 상호작용 캐릭터가 날 차단했는지 확인 유무
        [SerializeField] private bool isCheckBlockFlag = false;
        string hitPlayerName;

        #region 꾹 눌러서 우클릭 기능 변수        
        [Header("길게누르기 UI")]
        private float pressDuration = 0f;
        private float timeToFill = 0.5f;    //기능 실행까지 걸리는 시간
        bool hasTarget = false;
        [SerializeField] Image UI_Click;
        Canvas canvas;
        #endregion

        protected virtual void Awake()
        {
            if (btn_Whispering != null) btn_Whispering.onClick.AddListener(() => WhisperingBtnClick()); // 귓속말
            if (btn_LibVisit != null) btn_LibVisit.onClick.AddListener(() => MyLibRoomVisitBtnClick()); // 서재 방문
            if (btn_ClubInVate != null) btn_ClubInVate.onClick.AddListener(() => ClubInViteBtnClick()); // 동아리 가입 초대
            if (btn_CutOff != null) btn_CutOff.onClick.AddListener(() => CutOffBtnClick());             // 차단

            canvas = GetComponentInParent<Canvas>();
            UI_Click.gameObject.SetActive(false);
        }

        private void Update()
        {
            Vector3 mousePos = Input.mousePosition;

#if UNITY_EDITOR || UNITY_WEBGL
            if (Input.GetMouseButtonDown(1)) 
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, rayCastMaxDistance, LayerMask.GetMask("Player")))
                {
                    interactionPV = hit.collider.gameObject.GetPhotonView();
                    hitPlayerName = interactionPV.Controller.NickName;

                    if (hitPlayerName == PhotonNetwork.NickName) return;

                    UIInteractionManager.Instance.PlayerInteractionOn(hitPlayerName, mousePos);
                    SetCutOff();
                    Load_UserID();
                    return;
                }
                else if(!EventSystem.current.IsPointerOverGameObject())
                {
                    Close_UI();
                }
            }
#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            //좌클릭 길게 눌러 실행
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, rayCastMaxDistance, LayerMask.GetMask("Player")))
                {
                    interactionPV = hit.collider.gameObject.GetPhotonView();
                    hitPlayerName = interactionPV.Controller.NickName;

                    if (hitPlayerName == PhotonNetwork.NickName) return;

                    Start_Click();
                }
                else if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Close_UI();
                }
            }
            else if (Input.GetMouseButton(0) && hasTarget) //마우스를 누르고 있는 동안
            {
                pressDuration += Time.deltaTime; //누른시간 증가
                UI_Click.fillAmount = Mathf.Clamp01(pressDuration / timeToFill); //누른시간을 전체 비율로 환산하여 fillAmount적용

                if (UI_Click.fillAmount >= 1f) //fillAmount가 가득 찼을때
                {
                    UIInteractionManager.Instance.PlayerInteractionOn(hitPlayerName, mousePos);
                    SetCutOff();
                    Load_UserID();
                    Reset_FillAmount();
                    return;
                }
            }
            else if (!Input.GetMouseButton(0) && UI_Click.fillAmount < 1) // 중간에 클릭을 멈추었을 때
            {
                Reset_FillAmount();
            }
#endif

        }

        /// <summary>
        /// 유저 클릭 시작
        /// </summary>
        void Start_Click() 
        {
            //클릭 UI 위치 변경
            Vector2 mousePosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                mousePosition, canvas.worldCamera, out Vector2 localPoint);

            UI_Click.gameObject.SetActive(true);
            UI_Click.rectTransform.localPosition = localPoint;
            UI_Click.fillAmount = 0;

            pressDuration = 0;
            hasTarget = true;
        }

        /// <summary>
        /// 클릭 UI 초기화
        /// </summary>
        void Reset_FillAmount()
        {
            UI_Click.gameObject.SetActive(false); 
            UI_Click.fillAmount = 0;          
            pressDuration = 0f;                   
            hasTarget = false;
        }
        
        /// <summary>
        /// 타 유저 상호작용 UI 닫기
        /// </summary>
        public void Close_UI()
        {
            if (UIInteractionManager.Instance != null)
            {
                if (SceneManager.GetActiveScene().name.Equals("02_Garden_Scene"))
                {
                    if (UIInteractionManager.Instance.ui_PlayerInteraction.GetComponent<CanvasGroup>().alpha == 1)
                    {
                        UIInteractionManager.Instance.PlayerInteractionOff();
                    }
                }
                else if (SceneManager.GetActiveScene().name.Equals("03_Inside"))
                {
                    UIInteractionManager_Inside inter = null;
                    try
                    {
                        inter = (UIInteractionManager_Inside)UIInteractionManager.Instance;
                    }
                    catch
                    {
                        inter = null;
                    }

                    if (inter != null)
                    {
                        if (inter.ui_PlayerInteraction.GetComponent<CanvasGroup>().alpha == 1)
                        {
                            inter.PlayerInteractionOff();
                        }
                        else if (inter.ui_ClubPlayerInteraction.GetComponent<CanvasGroup>().alpha == 1)
                        {
                            inter.ClubPlayerInteractionOff();
                        }
                    }
                }
                else if (SceneManager.GetActiveScene().name.Equals("04_Myroom"))
                {
                    UIInteractionManager_Room inter = null;
                    try
                    {
                        inter = (UIInteractionManager_Room)UIInteractionManager.Instance;
                    }
                    catch
                    {
                        inter = null;
                    }

                    if (inter != null)
                    {
                        if (inter.ui_PlayerInteraction.GetComponent<CanvasGroup>().alpha == 1)
                        {
                            inter.PlayerInteractionOff();
                        }
                        else if (inter.ui_LibPlayerInteraction.GetComponent<CanvasGroup>().alpha == 1)
                        {
                            inter.LibPlayerInteractionOff();
                        }
                    }
                }
                Reset_FillAmount();
            }
        }

        private void OnEnable()
        {
            if (btn_Whispering != null) btn_Whispering.gameObject.GetComponent<ButtonControl>().TextColorChange(false);
            if (btn_LibVisit != null) btn_LibVisit.gameObject.GetComponent<ButtonControl>().TextColorChange(false);
            if (btn_ClubInVate != null) btn_ClubInVate.gameObject.GetComponent<ButtonControl>().TextColorChange(false);
            if (btn_CutOff != null) btn_CutOff.gameObject.GetComponent<ButtonControl>().TextColorChange(false);
        }

        #region 설정
        /// <summary>
        /// 상호작용 캐릭터의 닉네임 설정
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            text_Name.text = name;
        }

        // 상호작용 캐릭터가 날 차단했는지 체크
        public void SetInteractionPlayerCheckBlockFlag()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.checkBlockFlag}", $"nickname={text_Name.text}", (jsonData) =>
            {
                SetCheckBlockFlag(bool.Parse(jsonData));
            }));
        }

        private void SetCheckBlockFlag(bool isCheck)
        {
            isCheckBlockFlag = isCheck;
        }

        /// <summary>
        /// UI의 차단 / 차단 해제 설정
        /// </summary>
        public void SetCutOff()
        {
            isCutOff = false;
            foreach (Response_MyBlockListResultData block in NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().blockPlayerList.response_MyBlockListResults)
            {
                if (block.nickname == text_Name.text)
                {
                    isCutOff = true;
                    break;
                }
            }

            btn_CutOff.transform.GetChild(0).GetComponent<TMP_Text>().text = isCutOff ? $"차단 해제" : $"차단 하기";
        }

        /// <summary>
        /// 상대 ID 검색
        /// </summary>
        void Load_UserID()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userIdCheck}", $"nickname={interactionPV.Controller.NickName}",
               (jsonData) =>
               {
                   Set_UserID(jsonData);
               }
               ));
        }

        /// <summary>
        /// 상대 ID 적용
        /// </summary>
        /// <param name="userID"></param>
        void Set_UserID(string userID)
        {
            NetworkManager.Instance.user_id = userID;
            NetworkManager.Instance.user_name = interactionPV.Controller.NickName;
        }
        #endregion

        #region 귓속말 관련
        /// <summary>
        /// 귓속말 설정
        /// </summary>
        private void WhisperingBtnClick()
        {
            Close_UI();

            UTILS.Log("WhisperingBtnClick");
            UIInteractionManager.Instance.chatMode = ChatMode.Whispering;
            if (text_Name.text.Length > 5)
            {
                UIInteractionManager.Instance.SendWhisperNickName = text_Name.text;
                text_ChatMode.text = text_Name.text.Substring(0, 4) + "...";
            }
            else
            {
                text_ChatMode.text = text_Name.text;
            }
        }
        #endregion

        #region 개인서재 관련
        /// <summary>
        /// 상호작용 상대 개인서재 방문
        /// </summary>
        private void MyLibRoomVisitBtnClick()
        {
            Close_UI();

            if (interactionPV.GetComponent<PlayerManager>().IsGuest)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"게스트의 개인서재는 방문할 수 없습니다.");
                return;
            }

            if (CutOffCheck()) 
            { 
                UIInteractionManager.Instance.ShowSystemPopUp($"차단되어 있는 회원은 개인서재는 방문할 수 없습니다.");
                return;
            }

            if (isCheckBlockFlag)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"{text_Name.text}님이 나(을)를 차단하여 동아리 가입 권유할 수 없습니다.");
                return;
            }

            InvadeLib invadeLib = GetComponentInChildren<InvadeLib>();

            if (invadeLib != null)
            {
                invadeLib.On_UI();
            }
        }
        #endregion

        #region 동아리 관련
        /// <summary>
        /// 상호작용 상대에게 자신의 동아리 가입 권유
        /// </summary>
        private void ClubInViteBtnClick()
        {
            Close_UI();

            if (playerManager == null)
            {
                playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            }

            if (interactionPV.GetComponent<PlayerManager>().IsGuest)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"게스트는 동아리에 초대 할 수 없습니다.");
                return;
            }

            // 가입된 동아리가 없다면
            if (playerManager.joinedClubData == null || string.IsNullOrEmpty(playerManager.joinedClubData.clubName))
            {
                UIInteractionManager.Instance.ui_SystemPopUp.GetComponent<UI_SystemPopUp>().ShowPopUp($"당신이 생성한 동아리가 없습니다.");
                return;
            }

            // 차단 확인해서 가입 불가 메시지 띄움
            if (CutOffCheck())
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"차단되어 있는 회원은 동아리 가입 권유할 수 없습니다.");
                return;
            }

            if (isCheckBlockFlag)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"{text_Name.text}님이 나(을)를 차단하여 동아리 가입 권유할 수 없습니다.");
                return;
            }

            // 동아리 가입 권유 실행
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.enterFlagClubUrl}", $"groupSeq={playerManager.joinedClubData.groupSeq}", (jsonData) =>
            {
                if (bool.Parse(jsonData))
                {
                    if (interactionPV.GetComponent<PlayerManager>().loginData != null)
                    {
                        // 상대에게 팝업을 띄워야 하기때문에 RPC를 통해 띄운다.
                        PhotonView pv = NetworkManager.Instance.Go_Player.GetPhotonView();
                        pv.RPC("RPCClubVitePopUp", RpcTarget.All, pv.Controller.NickName, interactionPV.Controller.NickName, playerManager.joinedClubData.clubName, playerManager.joinedClubData.groupSeq);
                    }
                }
                else
                {
                    UIInteractionManager.Instance.ui_SystemPopUp.GetComponent<UI_SystemPopUp>().ShowPopUp($"[{playerManager.joinedClubData.clubName}]에 남은 자리가 없습니다.");
                    return;
                }
            }));
        }
        #endregion

        #region 차단 관련
        /// <summary>
        /// 차단 확인
        /// </summary>
        public bool CutOffCheck()
        {
            if (playerManager == null)
            {
                playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            }

            Response_MyBlockList blockList = playerManager.blockPlayerList;
            foreach (Response_MyBlockListResultData block in blockList.response_MyBlockListResults)
            {
                if (block.nickname == NetworkManager.Instance.user_name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 차단하기 Yes 버튼 동작
        /// </summary>
        private void CutOffYesButtonClick()
        {
            if (!isCutOff)
            {
                UTILS.Log(NetworkManager.Instance.user_id);
                Request_UserBlock request_UserBlock = new Request_UserBlock(NetworkManager.Instance.user_id);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userBlockUrl}", request_UserBlock, (jsonData) =>
                {
                    Response_UserBlock response_UserBlock = null;
                    try
                    {
                        response_UserBlock = JsonUtility.FromJson<Response_UserBlock>(jsonData);
                    }
                    catch
                    {
                        response_UserBlock = null;
                    }

                    if (response_UserBlock == null) return;

                    CutOff(response_UserBlock);
                }));
            }
            else
            {
                Request_UserUnBlock request_UserUnBlock = new Request_UserUnBlock(NetworkManager.Instance.user_id);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userUnBlockUrl}", request_UserUnBlock, (jsonData) =>
                {
                    Response_UserUnBlock response_UserBlock = null;
                    try
                    {
                        response_UserBlock = JsonUtility.FromJson<Response_UserUnBlock>(jsonData);
                    }
                    catch
                    {
                        response_UserBlock = null;
                    }
                    if (response_UserBlock == null) return;

                    CutOn(response_UserBlock);
                }));
            }
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().HidePopUp();
        }

        /// <summary>
        /// 차단하기 No 버튼 동작
        /// </summary>
        private void CutOffNoButtonClick()
        {
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().HidePopUp();
        }

        /// <summary>
        /// 상호작용 상대 클럽 가입 권유 버튼 클릭
        /// </summary>
        private void CutOffBtnClick()
        {
            Close_UI();

            if (playerManager == null)
            {
                playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            }

            UTILS.Log("CutOffBtnClick");

            yesBtnDelegate = new YesBtnDelegate(CutOffYesButtonClick);
            noBtnDelegate = new NoBtnDelegate(CutOffNoButtonClick);
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);

            string strInfo = $"{text_Name.text}님을(를) 차단 하시겠습니까?";

            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp(strInfo);
        }

        /// <summary>
        /// 차단한 목록 업데이트(차단 하기)
        /// </summary>
        /// <param name="resultData"></param>
        private void CutOff(Response_UserBlock resultData)
        {
            if (resultData.rtnCode == "000")
            {
                playerManager.UpdateUserBlockList();
            }
        }

        /// <summary>
        /// 차단한 목록 업데이트(차단 해제)
        /// </summary>
        /// <param name="resultData"></param>
        private void CutOn(Response_UserUnBlock resultData)
        {
            if (resultData.rtnCode == "000")
            {        
                playerManager.UpdateUserBlockList();
            }
        }
        #endregion
    }
}

