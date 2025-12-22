using Suncheon.UI;
using Suncheon.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Suncheon.WebData;

namespace Suncheon
{
    public enum AchievementMenu
    {
        All = 0,
        MiniGame,
        Lib,
        MyRoom,
        Rcmm,
        Treasure
    }

    public enum MiniGameAchievement
    {
        OX퀴즈 = 0,
        카드뒤집기,
        먹이주기
    }

    public enum LibAchievement
    {
        삼산도서관 = 0,
        그림책도서관,
        연향도서관,
        기적의도서관,
        //조례호수도서관,
        신대도서관
    }

    public enum MyRoomAchievement
    {
        방명록 = 0,
    }

    public enum TreasureAchievement
    {
        오천그린광장 = 0,
        물의광장,
        삼산도서관,
        그림책도서관,
        연향도서관,
        기적의도서관,
        조례호수도서관,
        신대도서관
    }

    public class Achievement_ScrollView : MonoBehaviour
    {
        [SerializeField] private Transform scroll_Content;
        [SerializeField] private Scrollbar achievement_Scrollbar;
        [SerializeField] private GameObject achievement_Image;

        [SerializeField] private List<string> miniGameName_List;
        [SerializeField] private List<string> libName_List;
        [SerializeField] private List<string> myRoomName_List;
        [SerializeField] private List<string> treasure_List;
        private int[] achievementCnt = { 1, 5, 10, 50, 100 };

        [SerializeField] private AchievementMenu type = AchievementMenu.All;

        [Header("버튼")]
        [SerializeField] private Button btn_All;
        [SerializeField] private Button btn_MiniGame;
        [SerializeField] private Button btn_Lib;
        [SerializeField] private Button btn_MyRoom;
        [SerializeField] private Button btn_Treasure;

        [SerializeField] private Button btn_Compensation;
        [SerializeField] private TMP_Text text_Compensation;

        [Header("텍스트")]
        // 업적 달성도
        [SerializeField] private TMP_Text text_CompletionPercentage;
        // 보상 이름
        [SerializeField] private TMP_Text text_CompensationName;

        List<GameObject> miniGameObjList = new List<GameObject>();
        List<GameObject> libVisteObjList = new List<GameObject>();
        List<GameObject> myRoomObjList = new List<GameObject>();
        List<GameObject> treasureObjList = new List<GameObject>();

        private void Awake()
        {
            miniGameName_List = Enum.GetNames(typeof(MiniGameAchievement)).ToList();
            libName_List = Enum.GetNames(typeof(LibAchievement)).ToList();
            myRoomName_List = Enum.GetNames(typeof(MyRoomAchievement)).ToList();
            treasure_List = Enum.GetNames(typeof(TreasureAchievement)).ToList();

            btn_All.onClick.AddListener(() => AllHideObject());
            btn_All.onClick.AddListener(() => BtnAllClick());

            btn_MiniGame.onClick.AddListener(() => AllHideObject());
            btn_MiniGame.onClick.AddListener(() => BtnMiniGameClick());

            btn_Lib.onClick.AddListener(() => AllHideObject());
            btn_Lib.onClick.AddListener(() => BtnLibClick());

            btn_MyRoom.onClick.AddListener(() => AllHideObject());
            btn_MyRoom.onClick.AddListener(() => BtnMyRoomClick());

            btn_Treasure.onClick.AddListener(() => AllHideObject());
            btn_Treasure.onClick.AddListener(() => BtnTreasureClick());

            btn_Compensation.onClick.AddListener(() => BtnCompensationClick());
        }

        // Start is called before the first frame update
        void Start()
        {
            ScrollClear();

            CreateMiniGameList();
            CreateLibList();
            CreateMyRoomList();
            CreateTreasureList();

            // 보상 검사 루틴 추가
            BtnAllClick();
        }

