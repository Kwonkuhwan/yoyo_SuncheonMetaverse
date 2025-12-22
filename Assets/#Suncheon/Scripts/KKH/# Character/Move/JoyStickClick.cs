using Suncheon;
using Suncheon.Player;
using UnityEngine.EventSystems;
using UnityEngine;

public class JoyStickClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private void Awake()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        gameObject.SetActive(true);
#elif UNITY_EDITOR || UNITY_WEBGL
        gameObject.SetActive(false);
#endif
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        NetworkManager.Instance.Go_Player.GetComponent<PlayerMoveManager>().IsJoyStick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        NetworkManager.Instance.Go_Player.GetComponent<PlayerMoveManager>().IsJoyStick = false;
    }
}
