using Photon.Pun;
using Suncheon.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class PlayerUIInteraction : MonoBehaviour
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PhotonView pv;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            pv = playerManager.Pv;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (pv.IsMine)
            {
                if (other.CompareTag("Inside_Trigger"))
                {
                    UIInteractionManager.Instance.ui_LibPopUp.GetComponent<UI_LibPopUp>().ShowPopUp(other.gameObject.GetComponent<InSideTrigger>().LibaryName);
                    GetComponent<PlayerAnimManager>().SetIdle();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (pv.IsMine)
            {
                if (other.CompareTag("Inside_Trigger"))
                {
                    if (UIInteractionManager.Instance.ui_LibPopUp.activeInHierarchy)
                    {
                        UIInteractionManager.Instance.ui_LibPopUp.GetComponent<UI_LibPopUp>().HidePopUp();
                    }
                }
            }
        }
    }
}