using Photon.Pun;
using Suncheon.Player;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class UI_Mission : MonoBehaviour
    {
        [SerializeField] private Sprite[] comp_Sprites;
        [SerializeField] private Sprite[] noneComp_Sprites;

        [SerializeField] private Image image01;
        [SerializeField] private Image image02;
        [SerializeField] private Image image03;
        [SerializeField] private Image image04;
        [SerializeField] private Image image05;

        [SerializeField] private Image image01_Com;
        [SerializeField] private Image image02_Com;
        [SerializeField] private Image image03_Com;
        [SerializeField] private Image image04_Com;
        [SerializeField] private Image image05_Com;

        [SerializeField] private Image image_Comp;
        [SerializeField] private TMP_Text text_Comp;

        PlayerManager playermanager;
        private void OnEnable()
        {
            if(playermanager == null ) playermanager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            SetMissionInfo();
        }

        private void SetMissionInfo()
        {
            NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().UpdateMission();
            Response_MissionInfo missionInfo = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().missionInfo;

            if (missionInfo.mission1 == 0) image01.sprite = noneComp_Sprites[0];
            else
            {
                image01.sprite = comp_Sprites[0];
                image01_Com.gameObject.SetActive(true);
            }

            if (missionInfo.mission2 == 0) image02.sprite = noneComp_Sprites[1];
            else
            {
                image02.sprite = comp_Sprites[1];
                image02_Com.gameObject.SetActive(true);
            }

            if (missionInfo.mission3 == 0) image03.sprite = noneComp_Sprites[2];
            else
            {
                image03.sprite = comp_Sprites[2];
                image03_Com.gameObject.SetActive(true);
            }

            if (missionInfo.mission4 == 0) image04.sprite = noneComp_Sprites[3];
            else
            {
                image04.sprite = comp_Sprites[3];
                image04_Com.gameObject.SetActive(true);
            }

            if (missionInfo.mission5 == 0) image05.sprite = noneComp_Sprites[4];
            else
            {
                image05.sprite = comp_Sprites[4];
                image05_Com.gameObject.SetActive(true);
            }

            if (missionInfo.mission1 == 1 && missionInfo.mission2 == 1 && missionInfo.mission3 == 1 && missionInfo.mission4 == 1 && missionInfo.mission5 == 1)
            {
                image_Comp.gameObject.SetActive(true);
                text_Comp.text = $"[{PhotonNetwork.NickName}]\n네가 최고야!!";
            }
            else
            {
                image_Comp.gameObject.SetActive(false);
            }
        }
    }
}
