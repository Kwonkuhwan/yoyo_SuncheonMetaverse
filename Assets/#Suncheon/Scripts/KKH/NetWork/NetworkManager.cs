using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;
using System.Collections;
using Suncheon.WebData;
using Suncheon.Player;

namespace Suncheon
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private static NetworkManager instance;
        public static NetworkManager Instance => instance;

        [SerializeField] private GameObject[] photon_Prefabs;
        [SerializeField] private GameObject go_Player;
        public GameObject Go_Player => go_Player;

        //상호작용할 플레이어의 ID
        public string user_id;
        public string user_name;
        public string user_no;

        [Space(10)]
        #region 방 관련
        [Header("방 관련")]
        [SerializeField] private GameObject roomPrefab;
        private RoomOptions roomOptions;
        [SerializeField] private int maxPlayerCnt = 8;

        public int channel = 1;

        private int maxRoomCnt = 20;
        public int MaxRoomCnt => maxRoomCnt;

        public string channelName = string.Empty;
        #endregion

        [SerializeField] private Transform spawner = null;
        [SerializeField] private SpawnerPos spawnerPos = SpawnerPos.오천그린광장;
        public SpawnerPos SpPos
        {
            get { return spawnerPos; }
            set
            {
                spawnerPos = value;
            }
        }

        [SerializeField] private LibName libPos = LibName.None;
        public LibName LibPos
        {
            get { return libPos; }
            set
            {
                libPos = value;
            }
        }

        private bool isInitJoin = false;
        public bool IsInitJoin => isInitJoin;

        public bool IsCheckChCntChange = false;
        private Dictionary<string, int> channelPlayerCnts = new Dictionary<string, int>();
        public Dictionary<string, int> ChannelPlayerCnts
        {
            get
            {
                return channelPlayerCnts;
            }
        }

        public string ClubName = string.Empty;

        private void Awake()
        {
            if (instance == null || instance != this)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }

            // 포톤 네트워크 설정
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.KeepAliveInBackground = 300;
            PhotonNetwork.MaxResendsBeforeDisconnect = 8;
            PhotonNetwork.SerializationRate = 60;
            PhotonNetwork.SendRate = 60;

            // 씬로드했을때 함수 추가
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            // 방 옵션 설정
            roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayerCnt;
            roomOptions.IsOpen = true;

            // Phorton에서 사용할 프리펩 설정
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            pool.ResourceCache.Clear();
            foreach (GameObject prefab in photon_Prefabs)
            {
                pool.ResourceCache.Add(prefab.name, prefab);
            }

            // 포톤 연결
            ConnectPhoton();
        }

        public override void OnDisable()
        {
            // NetWorkManager 초기화 및 PhotonNetwork 연결 해제
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            PhotonNetwork.Disconnect();
        }

        public void OnDestroy()
        {
            if (PhotonNetwork.InRoom)
            {
                OnLeaveRoom();
            }

            // 인스턴스 초기화
            instance = null;
        }

        /// <summary>
        /// 포톤 연결
        /// </summary>
        public void ConnectPhoton()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        /// <summary>
        /// 포톤 연결 성공
        /// </summary>
        public override void OnConnectedToMaster()
        {
            JoinLobby();
        }

        /// <summary>
        /// 로비 접속
        /// </summary>
        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
        }

        /// <summary>
        /// 로비 접속 성공
        /// </summary>
        public override void OnJoinedLobby()
        {

        }

        /// <summary>
        /// 도서관 이름으로 방접속
        /// </summary>
        /// <param name="Ch"></param>
        /// <param name="roomName"></param>
        /// <param name="libName"></param>
        /// <returns></returns>
        public IEnumerator JoinRoom(string Ch, string roomName, LibName libName)
        {
            yield return null;
            spawnerPos = (SpawnerPos)libName;
            StartCoroutine(JoinRoom(Ch, roomName, spawnerPos));
        }

        /// <summary>
        /// 방 접속
        /// </summary>
        /// <param name="Ch"></param>
        /// <param name="roomName"></param>
        /// <param name="spPos"></param>
        /// <returns></returns>
        public IEnumerator JoinRoom(string Ch, string roomName, SpawnerPos spPos)
        {
            if (!isInitJoin)
            {
                isInitJoin = true;
                StartCoroutine(ChannelCntCheck());
                yield return new WaitForSeconds(0.5f);
            }

            if (string.IsNullOrEmpty(Ch))
            {
                channelName = $"CH_{channel}";
            }
            else
            {
                channelName = Ch;
            }

//#if UNITY_EDITOR
//            channelName = $"CH_1_Test";
//#endif

            // 이미 방 안이라면
            if (PhotonNetwork.InRoom)
            {
                OnLeaveRoom();
                yield return new WaitForSeconds(1);
            }

            // 로비에 나와있다면
            if (PhotonNetwork.InLobby)
            {
                if (channel >= maxRoomCnt)
                {
                    OnLeaveRoom();
                    yield return new WaitForSeconds(1.0f);
                    UTILS.LoadingSceneLoad("01_Title_Scene");
                }

                GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
                foreach (GameObject sp in spawners)
                {
                    if (sp.GetComponent<Spawner>().SpawnerPos == spPos)
                    {
                        spawner = sp.transform;
                        break;
                    }
                }

                string _roomName = string.Empty;
                if (string.IsNullOrEmpty(roomName))
                {
                    _roomName = channelName;
                }
                else
                {
                    _roomName = $"{channelName}_{roomName}";
                }

                PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, null);
            }
        }

        /// <summary>
        /// 채널 카운트 조회해서 꽉차있으면 다음채널
        /// </summary>
        /// <returns></returns>
        IEnumerator ChannelCntCheck()
        {
            StartCoroutine(GetChannelAllCnt());

            yield return new WaitForSeconds(0.5f);

            foreach (var channelCnt in channelPlayerCnts)
            {
                if (channelCnt.Value >= maxPlayerCnt)
                {
                    channel++;
                }
                else
                {
                    break;
                }
            }

            UTILS.Log($"ChannelCntCheck : {channel}");
            IsCheckChCntChange = true;

            yield return null;
        }

        /// <summary>
        /// 전체 채널 접속 인원 조회
        /// </summary>
        /// <returns></returns>
        IEnumerator GetChannelAllCnt()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.channalCntUrl}", (jsonData) =>
            {
                Response_ChannelUserCnt response_ChannelUserCnt = JsonUtility.FromJson<Response_ChannelUserCnt>(jsonData);
                if (response_ChannelUserCnt == null) return;

                SetChannelPlayerCnts(response_ChannelUserCnt);
            }));

            yield return null;
        }

        private void SetChannelPlayerCnts(Response_ChannelUserCnt response_ChannelUserCnt)
        {
            channelPlayerCnts.Clear();
            maxRoomCnt = int.Parse(response_ChannelUserCnt.channelCnt);

            foreach (Response_ChannelUserCntResultData channel in response_ChannelUserCnt.channleUserCnt)
            {
                channelPlayerCnts.Add($"CH {channel.channel}", int.Parse(channel.cnt));
            }
        }

        /// <summary>
        /// 방 생성 성공
        /// </summary>
        public override void OnCreatedRoom()
        {
            UTILS.Log($"CreateRoom : {PhotonNetwork.CurrentRoom.Name}");
        }

        /// <summary>
        /// 방 생성 실패
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            UTILS.Log(message);
        }

        /// <summary>
        /// 방 접속 실패
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            channel++;
            StartCoroutine(JoinRoom($"CH_{channel}", "", SpawnerPos.오천그린광장));
        }

        /// <summary>
        /// 방 접속 성공
        /// </summary>
        public override void OnJoinedRoom()
        {
            UTILS.Log($"{MethodBase.GetCurrentMethod().Name} : {PhotonNetwork.CurrentRoom.Name}");

            int channel = this.channel;
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.channalCntPlusUrl}", $"channel={channel}", (jsonData) =>
            {
                Response_ChannelUserCntPlus response_ChannelUserCntPlus = JsonUtility.FromJson<Response_ChannelUserCntPlus>(jsonData);
                if (response_ChannelUserCntPlus.rtnCode == "999")
                {
                    UTILS.Log($"CH {channel} 접속 실패");
                    StartCoroutine(JoinRoom(channel.ToString(), "", SpawnerPos.오천그린광장));
                    return;
                }
                else
                {
                    UTILS.Log($"CH {channel} 접속 성공");
                }
            }));

            go_Player = PhotonNetwork.Instantiate("RuntimeCharacter", spawner.position, Quaternion.identity);
            //go_Player = PhotonNetwork.Instantiate("Character", spawner.position, Quaternion.identity);
            if (SceneManager.GetActiveScene().name.Equals("02_Garden_Scene"))
            {
                SpPos = SpawnerPos.None;
                LibPos = LibName.None;
            }
        }

        /// <summary>
        /// 방 떠남
        /// </summary>
        /// <returns></returns>
        public bool OnLeaveRoom()
        {
            foreach (string str in PhotonNetwork.CurrentRoom.Name.Split('_'))
            {
                UTILS.Log(str);
            }

            int channel = int.Parse(PhotonNetwork.CurrentRoom.Name.Split('_')[1]);
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.channalCntMinusUrl}", $"channel={channel}", (jsonData) =>
            {
                Response_ChannelUserCntMinus response_ChannelUserCntMinus = JsonUtility.FromJson<Response_ChannelUserCntMinus>(jsonData);
                if (response_ChannelUserCntMinus.rtnCode == "999")
                {
                    //StartCoroutine(JoinRoom(channel.ToString(), "", SpawnerPos.오천그린광장));
                    UTILS.Log($"{channel} 감소 실패");
                }
                else
                {
                    UTILS.Log($"{channel} 감소 확인");
                }
            }));

            return PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Return_To_Robby()
        {
            Instance.SetSpawnerPos(SpawnerPos.오천그린광장);
            UTILS.LoadingSceneLoad("02_Garden_Scene");
            PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// 방 떠남(포톤)
        /// </summary>
        public override void OnLeftRoom()
        {
            UTILS.Log("방 나감");
            go_Player = null;
            //OnLeaveRoom();
        }

        /// <summary>
        /// 플레이어 입장
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            UTILS.Log($"{MethodBase.GetCurrentMethod().Name} : newPlayer NickName [{newPlayer.NickName}]");
            StartCoroutine(GetChannelAllCnt());
        }

        /// <summary>
        /// 플레이어 스폰 위치 설정
        /// </summary>
        /// <param name="spPos"></param>
        /// <param name="libPos"></param>
        public void SetSpawnerPos(SpawnerPos spPos, LibName libPos = LibName.None)
        {
            SpPos = spPos;
            LibPos = libPos;
        }

        /// <summary>
        /// 씬변경
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string roomName = string.Empty;

            if (scene.name.Equals("02_Garden_Scene"))
            {
                if (LibPos == LibName.None)
                {
                    StartCoroutine(JoinRoom($"", roomName, spawnerPos));
                }
                else
                {
                    StartCoroutine(JoinRoom($"", roomName, LibPos));
                }
            }
            else if (scene.name.Equals("03_Inside") || scene.name.Equals("05_AVRoom"))
            {
                if (LibPos == LibName.삼산도서관)
                {
                    roomName = "Inside_SS";
                }
                else if (LibPos == LibName.그림책도서관)
                {
                    roomName = "Inside_PB";
                }
                else if (LibPos == LibName.연향도서관)
                {
                    roomName = "Inside_YH";
                }
                else if (LibPos == LibName.기적의도서관)
                {
                    roomName = "Inside_MI";
                }
                else if (LibPos == LibName.조례호수도서관)
                {
                    roomName = "Inside_JL";
                }
                else if (LibPos == LibName.신대도서관)
                {
                    roomName = "Inside_SD";
                }

                if (SpPos == SpawnerPos.소그룹동아리 || SpPos == SpawnerPos.중그룹동아리)
                {
                    roomName = ClubName;
                }
                else if (SpPos == SpawnerPos.어린이실)
                {
                    roomName += "_ChildrenRoom";
                }
                else if (SpPos == SpawnerPos.자료실)
                {
                    roomName += "_ReferenceRoom";
                }
                else if (SpPos == SpawnerPos.시청각실)
                {
                    roomName += "_AVRoom";
                }

                StartCoroutine(JoinRoom($"{channelName}", roomName, SpPos));
            }
            else if (scene.name.Equals("04_Myroom"))
            {
                if (GameManager.Instance.VisitPlayerName == string.Empty)
                {
                    StartCoroutine(JoinRoom($"{channelName}", $"{PhotonNetwork.NickName}_Myroom", SpPos));
                }
                else
                {
                    StartCoroutine(JoinRoom($"{channelName}", $"{GameManager.Instance.VisitPlayerName}_Myroom", SpPos));
                    GameManager.Instance.VisitPlayerName = string.Empty;
                }
            }
        }

        /// <summary>
        /// 현재 서재가 나의 서재인지 확인
        /// </summary>
        /// <returns>나의 서재일때 true</returns>
        public bool Check_MyRoom()
        {
            if (GameManager.Instance.loginData.user_id != user_id)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 어플리케이션 종료
        /// </summary>
        private void OnApplicationQuit()
        {
            if (PhotonNetwork.InRoom)
            {
                OnLeaveRoom();
            }
        }
    }
}