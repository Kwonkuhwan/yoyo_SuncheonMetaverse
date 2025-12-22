using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Suncheon
{
    public class LoadingScripts : MonoBehaviour
    {
        AsyncOperationHandle<SceneInstance> handle;

        [SerializeField] private Image image_LoadingBg;
        [SerializeField] private Image image_loading;
        [SerializeField] private float loadingSpeed = 100.0f;

        [SerializeField] private Sprite[] sprites_loading;

        [SerializeField] private float loadingTime;
        [Range(0.0f, 100.0f)]
        [SerializeField] private float loadingGardenMaxTime;

        [Range(0.0f, 100.0f)]
        [SerializeField] private float loadingOuthermaxtime;

        [SerializeField] private GameObject loadingDelayNotice;
        [Range(0.0f, 30.0f)]
        [SerializeField] private float loadingDelay = 30.0f;

        [SerializeField] private TMP_Text text_LodingText;

        private void Start()
        {
            loadingTime = 0.0f;
            image_LoadingBg.sprite = sprites_loading[Random.Range(0, sprites_loading.Length)];
            StartCoroutine(SceneLoad());
        }

        private void Update()
        {
            if (loadingTime < loadingGardenMaxTime)
            {
                loadingTime += Time.deltaTime;
            }

            if (loadingTime > loadingDelay)
            {
                loadingDelayNotice.SetActive(true);
            }

            image_loading.transform.Rotate(0.0f, 0.0f, -loadingSpeed * Time.deltaTime);
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR
            UnloadSceneAysnc();
#endif
        }


        private void SceneDownloadComplete(AsyncOperationHandle<SceneInstance> _handle)
        {
            if (_handle.Status == AsyncOperationStatus.Succeeded)
            {
                UTILS.Log($"OnSceneLoaded Done.");
                handle = _handle;
            }
            else
            {
                UTILS.Log("로드 실패");
            }
        }

        private void UnloadSceneAysnc()
        {
            Addressables.UnloadSceneAsync(handle, true).Completed +=
                (op) =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                    }
                };
        }

        private IEnumerator SceneLoad()
        {
            if (GameManager.Instance.nextSceneName == "01_Title_Scene")
            {
                yield return new WaitForSeconds(3.0f);
                Destroy(GameManager.Instance.gameObject);
                Destroy(NetworkManager.Instance.gameObject);
                SceneManager.LoadSceneAsync("01_Title_Scene", LoadSceneMode.Single);
            }

            if (GameManager.Instance != null)
            {
                StartCoroutine(LoadingText());

                yield return new WaitForSeconds(3.0f);

#if UNITY_EDITOR
                SceneManager.LoadSceneAsync(GameManager.Instance.nextSceneName, LoadSceneMode.Single);
#else
                var downloadScene = Addressables.LoadSceneAsync(GameManager.Instance.nextSceneName, LoadSceneMode.Single);
                downloadScene.Completed += SceneDownloadComplete;
#endif
                StartCoroutine(Downloading());
            }
        }

        IEnumerator Downloading()
        {
            while (!handle.IsDone)
            {
                yield return new WaitForSeconds(0.1f);
                var status = handle.GetDownloadStatus();
                float progress = status.Percent;
                UTILS.Log((progress * 100).ToString());
            }
            yield return null;
        }

        IEnumerator LoadingText()
        {
            string movePosText = string.Empty;
            if (GameManager.Instance.nextSceneName == "03_Inside" || GameManager.Instance.nextSceneName == "05_AVRoom")
            {
                string spPosText = string.Empty;
                if (NetworkManager.Instance.SpPos == SpawnerPos.중그룹동아리 || NetworkManager.Instance.SpPos == SpawnerPos.소그룹동아리)
                {
                    movePosText = $"{NetworkManager.Instance.SpPos} 이동중";
                }
                else if (NetworkManager.Instance.LibPos.ToString() == NetworkManager.Instance.SpPos.ToString())
                {
                    spPosText = "특화공간";
                    movePosText = $"{NetworkManager.Instance.LibPos}의 {spPosText} 이동중";
                }
                else
                {
                    spPosText = NetworkManager.Instance.SpPos.ToString();
                    movePosText = $"{NetworkManager.Instance.LibPos}의 {spPosText} 이동중";
                }

            }
            else if (GameManager.Instance.nextSceneName == "04_Myroom")
            {
                string userRoom = NetworkManager.Instance.user_name;
                if (string.IsNullOrEmpty(userRoom))
                {
                    movePosText = $"{PhotonNetwork.NickName}의 개인서재로 이동중";
                }
                else
                {
                    movePosText = $"{userRoom}의 개인서재로 이동중";
                }
            }
            else if (GameManager.Instance.nextSceneName == "02_Garden_Scene")
            {
                //movePosText = $"{NetworkManager.Instance.SpPos}으로 이동중";
                movePosText = $"오천그린광장으로 이동중";
            }
            else if(GameManager.Instance.nextSceneName == "01_Title_Scene")
            {
                movePosText = $"로그아웃 중";
            }

            string dotText = string.Empty;
            int cnt = 0;
            while (true)
            {
                if (cnt >= 3)
                {
                    cnt = 0;
                    dotText = string.Empty;
                }

                dotText += ".";
                cnt++;

                text_LodingText.text = movePosText + dotText;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}