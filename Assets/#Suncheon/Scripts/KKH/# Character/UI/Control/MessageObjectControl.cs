using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Suncheon.Player;
using TMPro;

namespace Suncheon.UI
{
    public class MessageObjectControl : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ChatMode chatMode;

        public void Init(ChatMode _chatMode)
        {
            chatMode = _chatMode;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {

            }
            else
            {
                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    string[] strings = new string[0];
                    if (gameObject.GetComponent<TMP_Text>().text.Contains(">>"))
                    {
                        strings = gameObject.GetComponent<TMP_Text>().text.Split(">>");
                    }
                    else if (gameObject.GetComponent<TMP_Text>().text.Contains("<<"))
                    {
                        strings = gameObject.GetComponent<TMP_Text>().text.Split("<<");
                    }

                    if (strings.Length < 0) return;
                    string hitPlayerName = strings[0].Trim();

                    //if(hitPlayerName == PlayerManager.Instance.PlayerName) return;
                    Vector3 canvasPos = eventData.position;

                    UIInteractionManager.Instance.PlayerInteractionOn(hitPlayerName, canvasPos);
                }
            }
        }
    }
}
