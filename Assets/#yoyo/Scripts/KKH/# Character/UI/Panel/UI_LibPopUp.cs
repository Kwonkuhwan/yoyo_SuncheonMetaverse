using TMPro;
using UnityEngine;

namespace Suncheon.UI
{
    public class UI_LibPopUp : MonoBehaviour
    {
        [SerializeField] private TMP_Text text_LibName;
        [SerializeField] private GameObject[] panals_Lib;

        public void ShowPopUp(LibName libName)
        {
            text_LibName.text = libName.ToString();

            foreach (var panel in panals_Lib)
            {
                panel.SetActive(false);
            }

            UIInteractionManager.Instance.OpenPopUp(gameObject);

            panals_Lib[(int)libName].SetActive(true);
        }

        public void HidePopUp()
        {
            foreach (var panel in panals_Lib)
            {
                panel.SetActive(false);
            }
            UIInteractionManager.Instance.ClosePopUp(gameObject);
        }
    }
}