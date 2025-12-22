using UnityEngine;
using Suncheon.Player;
using Suncheon.WebData;
using Newtonsoft.Json.Linq;
using TMPro;

namespace Suncheon.UI
{
    public class UI_ClubMembers : MonoBehaviour
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private Transform content_ClubMembers;
        [SerializeField] private GameObject clubMemeberObject;

        [SerializeField] private TMP_Text text_clubMemberCnt;

        public void ShowPopUp()
        {
            UIInteractionManager.Instance.OpenPopUp(gameObject);
            if (playerManager == null)
            {
                playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            }

            ListUpdate();
        }

        public void SetClubMembers(Response_ClubPlayerCnt resultData)
        {
            text_clubMemberCnt.text = resultData.response_ClubPlayerCntResultData.Count.ToString();
            foreach (Response_ClubPlayerCntResultData data in resultData.response_ClubPlayerCntResultData)
            {
                GameObject go = Instantiate(clubMemeberObject, content_ClubMembers);

                go.GetComponent<ClubMemberObject>().SetObject(data.nickname == playerManager.joinedClubData.nickname, data.nickname);
            }
        }

        public void ListUpdate()
        {
            foreach (ClubMemberObject child in content_ClubMembers.GetComponentsInChildren<ClubMemberObject>())
            {
                Destroy(child.gameObject);
            }

            Request_ClubPlayerCnt requset_ClubPlayerCnt = new Request_ClubPlayerCnt(playerManager.joinedClubData.groupSeq);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.getClubPeopleInfoUrl}", requset_ClubPlayerCnt, (jsonData) =>
            {
                JArray jArray = JArray.Parse(jsonData);
                Response_ClubPlayerCnt response_ClubPlayerCnt = new Response_ClubPlayerCnt();

                foreach (JObject jObject in jArray)
                {
                    response_ClubPlayerCnt.response_ClubPlayerCntResultData.Add(JsonUtility.FromJson<Response_ClubPlayerCntResultData>(jObject.ToString()));
                }

                SetClubMembers(response_ClubPlayerCnt);
            }));
        }
    }
}
