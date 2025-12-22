using Suncheon.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon
{
    public class NPCChatMenuBtn : MonoBehaviour
    {
        [SerializeField] private UI_NPCChat ui_NPCChat;
        [SerializeField] private TMP_Text text_Info;
        public string type;

        private void Awake()
        {
            ui_NPCChat = FindFirstObjectByType<UI_NPCChat>();

            GetComponent<Button>().onClick.AddListener(() =>
            {
                ui_NPCChat.SetNCPChatList(type);
            });
        }

        private void Start()
        {
            ButtonControl buttonControl = GetComponent<ButtonControl>();
            buttonControl.image_BtnList.Clear();
            foreach (NPCChatMenuBtn obj in transform.parent.GetComponentsInChildren<NPCChatMenuBtn>())
            {
                if (obj.gameObject == this) continue;
                buttonControl.image_BtnList.Add(obj.GetComponent<Image>());
            }
        }

        public void Init(string _type)
        {
            type = _type;
            text_Info.text = type;
        }
    }
}
