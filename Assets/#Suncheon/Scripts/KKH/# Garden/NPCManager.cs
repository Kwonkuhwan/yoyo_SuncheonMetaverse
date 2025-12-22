using Suncheon.UI;
using Suncheon.Player;
using UnityEngine;

namespace Suncheon
{
    public class NPCManager : MonoBehaviour, Clickable
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject ui_TalkBox;
        [SerializeField] private Animator anim;
        [SerializeField] string obj_Name = null;
        private static readonly int _Idle = Animator.StringToHash("Idle");

        [SerializeField] private GameObject ui_NpcChat;

        [Range(0.0f, 300.0f)]
        [SerializeField] private float maxDistance;             

        private void Start()
        {
            anim.SetTrigger(_Idle);
        }

        private void Update()
        {
            if (NetworkManager.Instance == null) return;

            if (player == null)
            {
                player = NetworkManager.Instance.Go_Player;
            }

            if (player != null)
            {
                float distance = Vector3.Distance(player.transform.position, transform.position);
                if (distance > maxDistance)
                {
                    ui_TalkBox.SetActive(false);
                }
                else
                {
                    ui_TalkBox.SetActive(true);
                    Vector3 direction = player.transform.position - transform.position;
                    //direction.x = 0;
                    direction.y = 0;
                    //direction.z = 0;
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = rotation;
                    //transform.LookAt(player.GetComponent<PlayerManager>().tr_LoockAtPos);
                    //transform.eulerAngles += new Vector3(0.0f, 180.0f, 0.0f);
                }                
            }
        }

        public void OnClick()
        {
            UIInteractionManager.Instance.OpenPopUp(ui_NpcChat);
        }

        public string Return_ObjName()
        {
            return obj_Name;
        }
    }
}
    