using Newtonsoft.Json.Linq;
using UnityEngine;
using Suncheon.WebData;


namespace Suncheon
{
    public class ChildrenEasel : MonoBehaviour
    {
        private Easel[] easels = null;

        void Start()
        {
            easels = GetComponentsInChildren<Easel>();
            SetEasel();
        }

        private void SetEasel()
        {
            
            // 잠깐 주석 처리
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrChildrenArtLoad}", (jsonData) =>
            //StartCoroutine(UTILS.Requset_HttpGetData($"https://metalibrary.suncheon.go.kr/suncheonlib/frnt/mngr/childrenArt.ax", (jsonData) =>
            {
                UTILS.Log($"SetEasel : {jsonData}");
                JArray jArray = null;
                try
                {
                    jArray = JArray.Parse(jsonData);
                }
                catch
                {
                    jArray = null;
                }

                for (int i = 0; i < easels.Length; i++)
                {
                    Response_ChildrenEaselLoad response_ChildrenEaselLoad = null;
                    try
                    {
                        response_ChildrenEaselLoad = JsonUtility.FromJson<Response_ChildrenEaselLoad>(jArray[i].ToString());
                    }
                    catch
                    {
                        response_ChildrenEaselLoad = null;
                    }

                    if (response_ChildrenEaselLoad == null) return;

                    easels[i].Init(response_ChildrenEaselLoad);
                }
            }));
        }
    }
}