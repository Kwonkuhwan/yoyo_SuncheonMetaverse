using Suncheon.WebData;
using Suncheon.Player;
using UnityEngine;
using Suncheon.UI;
using Newtonsoft.Json.Linq;

namespace Suncheon
{
    public class MuseumObject : MonoBehaviour, Clickable
    {
        [SerializeField] GameObject glassObject;
        [SerializeField] Material onMaterial;
        [SerializeField] Material offMaterial;

        [SerializeField] Material glassMaterial;

        [SerializeField] bool isOn;

        [SerializeField] MeshRenderer[] frameMeshRenderers;

        private void Awake()
        {
            isOn = false;

            glassMaterial = glassObject.GetComponent<MeshRenderer>().materials[0];
            glassMaterial.color = offMaterial.color;

            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrMuseumImageLoad}", (jsonData) =>
            //StartCoroutine(UTILS.Requset_HttpGetData($"https://metalibrary.suncheon.go.kr/suncheonlib/frnt/mngr/museum.ax", (jsonData) =>
            {
                JArray jArray = null;
                try { jArray = JArray.Parse(jsonData); }
                catch { jArray = null; }

                if (jArray == null) return;

                SetImage(jArray);
            }));
        }

        private void SetImage(JArray jArray)
        {
            foreach (var jobj in jArray)
            {
                Response_MuseumLoad response_MuseumLoad = null;
                try { response_MuseumLoad = JsonUtility.FromJson<Response_MuseumLoad>(jobj.ToString()); }
                catch { continue; }

                if (response_MuseumLoad == null) continue;

                StartCoroutine(UTILS.Requset_HttpGetTexture($"{GameManager.Instance.defaultData.fileUrl}{response_MuseumLoad.img}", (textureData) =>
                //StartCoroutine(UTILS.Requset_HttpGetTexture($"https://metalibrary.suncheon.go.kr/upload/{response_MuseumLoad.img}", (textureData) =>
                {
                    SetMaterialImage(response_MuseumLoad.museumSeq, textureData);
                }));
            }
        }

        private void SetMaterialImage(int museumSeq, Texture2D texture)
        {
            if (museumSeq == 6 || museumSeq == 10)
            {                
                frameMeshRenderers[museumSeq - 1].materials[1].mainTexture = texture;
            }
            else
            {
                frameMeshRenderers[museumSeq - 1].material.mainTexture = texture;
            }
        }

        public void OnClick()
        {
            if (UIInteractionManager.Instance.OpenPopUps.Count > 0) return;

            if (!isOn)
            {
                //glassObject.GetComponent<MeshRenderer>().materials[0] = onMaterial;
                glassMaterial.color = onMaterial.color;
                isOn = true;
            }
            else
            {
                //glassObject.GetComponent<MeshRenderer>().materials[0] = offMaterial;
                glassMaterial.color = offMaterial.color;
                isOn = false;
            }
        }

        [SerializeField] string obj_Name;
        public string Return_ObjName()
        {
            return obj_Name;
        }
    }
}