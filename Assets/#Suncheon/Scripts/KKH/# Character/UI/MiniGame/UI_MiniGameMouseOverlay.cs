using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MiniGameMouseOverlay : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.Stop();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        anim.Play();
    }
}
