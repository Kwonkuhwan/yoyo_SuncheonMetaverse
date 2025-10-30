using Photon.Pun;
using Suncheon;
using Suncheon.Player;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class UI_Inside_Load : MonoBehaviour
    {
        [SerializeField] private SpawnerPos spawnerPos;
        [SerializeField] private Button btn_LibRoom;
        [SerializeField] private Button btn_AvRoom;
        [SerializeField] private Button btn_ChildrensRoom;
        [SerializeField] private Button btn_ReferenceRoom;

        private void Awake()
        {
            if (btn_LibRoom != null) btn_LibRoom.onClick.AddListener(() => LoadLibRoomClick());
            if (btn_AvRoom != null) btn_AvRoom.onClick.AddListener(() => LoadAVRoomClick());
            if (btn_ChildrensRoom != null) btn_ChildrensRoom.onClick.AddListener(() => LoadChildrensRoomClick());
            if (btn_ReferenceRoom != null) btn_ReferenceRoom.onClick.AddListener(() => LoadReferenceRoomClick());
        }

        public void LoadLibRoomClick()
        {
            if (NetworkManager.Instance.SpPos == spawnerPos)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"이미 {NetworkManager.Instance.LibPos}의 특화공간입니다.");
                return;
            }

            if (spawnerPos == SpawnerPos.삼산도서관)
            {
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.lib1 += 1;
                Request_SSLibCntSet request_SSLibCntSet = new Request_SSLibCntSet(1);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userLibCntPlus}", request_SSLibCntSet, (jsonData) =>
                {

                }));
            }
            else if (spawnerPos == SpawnerPos.그림책도서관)
            {
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.lib2 += 1;
                Request_PBLibCntSet request_PBLibCntSet = new Request_PBLibCntSet(1);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userLibCntPlus}", request_PBLibCntSet, (jsonData) =>
                {

                }));
            }
            else if (spawnerPos == SpawnerPos.연향도서관)
            {
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.lib3 += 1;
                Request_YHLibCntSet request_YHLibCntSet = new Request_YHLibCntSet(1);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userLibCntPlus}", request_YHLibCntSet, (jsonData) =>
                {

                }));
            }
            else if (spawnerPos == SpawnerPos.기적의도서관)
            {
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.lib4 += 1;
                Request_MILibCntSet request_MILibCntSet = new Request_MILibCntSet(1);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userLibCntPlus}", request_MILibCntSet, (jsonData) =>
                {

                }));
            }            
            //else if(spawnerPos == SpawnerPos.조례호수도서관) // 특화 공간 미존재
            //{
            //    StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userLibCntPlus}", "lib5", 1, (jsonData) =>
            //    {

            //    }));
            //}
            else if (spawnerPos == SpawnerPos.신대도서관)
            {
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.lib6 += 1;
                Request_SDLibCntSet request_SDLibCntSet = new Request_SDLibCntSet(1);
                StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userLibCntPlus}", request_SDLibCntSet, (jsonData) =>
                {

                }));
            }

            NetworkManager.Instance.SetSpawnerPos(spawnerPos, (LibName)spawnerPos);
            OnLoadInside("03_Inside");            
        }

        public void LoadAVRoomClick()
        {
            if(NetworkManager.Instance.SpPos == SpawnerPos.시청각실)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"이미 {NetworkManager.Instance.LibPos}의 시청각실입니다.");
                return;
            }
            NetworkManager.Instance.SetSpawnerPos(SpawnerPos.시청각실, (LibName)spawnerPos);
            OnLoadInside("05_AVRoom");
        }

        public void LoadChildrensRoomClick()
        {
            if (NetworkManager.Instance.SpPos == SpawnerPos.어린이실)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"이미 {NetworkManager.Instance.LibPos}의 어린이실입니다.");
                return;
            }
            NetworkManager.Instance.SetSpawnerPos(SpawnerPos.어린이실, (LibName)spawnerPos);
            OnLoadInside("03_Inside");
        }

        public void LoadReferenceRoomClick()
        {
            if (NetworkManager.Instance.SpPos == SpawnerPos.자료실)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"이미 {NetworkManager.Instance.LibPos}의 자료실입니다.");
                return;
            }
            NetworkManager.Instance.SetSpawnerPos(SpawnerPos.자료실, (LibName)spawnerPos);
            OnLoadInside("03_Inside");
        }

        public void OnLoadInside(string loadSceneName)
        {
            NetworkManager.Instance.OnLeaveRoom();
            UTILS.LoadingSceneLoad(loadSceneName);
        }
    }
}