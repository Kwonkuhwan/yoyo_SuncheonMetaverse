using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Suncheon
{
    public class RoomData : MonoBehaviour
    {
        [SerializeField] private TMP_Text text_RoomName;
        [SerializeField] private TMP_Text text_PlayerCnt;
        [SerializeField] private Image image_PlayerCnt;
        [SerializeField] private Sprite[] sprite_PlayerCnt;
        [SerializeField] private RoomInfo _roomInfo;

        private void Awake()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() => OnEnterRoom(text_RoomName.text));
        }

        public void SetRoomData(string roomName, int playerCnt = 0)
        {
            text_RoomName.text = roomName;
            text_PlayerCnt.text = playerCnt.ToString();

            if (playerCnt < 5)
            {
                image_PlayerCnt.sprite = sprite_PlayerCnt[0];
            }
            else if (playerCnt >= 5 && playerCnt < 10)
            {
                image_PlayerCnt.sprite = sprite_PlayerCnt[1];
            }
            else if (playerCnt >= 10 && playerCnt < 20)
            {
                image_PlayerCnt.sprite = sprite_PlayerCnt[2];
            }
            else if (playerCnt >= 20)
            {
                image_PlayerCnt.sprite = sprite_PlayerCnt[3];
            }
        }

        public RoomInfo RoomInfo
        {
            get { return _roomInfo; }
            set
            {
                _roomInfo = value;
                text_RoomName.text = $"{_roomInfo.Name}";
                text_PlayerCnt.text = $"{_roomInfo.PlayerCount}";

                if (_roomInfo.PlayerCount < 3)
                {
                    image_PlayerCnt.sprite = sprite_PlayerCnt[0];
                }
                else if (_roomInfo.PlayerCount >= 3 && _roomInfo.PlayerCount < 5)
                {
                    image_PlayerCnt.sprite = sprite_PlayerCnt[1];
                }
                else if (_roomInfo.PlayerCount >= 5 && _roomInfo.PlayerCount < 7)
                {
                    image_PlayerCnt.sprite = sprite_PlayerCnt[2];
                }
                else if (_roomInfo.PlayerCount >= 7)
                {
                    image_PlayerCnt.sprite = sprite_PlayerCnt[3];
                }
                GetComponent<Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
            }
        }

        private void OnEnterRoom(string channel)
        {
            string[] ch = channel.Split(' ');
            StartCoroutine(CoroutineEnterRoom(ch[1]));
        }

        private IEnumerator CoroutineEnterRoom(string channel)
        {
            NetworkManager.Instance.OnLeaveRoom();
            if (SceneManager.GetActiveScene().name.Equals("02_Garden_Scene"))
            {
                NetworkManager.Instance.SetSpawnerPos(SpawnerPos.오천그린광장);
            }

            NetworkManager.Instance.channel = int.Parse(channel);
            UTILS.LoadingSceneLoad(SceneManager.GetActiveScene().name);
            //StartCoroutine(NetworkManager.Instance.JoinRoom(channelName, "", SpawnerPos.오천그린광장));
            yield return new WaitForSeconds(1);
        }
    }
}