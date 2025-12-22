using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suncheon.UI;

namespace Suncheon {
    public class NPCChatQuestionList : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject questionBtn;

        public void Init(List<string> questions, List<string> answers)
        {
            ScrollClear();

            for(int i=0; i<questions.Count; i++)
            {
                GameObject obj = Instantiate(questionBtn, content);
                obj.GetComponent<NPCQuestionBtn>().Init(questions[i], answers[i]);
            }
        }

        private void ScrollClear()
        {
            foreach (var obj in content.GetComponentsInChildren<ButtonControl>())
            {
                Destroy(obj.gameObject);
            }
        }
    }
}
