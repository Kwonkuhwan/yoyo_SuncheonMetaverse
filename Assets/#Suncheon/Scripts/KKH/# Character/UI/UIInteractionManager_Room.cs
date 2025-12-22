using Newtonsoft.Json.Linq;
using Photon.Pun;
using Suncheon.MyRoom;
using Suncheon.Player;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace Suncheon.UI
{
    public class UIInteractionManager_Room : UIInteractionManager
    {
        [Header("Room_Button")]
        [SerializeField] Button btn_Interior;
        [SerializeField] Button btn_GetOut;
        [SerializeField] Button btn_GuestBook;

        [Header("나가기 UI")]
        [SerializeField] Button btn_Exit;
        [SerializeField] Button btn_Exit_Ok;
        [SerializeField] Button btn_Exit_Cancle;

        [Header("게시글 삭제 UI")]
        public GameObject Comm_DeletUI;
        public Button btn_Delete_Ok;
        public Button btn_Delete_Cancle;

        [Header("UI")]
        [SerializeField] GameObject ui_LibMembers;
        [SerializeField] GameObject Interior;
        [SerializeField] GameObject Player;

        public GameObject ui_LibPlayerInteraction;

        PlayerObjInteraction objInteraction;
        PlayerMoveManager moveManager;
        CameraManager cameraManager;

        GusetBook_Interactive gusetBook_Interactive;

        override protected void Awake()
        {
            base.Awake();

            btn_Interior.onClick.AddListener(() => Btn_Interior());
            btn_GetOut.onClick.AddListener(() => Btn_GetOut());
            btn_GuestBook.onClick.AddListener(() => Btn_GuestBook());

            btn_Exit.onClick.AddListener(()=>Btn_Exit());
            btn_Exit_Ok.onClick.AddListener(()=>Exit_Y());
            btn_Exit_Cancle.onClick.AddListener(() => Exit_N());

            gusetBook_Interactive = FindAnyObjectByType<GusetBook_Interactive>();

            if (!NetworkManager.Instance.Check_MyRoom())
            {
                btn_Interior.gameObject.SetActive(false);
                btn_GetOut.gameObject.SetActive(false); 
            }
        }

        /// <summary>
        /// 인테리어UI와 플레이어UI의 전환
        /// </summary>
        void Btn_Interior()
        {
            if (PhotonNetwork.PlayerList.Length == 1)
            {
                cameraManager = NetworkManager.Instance.Go_Player.GetComponentInChildren<CameraManager>();
                moveManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerMoveManager>();
                objInteraction = NetworkManager.Instance.Go_Player.GetComponent<PlayerObjInteraction>();

                InteractionManager.Inst.Ray_Off();
                objInteraction.Clear();
                Set_PlayerMove(false);
                Player_Physics_Off();
                FurnitureManager.Instance.usingInterior = true;
                Interior.SetActive(true);
                Player.SetActive(false);
            }
            else
            {
                ShowSystemPopUp("서재에 방문객이 있어 인테리어를 수정할 수 없습니다.");
            }
           
        }

        void Btn_GetOut()
        {
            UTILS.Log("내보내기");
            InteractionManager.Inst.Ray_Off();
            ui_LibMembers.GetComponent<UI_LibMembers>().ShowPopUp();
        }
        void Btn_GuestBook()
        {
            InteractionManager.Inst.Ray_Off();
            gusetBook_Interactive.OnClick();
        }

        /// <summary>
        /// 개인서재 씬 나가기
        /// </summary>
        public void Btn_Exit()
        {
            yesBtnDelegate = new YesBtnDelegate(Exit_Y);
            noBtnDelegate = new NoBtnDelegate(Exit_N);

            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);
            ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp("광장으로 이동하시겠습니까?");

            OpenPopUp(ui_YesNoPopUp);
        }

        /// <summary>
        /// 광장씬으로 이동
        /// </summary>
        public void Exit_Y()
        {
            ClosePopUp(ui_YesNoPopUp);
            NetworkManager.Instance.SetSpawnerPos(SpawnerPos.오천그린광장);
            NetworkManager.Instance.OnLeaveRoom();
            UTILS.LoadingSceneLoad("02_Garden_Scene");
        }
        public void Exit_N()
        {
            ClosePopUp(ui_YesNoPopUp);
        }

        /// <summary>
        /// 플레이어 조작 가능여부 변경
        /// </summary>
        /// <param name="on"></param>
        void Set_PlayerMove(bool on)
        {
            cameraManager.enabled = on;
            moveManager.enabled = on;
        }

        /// <summary>
        /// 플레이어의 충돌판정 및 물리효과 비 활성화
        /// </summary>
        void Player_Physics_Off()
        {
            GameObject[] playerObject = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject obj in playerObject)
            {
                PlayerObjInteraction poi = obj.GetComponent<PlayerObjInteraction>();
                if (poi != null)
                {
                    poi.enabled = false;

                    Rigidbody r = obj.GetComponent<Rigidbody>();
                    r.useGravity = false;

                    Collider c = obj.GetComponent<Collider>();
                    c.enabled = false;
                }
                else
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 뭔지 모르겠음
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="pos"></param>
        public void LibPlayerInteractionOn(string playerName, Vector3 pos)
        {
            ui_LibPlayerInteraction.GetComponent<UI_Interaction_Room>().SetName(playerName);

            ui_LibPlayerInteraction.transform.localPosition = GetCanvasLocalPos(pos);

            CanvasGroup cg = ui_LibPlayerInteraction.GetComponent<CanvasGroup>();
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        /// <summary>
        /// 상호작용 UI를 숨김
        /// </summary>
        public void LibPlayerInteractionOff()
        {
            CanvasGroup cg = ui_LibPlayerInteraction.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }
    }
}