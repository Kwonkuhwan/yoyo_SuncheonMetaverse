using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Suncheon.Player;

namespace Suncheon.UI
{
    public class ClubMemberObject : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Image image_Bg;
        [SerializeField] private TMP_Text text_NickName;

        public void SetObject(bool isHost, string nickName)
        {
            if (isHost)
            {
                image_Bg.sprite = sprites[0];
            }
            else
            {
                image_Bg.sprite = sprites[1];
            }

            text_NickName.text = nickName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().joinedClubData.nickname == PhotonNetwork.NickName)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {

                }
                else
                {
                    if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        string hitPlayerName = text_NickName.text.Split(':')[0].Trim();

                        if (hitPlayerName == PhotonNetwork.NickName) return;

                        Vector3 canvasPos = eventData.position;
                        NetworkManager.Instance.user_name = text_NickName.text;
                        Load_UserID(hitPlayerName);

                        UIInteractionManager_Inside uiinter = (UIInteractionManager_Inside)UIInteractionManager.Instance;
                        uiinter.ClubPlayerInteractionOn(hitPlayerName, canvasPos);
                    }
                }
            }
        }

        public virtual void Load_UserID(string playerName)
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userIdCheck}", $"nickname={playerName}",
               (jsonData) =>
               {
                   Set_UserID(jsonData);
               }
               ));
        }

        public virtual void Set_UserID(string userID)
        {
            NetworkManager.Instance.user_id = userID;
        }        
    }
}