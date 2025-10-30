using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suncheon
{
    public class AnswerTrigger : MonoBehaviour
    {
        [SerializeField]
        bool answer;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.TryGetComponent(out PhotonView pv))
            {
                if (pv.IsMine)
                {
                    QuizManager.instance.ChooseUserAnswer(answer);
                }
            }
        }
    }
}