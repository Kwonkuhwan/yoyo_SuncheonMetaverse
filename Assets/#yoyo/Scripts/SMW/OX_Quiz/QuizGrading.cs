using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suncheon
{
    public class QuizGrading : MonoBehaviour
    {
        [SerializeField]
        GameObject[] Items;

        List<GameObject> correct;
        List<GameObject> wrong;

        int index = 0;

        private void Start()
        {
            QuizManager.instance.GradingEvent += Grading;
            QuizManager.instance.ResetQuizEvent += Reset;

            Bind();
            Reset();
        }

        private void OnDestroy()
        {
            QuizManager.instance.GradingEvent -= Grading;
            QuizManager.instance.ResetQuizEvent -= Reset;
        }

        private void Reset()
        {
            index = 0;

            for (int i = 0; i < Items.Length; i++)
            {
                correct[i].SetActive(false);
                wrong[i].SetActive(false);
            }
        }

        private void Bind()
        {
            correct = new List<GameObject>();
            wrong = new List<GameObject>();

            for (int i = 0; i < Items.Length; i++)
            {
                correct.Add(Items[i].transform.GetChild(0).gameObject);
                wrong.Add(Items[i].transform.GetChild(1).gameObject);
            }
        }

        void Grading(bool answer)
        {
            if (index > 9) return;

            if (answer)
            {
                correct[index].SetActive(true);
            }
            else
            {
                wrong[index].SetActive(true);
            }

            index++;
        }
    }
}