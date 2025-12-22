using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class VoteObject : MonoBehaviour
    {
        [SerializeField] private int voteNum;
        [SerializeField] private Image image = null;
        [SerializeField] private TMP_Text text_Name = null;

        public void Init(int _voteNum, string categoryName, string categoryimgName)
        {
            string fileUrl = "";
            if (string.IsNullOrEmpty(GameManager.Instance.defaultData.fileUrl.Trim()))
            {
                fileUrl = $"https://metalibrary.suncheon.go.kr/upload/";
            }
            else
            {
                fileUrl = GameManager.Instance.defaultData.fileUrl;
            }

            StartCoroutine(UTILS.Requset_HttpGetTexture($"{fileUrl}{categoryimgName}", (textyreData) =>
            {
                UTILS.Log($"SetVoteObject : {textyreData}");
                voteNum = _voteNum;
                text_Name.text = categoryName;
                SetImage(textyreData);
            }));
        }

        private void SetImage(Texture2D texture)
        {
            image.sprite = UTILS.ConvertToSprite(texture);
        }
    }
}