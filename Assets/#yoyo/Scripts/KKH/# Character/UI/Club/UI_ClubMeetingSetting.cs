using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Suncheon.Player;
using Photon.Pun;

namespace Suncheon.UI
{
    public class UI_ClubMeetingSetting : MonoBehaviour
    {
        [SerializeField] private TMP_InputField input_ClubMettingName;
        [SerializeField] private TMP_InputField input_FileURL;
        [SerializeField] private Button btn_FileUpLoad;

        private void Awake()
        {
            if(btn_FileUpLoad != null) btn_FileUpLoad.onClick.AddListener(()=>BtnFileUpLoadClick());
        }

        public void ShowPopUp()
        {
            UIInteractionManager.Instance.OpenPopUp(gameObject);
        }

        public void BtnFileUpLoadClick()
        {
            UTILS.Log("BtnFileUpLoadClick");

            NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().club_MeetingName = input_ClubMettingName.text;
            NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().club_FileURL = input_FileURL.text;

            UIInteractionManager.Instance.ClosePopUp(gameObject);
        }
    }
}
