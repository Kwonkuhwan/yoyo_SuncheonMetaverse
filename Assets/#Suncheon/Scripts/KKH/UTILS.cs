using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Suncheon
{
    public static class UTILS
    {
        public static void Log(string message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
        }

        public static void LoadingSceneLoad(string nextSceneName)
        {
            GameManager.Instance.nextSceneName = nextSceneName;
            SceneManager.LoadSceneAsync("99_Loading", LoadSceneMode.Single);
        }

        public static bool NickNameCheck(string nickName)
        {
            if (nickName == string.Empty) // || ���⿡ �Ƹ� �ܾ�, Ư������ �� ó��)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static string JsonToObject(object json)
        {
            return JsonUtility.ToJson(json);
        }

        public class CustomCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                // 인증서 유효성 검증 로직을 구현합니다.
                // 실제로 사용할 때에는 보다 안전한 방식으로 인증서 검증을 수행해야 합니다.
                // 이 예제는 모든 인증서를 신뢰하도록 true를 반환하는 간단한 예시입니다.
                return true;
            }
        }

        public static IEnumerator Requset_HttpGetData(string stServiceUrl, Action<string> callback)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpGetTexture(string stServiceUrl, Action<Texture2D> callback)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(stServiceUrl))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(DownloadHandlerTexture.GetContent(uwr));
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpGetData(string stServiceUrl, string param, Action<string> callback)
        {
            stServiceUrl = $"{stServiceUrl}?{param}";

            using (UnityWebRequest uwr = UnityWebRequest.Get(stServiceUrl))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, Action<string> callback)
        {
            using (UnityWebRequest uwr = UnityWebRequest.PostWwwForm(stServiceUrl, null))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, object postData, Action<string> callback)
        {
            string jsonData = JsonToObject(postData);
            using (UnityWebRequest uwr = UnityWebRequest.PostWwwForm(stServiceUrl, jsonData))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
                uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
                uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");

                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, string urlField, string url, string imageField, string imageName, byte[] imageBytes, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField(urlField, url);
            form.AddBinaryData(imageField, imageBytes, imageName, "image/jpeg");
            form.AddField("img_length", imageBytes.Length);

            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, string imageName, byte[] imageBytes, int location, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            form.AddBinaryData("img_file", imageBytes, imageName, "image/jpeg");
            form.AddField("img_length", imageBytes.Length);
            form.AddField("location", location);

            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, string urlField, string url, string imageField, string imageName, byte[] imageBytes, string locationField, string location, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField(urlField, url);
            if (Path.GetExtension(imageName).Equals(".png"))
            {
                form.AddBinaryData($"imageField", imageBytes, imageName, "image/png");
            }
            else if (Path.GetExtension(imageName).Equals(".jpg") || Path.GetExtension(imageName).Equals(".jpeg"))
            {
                form.AddBinaryData(imageField, imageBytes, imageName, "image/jpeg");
            }
            form.AddField("img_length", imageBytes.Length);
            form.AddField(locationField, location);

            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, List<string> imageNames, List<string> imageFileNames, List<byte[]> imageBytes, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            for (int i = 0; i < imageNames.Count; i++)
            {
                if (Path.GetExtension(imageFileNames[i]).Equals(".png"))
                {
                    form.AddBinaryData($"img_file{i + 1}", imageBytes[i], i.ToString(), "image/png");
                }
                else if (Path.GetExtension(imageFileNames[i]).Equals(".jpg") || Path.GetExtension(imageFileNames[i]).Equals(".jpeg"))
                {
                    form.AddBinaryData($"img_file{i + 1}", imageBytes[i], i.ToString(), "image/jpeg");
                }
                form.AddField($"file_length{i + 1}", imageBytes[i].Length);
                form.AddField($"art_title{i + 1}", imageNames[i]);
            }

            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, List<string> imageFileNames, List<byte[]> imageBytes, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            for (int i = 0; i < imageFileNames.Count; i++)
            {
                form.AddBinaryData($"img_file{i}", imageBytes[i], i.ToString(), "image/jpeg");
                form.AddField($"file_length{i}", imageBytes[i].Length);
            }

            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostDataMuseum(string stServiceUrl, List<string> imageLoactions, List<string> imageFileNames, List<byte[]> imageBytes, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            for (int i = 0; i < imageLoactions.Count; i++)
            {
                form.AddBinaryData($"{imageLoactions[i]}", imageBytes[i], i.ToString(), "image/jpeg");
                form.AddField($"n_length{i}", imageBytes[i].Length);
            }

            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, string csvField, string csvLengthField, string csvName, byte[] csvBytes, Action<string> callback)
        {
            //string jsonData =  Encoding.Default.GetString(File.ReadAllBytes("D:\\Work\\Suncheon-Metaverse\\01.Souces\\SuncheonMetaverse\\Assets\\#Suncheon\\Scripts\\KKH\\WebClass\\TestJson.json"));
            WWWForm form = new WWWForm();
            form.AddBinaryData(csvField, csvBytes, csvName, "text/csv");
            form.AddField(csvLengthField, csvBytes.Length);

            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, string voteTitleName, string voteStartTime, string voteEndTime, List<string> voteBookNames, List<string> voteimgFileNames, List<byte[]> voteimgBytes, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField("title", voteTitleName);
            form.AddField("vote_start_time", voteStartTime);
            form.AddField("vote_end_time", voteEndTime);

            for (int i = 0; i < voteBookNames.Count; i++)
            {
                int index = i + 1;
                form.AddField($"category_{index}", voteBookNames[i]);
                form.AddBinaryData($"category_file_{index}", voteimgBytes[i], voteimgFileNames[i], "image/jpeg");
                form.AddField($"category_{index}_length", voteimgBytes[i].Length);
            }


            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, string Field, int value, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField(Field, value);

            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }

        public static IEnumerator Requset_HttpPostData(string stServiceUrl, string Field, string value, Action<string> callback)
        {
            WWWForm form = new WWWForm();
            form.AddField(Field, value);

            using (UnityWebRequest uwr = UnityWebRequest.Post(stServiceUrl, form))
            {
                uwr.certificateHandler = new CustomCertificateHandler();
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Log($"Requset_HttpPostData Fail : {uwr.result}");
                }
                else
                {
                    if (uwr != null)
                    {
                        callback(uwr.downloadHandler.text);
                    }
                    yield return null;
                }
            }
        }


        public static Sprite ByteArrayToSprite(byte[] imageBytes)
        {
            Texture2D texture2D = new Texture2D(512, 512);
            texture2D.LoadImage(imageBytes);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        public static Texture2D ByteArrayToTexture(byte[] imageBytes)
        {
            Texture2D texture2D = new Texture2D(512, 512);
            texture2D.LoadImage(imageBytes);
            return texture2D;
        }
        public static Sprite ConvertToSprite(Texture2D texture)
        {
            // Texture2D를 Sprite로 변환하기 위해 Sprite 생성
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

            return sprite;
        }

        public static void CopyToClipBoard(string _copyText)
        {
            GUIUtility.systemCopyBuffer = _copyText;
        }

        public static void PasteToClipBoard(TMP_InputField inputField)
        {
            inputField.text = GUIUtility.systemCopyBuffer;
        }

        private static Texture2D ResizeTexture(Texture2D texture, int newWidth, int newHeight)
        {
            Texture2D resizedTexture = new Texture2D(newWidth, newHeight);
            for (int i = 0; i < newWidth; i++)
            {
                for (int j = 0; j < newHeight; j++)
                {
                    Color newColor = texture.GetPixelBilinear((float)i / newWidth, (float)j / newHeight);
                    resizedTexture.SetPixel(i, j, newColor);
                }
            }

            resizedTexture.Apply(); // 변경사항 적용

            // 원본 텍스처를 새로 조절된 텍스처로 교체
            return resizedTexture;
        }

        public static byte[] ByteTextureToResizeByte(byte[] bytes)
        {
            Texture2D texture2D = ByteArrayToTexture(bytes);
            if (texture2D.width > 4000 || texture2D.height > 4000)
            {
                texture2D = ResizeTexture(texture2D, texture2D.width / 5, texture2D.height / 5);
            }
            else if (texture2D.width > 3000 || texture2D.height > 3000)
            {
                texture2D = ResizeTexture(texture2D, texture2D.width / 4, texture2D.height / 4);
            }
            else if (texture2D.width > 2000 || texture2D.height > 2000)
            {
                texture2D = ResizeTexture(texture2D, texture2D.width / 3, texture2D.height / 3);
            }
            else if (texture2D.width > 1000 || texture2D.height > 1000)
            {
                texture2D = ResizeTexture(texture2D, texture2D.width / 2, texture2D.height / 2);
            }

            byte[] result = texture2D.EncodeToJPG();
            return result;
        }
    }
}