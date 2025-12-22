using Newtonsoft.Json.Linq;
using Suncheon.WebData;
using UnityEngine;

namespace Suncheon.Admin
{
    public class UI_GuestBook : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject go_GuestBookInfo;

        private void ScrollViewClear()
        {
            foreach( var obj in content.GetComponentsInChildren<AdminGuestBookInfo>()) 
            {
                Destroy(obj);
            }
            content.DetachChildren();
        }

        private void OnEnable()
        {
            LoadGuestBookList();
        }

        private void LoadGuestBookList()
        {
            ScrollViewClear();

            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrGBookOrRBookLoad}", "type=C", (jsonData) =>
            {
                JArray jarray = JArray.Parse(jsonData);
                if (jarray == null) return;

                SetScrollView(jarray);
            }));
        }

        private void SetScrollView(JArray jArray)
        {
            foreach(var info in jArray)
            {
                Response_GBorRBLoad response_GBLoad = JsonUtility.FromJson<Response_GBorRBLoad>(info.ToString());
                if (response_GBLoad == null) continue;

                GameObject obj = Instantiate(go_GuestBookInfo, content);
                obj.GetComponent<AdminGuestBookInfo>().Init(int.Parse(response_GBLoad.reportSeq), response_GBLoad.writerName, response_GBLoad.reporterName, response_GBLoad.reportComm);
            }
        }
    }
}
