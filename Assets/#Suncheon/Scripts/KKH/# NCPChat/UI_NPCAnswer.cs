using TMPro;
using UnityEngine;

namespace Suncheon
{
    public class UI_NPCAnswer : MonoBehaviour
    {
        [SerializeField] private TMP_Text text_Question;
        [SerializeField] private TMP_Text text_Answer;

        UI_NPCChat ui_NPCChat;

        private void OnDisable()
        {
            ui_NPCChat.btn.SetActive(true);
        }

        private void Awake()
        {
            ui_NPCChat = GetComponentInParent<UI_NPCChat>();
        }

        public void Init(string question, string answer)
        {
            text_Question.text = question;
            text_Answer.text = answer;
        }
    }
}
