using Suncheon.UI;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Suncheon
{
    public class UI_NPCChat : MonoBehaviour
    {
        List<Dictionary<string, object>> data;
        List<string> types = new List<string>();
        List<string> categorys = new List<string>();
        List<string> questions = new List<string>();
        List<string> answers = new List<string>();

        [SerializeField] private NPCChatMenuList npcMenuList;
        [SerializeField] private NPCChatQuestionList npcQuestionList;

        public GameObject btn; //닫기버튼
        public GameObject ui_NpcAnswer;

        private void Start()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrNpcFAQLoad}", (jsonData) =>
            {
                Response_NPCLoad response_NPCLoad = JsonUtility.FromJson<Response_NPCLoad>(jsonData);
                if (response_NPCLoad == null) return;
                StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.fileUrl}{response_NPCLoad.faq_file}", (jsonData) =>
                {
                    SetData(jsonData);

                    npcMenuList.Init(categorys);
                }));
            }));
        }

        void SetData(string jsonData)
        {
            data = new List<Dictionary<string, object>>();
            data = CSVReader.Read(jsonData);

            types = new List<string>();
            categorys = new List<string>();
            questions = new List<string>();
            answers = new List<string>();

            for (int i = 0; i < data.Count; i++)
            {
                object value;
                if (data[i].TryGetValue("Type", out value))
                {
                    types.Add(value.ToString());
                }
                // 키가 존재하지 않는 경우 처리
                else continue;

                if (data[i].TryGetValue("Category", out value))
                {
                    categorys.Add(value.ToString());
                }
                // 키가 존재하지 않는 경우 처리
                else continue;

                if (data[i].TryGetValue("Question", out value))
                {
                    questions.Add(value.ToString());
                }
                // 키가 존재하지 않는 경우 처리
                else continue;

                if (data[i].TryGetValue("Answer", out value))
                {
                    answers.Add(value.ToString());
                }
                // 키가 존재하지 않는 경우 처리
                else continue;
            }
        }

        public void SetNCPChatList(string category)
        {
            List<string> _Questions = new List<string>();
            List<string> _Answers = new List<string>();

            for (int i = 0; i < categorys.Count; i++)
            {
                if (categorys[i] != category) continue;

                _Questions.Add(questions[i]);
                _Answers.Add(answers[i]);
            }

            npcQuestionList.Init(_Questions, _Answers);
        }
    }
}
