using UnityEngine;
using Suncheon.WebData;
using System.IO;
using Suncheon.UI;

namespace Suncheon
{
    public class BannerObject : MonoBehaviour, Clickable
    {
        [SerializeField] private Material bannerImageMaterial;
        [SerializeField] private MeshRenderer meshRenderer;

        [SerializeField] private Texture2D testTexture2D;
        [SerializeField] private string bannerURL = $"https://www.naver.com/";
        [SerializeField] string obj_Name= null;

        void Start()
        {
            SetBanner();
        }

        public void SetBanner()
        {
            // 잠깐 주석 처리
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrBannerLoad}", (jsonData) =>
            {
                UTILS.Log($"SetBanner : {jsonData}");
                Response_BannerLoad response_BannerLoad = null;
                try
                {
                    response_BannerLoad = JsonUtility.FromJson<Response_BannerLoad>(jsonData);
                }
                catch
                {
                    response_BannerLoad = null;
                }

                if (response_BannerLoad == null) return;

                try
                {
                    SetBannerURL(response_BannerLoad.banner_url);
                    string fileUrl = "";
                    if (string.IsNullOrEmpty(GameManager.Instance.defaultData.fileUrl.Trim()))
                    {
                        fileUrl = $"https://metalibrary.suncheon.go.kr/upload/";
                    }
                    else
                    {
                        fileUrl = GameManager.Instance.defaultData.fileUrl;
                    }

                    StartCoroutine(UTILS.Requset_HttpGetTexture($"{fileUrl}{response_BannerLoad.banner_img}", (textureData) =>
                    {
                        SetMaterialImage(textureData);
                    }));
                }
                catch
                {
                    return;
                }
            }));
        }

        private void SetBannerURL(string result)
        {
            UTILS.Log($"SetBannerURL : {result}");
            string modifiedUrl = result.StartsWith("http://") || result.StartsWith("https://") ? result : "http://" + result;
            bannerURL = modifiedUrl;
        }

        private void SetMaterialImage(Texture2D texture)
        {
            meshRenderer.materials[0].mainTexture = texture;
            meshRenderer.materials[1].mainTexture = texture;
            meshRenderer.materials[2].mainTexture = texture;
            meshRenderer.materials[3].mainTexture = texture;
        }

        private void SetMaterialImage(string base64Data)
        {
            byte[] imageBytes = System.Convert.FromBase64String(base64Data);
            Texture2D texture = UTILS.ByteArrayToTexture(imageBytes);
            SetMaterialImage(texture);
        }

        public void OnClick()
        {
            if (UIInteractionManager.Instance.OpenPopUps.Count > 0) return;

            if (string.IsNullOrEmpty(bannerURL)) return;
            Application.OpenURL(bannerURL);
        }

        public string Return_ObjName()
        {
            return obj_Name;
        }
    }
}
