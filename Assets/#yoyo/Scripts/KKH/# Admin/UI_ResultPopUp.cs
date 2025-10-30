using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Suncheon.Admin
{
    public class UI_ResultPopUp : MonoBehaviour
    {
        [SerializeField] GameObject Notice_ResultPopUp;
        [SerializeField] TMP_Text text_ResultInfo;

        public void ShowPopUp(string info)
        {
            if (text_ResultInfo == null) return;

            text_ResultInfo.text = info;
            StartCoroutine(StartPopUp());
        }

        private IEnumerator StartPopUp()
        {
            Notice_ResultPopUp.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            Notice_ResultPopUp.SetActive(false);
        }
    }
}
