using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public delegate void YesBtnDelegate();
    public delegate void NoBtnDelegate();

    public delegate void ClubViteYesBtnDelegate(int clubSeq);
    public delegate void ClubViteNoBtnDelegate();

    public delegate void LibVisitYesBtnDelegate();
    public delegate void LibVisiteNoBtnDelegate();

    public class UI_YesNoPopUp : MonoBehaviour
    {
        [SerializeField] private Button btn_Yes;
        [SerializeField] private Button btn_No;
        [SerializeField] private TMP_Text text_Info;

        public void SetYesBtn(YesBtnDelegate yesBtnDelegate)
        {
            btn_Yes.onClick.RemoveAllListeners();
            btn_Yes.onClick.AddListener(() => yesBtnDelegate());
        }

        public void SetYesBtn(ClubViteYesBtnDelegate yesBtnDelegate, int clubSeq)
        {
            btn_Yes.onClick.RemoveAllListeners();
            btn_Yes.onClick.AddListener(() => yesBtnDelegate(clubSeq));
        }

        public void SetNoBtn(NoBtnDelegate noBtnDelegate)
        {
            btn_No.onClick.RemoveAllListeners();
            btn_No.onClick.AddListener(() => noBtnDelegate());
        }

        public void SetNoBtn(ClubViteNoBtnDelegate noBtnDelegate)
        {
            btn_No.onClick.RemoveAllListeners();
            btn_No.onClick.AddListener(() => noBtnDelegate());
        }

        public void ShowPopUp(string info)
        {
            text_Info.text = info;
            if (UIInteractionManager.Instance)
            {
                UIInteractionManager.Instance.OpenPopUp(gameObject);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void ShowPopUp()
        {
            if (UIInteractionManager.Instance)
            {
                UIInteractionManager.Instance.OpenPopUp(gameObject);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void HidePopUp()
        {
            text_Info.text = string.Empty;
            if (UIInteractionManager.Instance)
            {
                UIInteractionManager.Instance.ClosePopUp(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
