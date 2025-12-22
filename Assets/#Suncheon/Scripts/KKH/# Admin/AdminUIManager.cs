using Suncheon.UI;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class AdminUIManager : MonoBehaviour
    {
        private static AdminUIManager instance;
        public static AdminUIManager Instacne => instance;

        [Header("Button")]
        [SerializeField] private Button btn_Notice;
        [SerializeField] private Button btn_OperationalityBoard;

        [Space(10)]

        [SerializeField] private Button btn_EventUpLoad;
        [SerializeField] private Button btn_EventList;

        [Space(10)]

        [SerializeField] private Button btn_MiniGame;
        [SerializeField] private Button btn_NPC;

        [Space(10)]

        [SerializeField] private Button btn_BestSellerVoteSet;
        [SerializeField] private Button btn_BestSellerVote;

        [Space(10)]

        [SerializeField] private Button btn_ExhibitionSpace;
        [SerializeField] private Button btn_Museum;
        [SerializeField] private Button btn_ScreenSetting;
        [SerializeField] private Button btn_BannerSetting;
        [SerializeField] private Button btn_ChildrenPicture;
        [SerializeField] private Button btn_SSMuseum;

        [Space(10)]

        [SerializeField] private Button btn_GuestBook;
        [SerializeField] private Button btn_BookRecommendation;


        [Header("Panel")]
        [SerializeField] private GameObject ui_Notice;
        [SerializeField] private GameObject ui_OperationalityBoard;

        [Space(10)]

        [SerializeField] private GameObject ui_EventUpLoad;
        [SerializeField] private GameObject ui_EventList;

        [Space(10)]

        [SerializeField] private GameObject ui_MiniGame;
        [SerializeField] private GameObject ui_NPC;

        [Space(10)]

        [SerializeField] private GameObject ui_BestSellerVoteSet;
        [SerializeField] private GameObject ui_BestSellerVote;

        [Space(10)]

        [SerializeField] private GameObject ui_ExhibitionSpace;
        [SerializeField] private GameObject ui_Museum;
        [SerializeField] private GameObject ui_ScreenSetting;
        [SerializeField] private GameObject ui_BannerSetting;
        [SerializeField] private GameObject ui_ChildrenPicture;
        [SerializeField] private GameObject ui_SSMuseum;

        [Space(10)]

        [SerializeField] private GameObject ui_GuestBook;
        [SerializeField] private GameObject ui_BookRecommendation;

        [Space(10)]

        [SerializeField] private GameObject dev_ChannelReset;
        public GameObject ui_ResultPopUp;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            btn_Notice.onClick.AddListener(() => AllUIClose());
            btn_Notice.onClick.AddListener(() => BtnNoticeClick());

            btn_OperationalityBoard.onClick.AddListener(() => AllUIClose());
            btn_OperationalityBoard.onClick.AddListener(() => BtnOperationalityBoardClick());


            btn_EventUpLoad.onClick.AddListener(() => AllUIClose());
            btn_EventUpLoad.onClick.AddListener(() => BtnEventUpLoadClick());

            btn_EventList.onClick.AddListener(() => AllUIClose());
            btn_EventList.onClick.AddListener(() => BtnEventListClick());


            btn_MiniGame.onClick.AddListener(() => AllUIClose());
            btn_MiniGame.onClick.AddListener(() => BtnMiniGameClick());

            btn_NPC.onClick.AddListener(() => AllUIClose());
            btn_NPC.onClick.AddListener(() => BtnNPCClick());


            btn_BestSellerVoteSet.onClick.AddListener(() => AllUIClose());
            btn_BestSellerVoteSet.onClick.AddListener(() => BtnBestSellerVoteSetClick());

            btn_BestSellerVote.onClick.AddListener(() => AllUIClose());
            btn_BestSellerVote.onClick.AddListener(() => BtnBestSellerVoteClick());


            btn_ExhibitionSpace.onClick.AddListener(() => AllUIClose());
            btn_ExhibitionSpace.onClick.AddListener(() => BtnExhibitionSpaceClick());

            btn_Museum.onClick.AddListener(() => AllUIClose());
            btn_Museum.onClick.AddListener(() => BtnMuseumClick());

            btn_ScreenSetting.onClick.AddListener(() => AllUIClose());
            btn_ScreenSetting.onClick.AddListener(() => BtnScreenSettingClick());

            btn_BannerSetting.onClick.AddListener(() => AllUIClose());
            btn_BannerSetting.onClick.AddListener(() => BtnBannerSettingClick());

            btn_ChildrenPicture.onClick.AddListener(() => AllUIClose());
            btn_ChildrenPicture.onClick.AddListener(() => BtnChildrenPictureClick());

            btn_SSMuseum.onClick.AddListener(() => AllUIClose());
            btn_SSMuseum.onClick.AddListener(() => BtnSSMuseumClick());


            btn_GuestBook.onClick.AddListener(() => AllUIClose());
            btn_GuestBook.onClick.AddListener(() => BtnGuestBookClick());

            btn_BookRecommendation.onClick.AddListener(() => AllUIClose());
            btn_BookRecommendation.onClick.AddListener(() => BtnBookRecommendationClick());

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                if (ui_Notice.activeInHierarchy) { btn_Notice.GetComponent<ButtonControl>().OnBtnImageFadeIn(); }
                else if (ui_EventUpLoad.activeInHierarchy) { btn_EventUpLoad.GetComponent<ButtonControl>().OnBtnImageFadeIn(); }
                else if (ui_EventList.activeInHierarchy) { btn_EventList.GetComponent<ButtonControl>().OnBtnImageFadeIn(); }
                else if (ui_MiniGame.activeInHierarchy) { btn_MiniGame.GetComponent<ButtonControl>().OnBtnImageFadeIn(); }
                else if (ui_ExhibitionSpace.activeInHierarchy) { btn_ExhibitionSpace.GetComponent<ButtonControl>().OnBtnImageFadeIn(); }
                else if (ui_ScreenSetting.activeInHierarchy) { btn_ScreenSetting.GetComponent<ButtonControl>().OnBtnImageFadeIn(); }
                else if (ui_BannerSetting.activeInHierarchy) { btn_BannerSetting.GetComponent<ButtonControl>().OnBtnImageFadeIn(); }
                else if (ui_OperationalityBoard.activeInHierarchy) { btn_OperationalityBoard.GetComponent<ButtonControl>().OnBtnImageFadeIn(); }

                AllUIClose();
                dev_ChannelReset.SetActive(true);
            }
        }

        public void AllUIClose()
        {
            if (ui_Notice) ui_Notice.SetActive(false);
            if (ui_OperationalityBoard) ui_OperationalityBoard.SetActive(false);

            if (ui_EventUpLoad) ui_EventUpLoad.SetActive(false);
            if (ui_EventList) ui_EventList.SetActive(false);

            if (ui_MiniGame) ui_MiniGame.SetActive(false);
            if (ui_NPC) ui_NPC.SetActive(false);

            if (ui_ExhibitionSpace) ui_ExhibitionSpace.SetActive(false);
            if (ui_Museum) ui_Museum.SetActive(false);
            if (ui_ScreenSetting) ui_ScreenSetting.SetActive(false);
            if (ui_BannerSetting) ui_BannerSetting.SetActive(false);
            if (ui_ChildrenPicture) ui_ChildrenPicture.SetActive(false);
            if (ui_SSMuseum) ui_SSMuseum.SetActive(false);

            if (ui_BestSellerVoteSet) ui_BestSellerVoteSet.SetActive(false);
            if (ui_BestSellerVote) ui_BestSellerVote.SetActive(false);

            if (ui_GuestBook) ui_GuestBook.SetActive(false);
            if (ui_BookRecommendation) ui_BookRecommendation.SetActive(false);

            if (dev_ChannelReset) dev_ChannelReset.SetActive(false);
        }

        public void BtnNoticeClick()
        {
            ui_Notice.SetActive(true);
            btn_Notice.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnOperationalityBoardClick()
        {
            ui_OperationalityBoard.SetActive(true);
            btn_OperationalityBoard.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnEventUpLoadClick()
        {
            ui_EventUpLoad.SetActive(true);
            btn_EventUpLoad.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnEventListClick()
        {
            ui_EventList.SetActive(true);
            btn_EventList.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnMiniGameClick()
        {
            ui_MiniGame.SetActive(true);
            btn_MiniGame.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnNPCClick()
        {
            ui_NPC.SetActive(true);
            btn_NPC.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnBestSellerVoteSetClick()
        {
            ui_BestSellerVoteSet.SetActive(true);
            btn_BestSellerVoteSet.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnBestSellerVoteClick()
        {
            ui_BestSellerVote.SetActive(true);
            btn_BestSellerVote.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnExhibitionSpaceClick()
        {
            ui_ExhibitionSpace.SetActive(true);
            btn_ExhibitionSpace.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnMuseumClick()
        {
            ui_Museum.SetActive(true);
            btn_Museum.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnScreenSettingClick()
        {
            ui_ScreenSetting.SetActive(true);
            btn_ScreenSetting.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnBannerSettingClick()
        {
            ui_BannerSetting.SetActive(true);
            btn_BannerSetting.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnChildrenPictureClick()
        {
            ui_ChildrenPicture.SetActive(true);
            btn_ChildrenPicture.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnSSMuseumClick()
        {
            ui_SSMuseum.SetActive(true);
            btn_SSMuseum.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnGuestBookClick()
        {
            ui_GuestBook.SetActive(true);
            btn_GuestBook.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }

        public void BtnBookRecommendationClick()
        {
            ui_BookRecommendation.SetActive(true);
            btn_BookRecommendation.GetComponent<ButtonControl>().OnBtnImageFadeOut();
        }
    }
}