using TMPro;
using UnityEngine;
using Suncheon.Player;
using UnityEngine.EventSystems;

namespace Suncheon.UI
{
    public class ChatBoxControl : MonoBehaviour, IUIControl
    {
        [SerializeField] private TMP_InputField input_ChatBox;
        [SerializeField] private int nTextMaxCount;
        
        private void Awake()
        {
            input_ChatBox = GetComponent<TMP_InputField>();

            input_ChatBox.onSelect.AddListener((str) => OnSelect());
            input_ChatBox.onDeselect.AddListener((str) => DeSelect());            
            input_ChatBox.onValueChanged.AddListener((str) => OnValueChange());
        }

        public void DeSelect()
        {
            UIInteractionManager.Instance.IsUIInteraction = false;
        }

        public void OnSelect()
        {
            UIInteractionManager.Instance.IsUIInteraction = true;

            input_ChatBox.selectionAnchorPosition = 0;  // 커서의 처음 위치
            input_ChatBox.selectionFocusPosition = 0;   // 커서의 마지막 위치

            input_ChatBox.selectionAnchorPosition = input_ChatBox.text.Length; // 커서의 처음 위치
            input_ChatBox.selectionFocusPosition = input_ChatBox.text.Length;   // 커서의 마지막 위치
        }

        public void OnValueChange()
        {
            if(input_ChatBox.text.Length > nTextMaxCount)
            {
                input_ChatBox.text = input_ChatBox.text.Substring(nTextMaxCount);
            }
        }
    }
}