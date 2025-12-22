using Photon.Pun;
using Suncheon.Player;
using UnityEngine;

namespace Suncheon
{
    public class CustomMaizingManager : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private PlayerManager playerManager;


        private void Awake()
        {
            networkManager = NetworkManager.Instance;
            playerManager = networkManager.Go_Player.GetComponent<PlayerManager>(); ;
        }

        public void MetaverseStart()
        {
            networkManager.Go_Player.GetComponent<PhotonView>().RPC("RPCSetAvatarInfo", RpcTarget.AllBuffered, playerManager.AvatarInfo);
            playerManager.UIInteraction.Btn_CharDecorationClick();
        }
    }
}