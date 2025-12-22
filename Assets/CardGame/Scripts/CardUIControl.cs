using Suncheon.UI;
using UnityEngine;

public class CardUIControl : MonoBehaviour
{
    [SerializeField] GameObject UI;
    UIInteractionManager interactionManager;
    private void Awake()
    {
        interactionManager = FindAnyObjectByType<UIInteractionManager>();
    }
    private void OnDisable()
    {
        interactionManager.Play_Game = false;
        UI.SetActive(true);
    }

    private void OnEnable()
    {
        interactionManager.Play_Game = true;
        UI.SetActive(false);
    }
}
