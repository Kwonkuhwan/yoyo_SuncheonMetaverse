using UnityEngine;

namespace Suncheon.UI
{
    public class UI_Setting_Channel : MonoBehaviour
    {
        [SerializeField] private Transform content_Channel;
        [SerializeField] private GameObject go_Channel;

        private void OnEnable()
        {
            foreach (RoomData child in content_Channel.GetComponentsInChildren<RoomData>())
            {
                Destroy(child.gameObject);
            }

            foreach (var ch in NetworkManager.Instance.ChannelPlayerCnts)
            {
                GameObject channel = Instantiate(go_Channel, content_Channel);
                channel.GetComponent<RoomData>().SetRoomData(ch.Key, ch.Value);
            }
        }
    }
}