using Suncheon.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Suncheon
{
    public class NPCQuestionBtn : MonoBehaviour
    {
        [SerializeField] private UI_NPCChat ui_NPCChat;
        [SerializeField] private TMP_Text text_Answer;
        [SerializeField] private string question;
        [SerializeField] private string answer;

        private void Awake()
        {
            ui_NPCChat = FindFirstObjectByType<UI_NPCChat>();

            GetComponent<Button>().onClick.AddListener(() =>
            {
                ui_NPCChat.ui_NpcAnswer.GetComponent<UI_NPCAnswer>().Init(question, answer);
                ui_NPCChat.btn.SetActive(false);
                UIInteractionManager.Instance.OpenPopUp(ui_NPCChat.ui_NpcAnswer.gameObject);                
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

        public void Init(string _question, string _answer)
        {
            text_Answer.text = _question;
            question = _question;
            answer = _answer;
        }
    }
}
