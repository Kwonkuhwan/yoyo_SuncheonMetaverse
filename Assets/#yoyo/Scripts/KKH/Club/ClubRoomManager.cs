using Newtonsoft.Json.Linq;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suncheon
{
    public class ClubRoomManager : MonoBehaviour
    {
        [SerializeField] private GameObject floor_Obj;
        [Header("소그룹 머테리얼")]
        [SerializeField] private Material[] floor_S_Materials;
        [SerializeField] private GameObject[] club_S_Objs;


        [Header("중그룹 머테리얼")]
        [SerializeField] private Material[] floor_M_Materials;      //0 : basic, 1 : BlackAndWhite, 2 : GreenAndWhite
        [SerializeField] private GameObject[] club_M_Objs;


        // Start is called before the first frame update
        void Start()
        {
            if (NetworkManager.Instance.SpPos == SpawnerPos.소그룹동아리 || NetworkManager.Instance.SpPos == SpawnerPos.중그룹동아리)
            {
                StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.joinedClubInfoUrl}", (jsonData) =>
                {
                    JArray jArray = JArray.Parse(jsonData);
                    if (jArray == null || jArray.First == null) { return; }
                    else
                    {
                        Response_JoinedGroupListResultData _resultData = JsonUtility.FromJson<Response_JoinedGroupListResultData>(jArray.First.ToString());
                        if (_resultData == null) return;
                        SetClub(_resultData);
                    }
                }));
            }
        }

        private void SetClub(Response_JoinedGroupListResultData _resultData)
        {
            if (_resultData.clubType == "S")
            {
                foreach (GameObject obj in club_S_Objs)
                {
                    obj.SetActive(false);
                }

                floor_Obj.GetComponent<MeshRenderer>().materials[0] = floor_S_Materials[int.Parse(_resultData.clubTemplateCode) - 1];
                club_S_Objs[int.Parse(_resultData.clubTemplateCode) - 1].SetActive(true);
            }
            else if (_resultData.clubType == "M")
            {
                foreach (GameObject obj in club_M_Objs)
                {
                    obj.SetActive(false);
                }

                floor_Obj.GetComponent<MeshRenderer>().materials[0] = floor_M_Materials[int.Parse(_resultData.clubTemplateCode) - 1];
                club_M_Objs[int.Parse(_resultData.clubTemplateCode) - 1].SetActive(true);
            }
        }
    }
}
