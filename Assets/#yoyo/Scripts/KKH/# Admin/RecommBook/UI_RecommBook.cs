using Newtonsoft.Json.Linq;
using Suncheon.WebData;
using UnityEngine;

namespace Suncheon.Admin
{
    public delegate void ShowUIRecommBookInfo();

    public class UI_RecommBook : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject go_RecommBookInfo;

        [SerializeField] private UI_RecommBookInfo ui_RecommBookInfo;

        private void ScrollViewClear()
        {
            foreach (var obj in content.GetComponentsInChildren<AdminRecommBookInfo>())
            {
                Destroy(obj);
            }
            content.DetachChildren();
        }

        private void OnEnable()
        {
            LoadRecommBookList();
        }

        public void LoadRecommBookList()
        {
            ScrollViewClear();

            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrGBookOrRBookLoad}", "type=R", (jsonData) =>
            {
                JArray jarray = JArray.Parse(jsonData);
                if (jarray == null) return;

                SetScrollView(jarray);
            }));
        }

        private void SetScrollView(JArray jArray)
        {
            foreach (var info in jArray)
            {
                Response_GBorRBLoad response_GBLoad = JsonUtility.FromJson<Response_GBorRBLoad>(info.ToString());
                if (response_GBLoad == null|| string.IsNullOrEmpty(response_GBLoad.reportComm) || string.IsNullOrEmpty(response_GBLoad.writerName)) continue;

                GameObject obj = Instantiate(go_RecommBookInfo, content);
                obj.GetComponent<AdminRecommBookInfo>().Init(int.Parse(response_GBLoad.reportSeq), int.Parse(response_GBLoad.reportId), response_GBLoad.writerName, response_GBLoad.reporterName, response_GBLoad.reportComm, ui_RecommBookInfo);
            }
        }
    }
}