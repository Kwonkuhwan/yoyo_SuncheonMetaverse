using Photon.Pun;
using Suncheon.Player;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Suncheon.UI
{
    public class UI_ChannelInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text text_MapLocationName;
        [SerializeField] private TMP_Text text_ChannelNumber;
        [SerializeField] private TMP_Text text_PlayerCnt;

        private float updateChannelInfoCoolTime = 0.0f;
        public float updateChannelInfoCoolMaxTime = 5.0f;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            foreach (var chplayerCnt in NetworkManager.Instance.ChannelPlayerCnts)
            {
                UTILS.Log($"{chplayerCnt.Key} : {chplayerCnt.Value}");
                text_ChannelNumber.text = $"{chplayerCnt.Key}";
                text_PlayerCnt.text = $"{chplayerCnt.Value}";
            }
        }

        private void Update()
        {
            if (NetworkManager.Instance == null) return;

            if (PhotonNetwork.CurrentRoom == null) return;

            updateChannelInfoCoolTime += Time.deltaTime;

            if (NetworkManager.Instance.IsCheckChCntChange == true || (NetworkManager.Instance.ChannelPlayerCnts.Count > 0 || updateChannelInfoCoolTime > updateChannelInfoCoolMaxTime))
            {
                int playerCnt = 0;
                int channel = NetworkManager.Instance.channel;
                //if (SceneManager.GetActiveScene().buildIndex == (int)SceneName.광장)
                //{
                //    NetworkManager.Instance.ChannelPlayerCnts.TryGetValue($"CH {channel}", out playerCnt);
                //}
                //else
                //{
                playerCnt = PhotonNetwork.CurrentRoom.PlayerCount;
                //}

                text_ChannelNumber.text = $"CH {channel}";
                text_PlayerCnt.text = playerCnt.ToString();
                updateChannelInfoCoolTime = 0.0f;
                NetworkManager.Instance.IsCheckChCntChange = false;
            }


        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            NetworkManager networkManager = NetworkManager.Instance;
            string mapLocationName = string.Empty;

            if (scene.name.Equals("02_Garden_Scene"))
            {
                mapLocationName = $"메인광장";
            }
            else if (scene.name.Equals("03_Inside") || scene.name.Equals("05_AVRoom"))
            {
                if (networkManager.SpPos == SpawnerPos.중그룹동아리 || networkManager.SpPos == SpawnerPos.소그룹동아리)
                {
                    mapLocationName = $"{networkManager.ClubName}\n{networkManager.SpPos}";
                }
                else
                {
                    if (networkManager.LibPos != LibName.None)
                    {
                        mapLocationName = networkManager.LibPos.ToString();
                    }

                    if ((int)networkManager.LibPos == (int)networkManager.SpPos)
                    {
                        mapLocationName += $"\n특화공간";
                    }
                    else
                    {
                        mapLocationName += $"\n{networkManager.SpPos}";
                    }
                }
            }
            else if (scene.name.Equals("04_Myroom"))
            {
                if (NetworkManager.Instance.Check_MyRoom())
                {
                    mapLocationName = $"{PhotonNetwork.NickName}님의\n개인서재";
                }
                else
                {
                    mapLocationName = $"{NetworkManager.Instance.user_name}님의\n개인서재";
                }

                
            }

            text_MapLocationName.text = mapLocationName;
        }
    }
}