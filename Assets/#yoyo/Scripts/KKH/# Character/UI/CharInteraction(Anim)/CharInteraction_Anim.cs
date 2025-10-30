using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Player
{
    public class CharInteraction_Anim : MonoBehaviour
    {
        [SerializeField] private Button btn_Dacne;
        [SerializeField] private Button btn_Hello;
        [SerializeField] private Button btn_Smlie;
        [SerializeField] private Button btn_Sad;

        private void Start()
        {
            btn_Dacne.onClick.AddListener(() => BtnDacneClick());
            btn_Hello.onClick.AddListener(() => BtnHelloClick());
            btn_Smlie.onClick.AddListener(() => BtnSmlieClick());
            btn_Sad.onClick.AddListener(() => BtnSadClick());
        }

        private void BtnDacneClick()
        {
            PlayerAnimManager playerAnimManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerAnimManager>();
            playerAnimManager.SetDance();
        }

        private void BtnHelloClick()
        {
            PlayerAnimManager playerAnimManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerAnimManager>();
            playerAnimManager.SetHello();
        }

        private void BtnSmlieClick()
        {
            PlayerAnimManager playerAnimManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerAnimManager>();
            playerAnimManager.SetSmlie();
        }

        private void BtnSadClick()
        {
            PlayerAnimManager playerAnimManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerAnimManager>();
            playerAnimManager.SetSad();
        }
    }
}