        private void BtnCompensationClick()
        {
            PlayerManager playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            Request_InsertUserReward request_InsertUserReward = null;

            if (type == AchievementMenu.All)
            {
                playerManager.achievementCompInfo.isAllComp = true;
                request_InsertUserReward = new Request_InsertUserReward("all");
            }
            else if (type == AchievementMenu.MiniGame)
            {
                playerManager.achievementCompInfo.isMiniGameComp = true;
                request_InsertUserReward = new Request_InsertUserReward("game");
            }
            else if (type == AchievementMenu.Lib)
            {
                playerManager.achievementCompInfo.isLibVisiteComp = true;
                request_InsertUserReward = new Request_InsertUserReward("lib");
            }
            else if (type == AchievementMenu.MyRoom)
            {
                playerManager.achievementCompInfo.isMyRoomComp = true;
                request_InsertUserReward = new Request_InsertUserReward("comm");
            }
            else if (type == AchievementMenu.Treasure)
            {
                playerManager.achievementCompInfo.isTreasureComp = true;
                request_InsertUserReward = new Request_InsertUserReward("treasure");
            }

            if (request_InsertUserReward == null) return;

            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.insertUserReward}", request_InsertUserReward, (jsonData) =>
            {
                Response_ReturnMsg returnMsg = null;
                try { returnMsg = JsonUtility.FromJson<Response_ReturnMsg>(jsonData); }
                catch { returnMsg = null; }

                if (returnMsg == null) return;

                if (returnMsg.rtnCode == "000")
                {
                    SetCompBtnInteractable(false);
                }
                else
                {
                    UTILS.Log($"{returnMsg.rtnMsg}");
                }
            }));
        }

        private void SetCompensation(string compensationName)
        {
            text_CompensationName.text = compensationName;

            Cal_CompletionPercentage();
        }

        private void SetCompBtnInteractable(bool isOn)
        {
            btn_Compensation.interactable = isOn;
            Color color = text_Compensation.color;
            if (isOn)
            {
                color = Color.white;
            }
            else
            {
                color = Color.black;

            }
            text_Compensation.color = color;
        }

        private void Cal_CompletionPercentage()
        {
            Achievement_Image[] AIs = scroll_Content.GetComponentsInChildren<Achievement_Image>();
            int compCnt = 0;
            int totalCnt = 0;

            foreach (Achievement_Image AI in AIs)
            {
                if (type != AchievementMenu.All)
                {
                    if (AI.Type == type)
                    {
                        totalCnt++;
                        if (AI.comp)
                        {
                            compCnt++;
                        }
                    }
                }
                else
                {
                    totalCnt++;
                    if (AI.comp)
                    {
                        compCnt++;
                    }
                }
            }

            double percentage = GetPercentage(compCnt, totalCnt, 2);
            text_CompletionPercentage.text = $"{percentage}% ({compCnt} / {totalCnt})";

            if (percentage >= 100.0f)
            {
                SetCompBtnInteractable(true);

                PlayerManager playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();

                if (type == AchievementMenu.All)
                {
                    if (playerManager.achievementCompInfo.isAllComp)
                    {
                        SetCompBtnInteractable(false);
                    }
                }
                else if (type == AchievementMenu.MiniGame)
                {
                    if (playerManager.achievementCompInfo.isMiniGameComp)
                    {
                        SetCompBtnInteractable(false);
                    }
                }
                else if (type == AchievementMenu.Lib)
                {
                    if (playerManager.achievementCompInfo.isLibVisiteComp)
                    {
                        SetCompBtnInteractable(false);
                    }
                }
                else if (type == AchievementMenu.MyRoom)
                {
                    if (playerManager.achievementCompInfo.isMyRoomComp)
                    {
                        SetCompBtnInteractable(false);
                    }
                }
                else if (type == AchievementMenu.Treasure)
                {
                    if (playerManager.achievementCompInfo.isTreasureComp)
                    {
                        SetCompBtnInteractable(false);
                    }
                }
            }
            else
            {
                SetCompBtnInteractable(false);
            }
        }

        private double GetPercentage(double value, double total, int decimalplaces)
        {
            return System.Math.Round(value * 100 / total, decimalplaces);
        }

        private void BtnAllClick()
        {
            type = AchievementMenu.All;
            btn_All.GetComponent<ButtonControl>().OnBtnImageFadeOut();

            ShowMiniGameList();
            ShowLibList();
            ShowMyRoomList();
            ShowTreasureList();

            SetCompensation("개인서재 모던가구");
        }

        private void BtnMiniGameClick()
        {
            type = AchievementMenu.MiniGame;

            btn_MiniGame.GetComponent<ButtonControl>().OnBtnImageFadeOut();
            ShowMiniGameList();

            SetCompensation("미니게임 업적 트로피");
        }

        private void BtnLibClick()
        {
            type = AchievementMenu.Lib;

            btn_Lib.GetComponent<ButtonControl>().OnBtnImageFadeOut();
            ShowLibList();

            SetCompensation("도서관 방문 업적 트로피");
        }

        private void BtnMyRoomClick()
        {
            type = AchievementMenu.MyRoom;

            btn_MyRoom.GetComponent<ButtonControl>().OnBtnImageFadeOut();
            ShowMyRoomList();

            SetCompensation("개인서재 업적 트로피");
        }

        private void BtnTreasureClick()
        {
            type = AchievementMenu.Treasure;

            btn_Treasure.GetComponent<ButtonControl>().OnBtnImageFadeOut();
            ShowTreasureList();

            SetCompensation("보물찾기 업적 트로피");
        }

        public void CreateMiniGameList()
        {
            PlayerManager playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();

            foreach (string miniGame in miniGameName_List)
            {
                foreach (int cnt in achievementCnt)
                {
                    GameObject obj = Instantiate(achievement_Image, scroll_Content);

                    if (miniGame == miniGameName_List[0].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.ox}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{miniGame} 미니게임 {cnt}회 참여 {strCnt}", AchievementMenu.MiniGame, playerManager.achievementInfo.ox >= cnt);
                    }
                    else if (miniGame == miniGameName_List[1].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.card}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{miniGame} 미니게임 {cnt}회 참여 {strCnt}", AchievementMenu.MiniGame, playerManager.achievementInfo.card >= cnt);
                    }
                    else if (miniGame == miniGameName_List[2].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.feeding}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{miniGame} 미니게임 {cnt}회 참여 {strCnt}", AchievementMenu.MiniGame, playerManager.achievementInfo.feeding >= cnt);
                    }
                    miniGameObjList.Add(obj);
                }
            }
        }

        public void ShowMiniGameList()
        {
            foreach (GameObject obj in miniGameObjList)
            {
                obj.SetActive(true);
            }
        }

        public void CreateLibList()
        {
            PlayerManager playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();

            foreach (string lib in libName_List)
            {
                foreach (int cnt in achievementCnt)
                {
                    GameObject obj = Instantiate(achievement_Image, scroll_Content);

                    if (lib == libName_List[0].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.lib1}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{lib} {cnt}번 방문 {strCnt}", AchievementMenu.Lib, playerManager.achievementInfo.lib1 >= cnt);
                    }
                    else if (lib == libName_List[1].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.lib2}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{lib} {cnt}번 방문 {strCnt}", AchievementMenu.Lib, playerManager.achievementInfo.lib2 >= cnt);
                    }
                    else if (lib == libName_List[2].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.lib3}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{lib} {cnt}번 방문 {strCnt}", AchievementMenu.Lib, playerManager.achievementInfo.lib3 >= cnt);
                    }
                    else if (lib == libName_List[3].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.lib4}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{lib} {cnt}번 방문 {strCnt}", AchievementMenu.Lib, playerManager.achievementInfo.lib4 >= cnt);
                    }
                    else if (lib == libName_List[4].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.lib5}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{lib} {cnt}번 방문 {strCnt}", AchievementMenu.Lib, playerManager.achievementInfo.lib5 >= cnt);
                    }
                    else if (lib == libName_List[5].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.lib6}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{lib} {cnt}번 방문 {strCnt}", AchievementMenu.Lib, playerManager.achievementInfo.lib6 >= cnt);
                    }

                    libVisteObjList.Add(obj);
                }
            }
        }

        public void ShowLibList()
        {
            foreach (GameObject obj in libVisteObjList)
            {
                obj.SetActive(true);
            }
        }

        public void CreateMyRoomList()
        {
            PlayerManager playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();

            foreach (string myroom in myRoomName_List)
            {
                foreach (int cnt in achievementCnt)
                {
                    GameObject obj = Instantiate(achievement_Image, scroll_Content);
                    if (myroom == myRoomName_List[0].ToString())
                    {
                        string strCnt = $"({playerManager.achievementInfo.commBoardCnt}/{cnt})";
                        obj.GetComponent<Achievement_Image>().Init($"{myroom} {cnt}번 작성 {strCnt}", AchievementMenu.MyRoom, playerManager.achievementInfo.commBoardCnt >= cnt);
                    }
                    myRoomObjList.Add(obj);
                }
            }
        }

        public void ShowMyRoomList()
        {
            foreach (GameObject obj in myRoomObjList)
            {
                obj.SetActive(true);
            }
        }

        public void CreateTreasureList()
        {
            PlayerManager playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            Response_Treasure treasureInfo = playerManager.treasureInfo;

            List<int> valueList = new List<int>();
            valueList.Add(treasureInfo.location1);
            valueList.Add(treasureInfo.location2);
            valueList.Add(treasureInfo.location3);
            valueList.Add(treasureInfo.location4);
            valueList.Add(treasureInfo.location5);
            valueList.Add(treasureInfo.location6);
            valueList.Add(treasureInfo.location7);
            valueList.Add(treasureInfo.location8);

            for (int i = 0; i < treasure_List.Count; i++)
            {
                GameObject obj = Instantiate(achievement_Image, scroll_Content);

                string strCnt = $"({valueList[i]}/1)";
                obj.GetComponent<Achievement_Image>().Init($"{treasure_List[i]} 1번 찾기 {strCnt}", AchievementMenu.Treasure, valueList[i] >= 1);

                treasureObjList.Add(obj);
            }
        }

        public void ShowTreasureList()
        {
            foreach (GameObject obj in treasureObjList)
            {
                obj.SetActive(true);
            }
        }

        private void AllHideObject()
        {
            foreach (GameObject obj in miniGameObjList)
            {
                obj.SetActive(false);
            }

            foreach (GameObject obj in libVisteObjList)
            {
                obj.SetActive(false);
            }

            foreach (GameObject obj in myRoomObjList)
            {
                obj.SetActive(false);
            }

            foreach (GameObject obj in treasureObjList)
            {
                obj.SetActive(false);
            }
        }

        private void ScrollClear()
        {
            achievement_Scrollbar.value = 1;

            Achievement_Image[] achievement_Images = scroll_Content.GetComponentsInChildren<Achievement_Image>();

            foreach (var obj in achievement_Images)
            {
                Destroy(obj.gameObject);
            }
            scroll_Content.DetachChildren();
        }
    }
}