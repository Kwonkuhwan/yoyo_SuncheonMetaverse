using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : MonoBehaviour
{
    [SerializeField] private GameObject ui_Sound;
    [SerializeField] private GameObject ui_Channel;
    [SerializeField] private GameObject ui_Info;
    [SerializeField] private GameObject ui_Tutorial;

    [SerializeField] private Button btn_Channel;
    [SerializeField] private Button btn_Info;
    [SerializeField] private Button btn_Tutorial;

    private void Awake()
    {
        btn_Channel.onClick.AddListener(() => ChannelUIOn());
        btn_Info.onClick.AddListener(() => InfoUIOn());
        btn_Tutorial.onClick.AddListener(() => TutorialOn());
    }

    private void OnEnable()
    {
        ui_Sound.SetActive(true);
        ui_Channel.SetActive(false);
        ui_Info.SetActive(false);
    }

    private void ChannelUIOn()
    {
        ui_Sound.SetActive(false);
        ui_Channel.SetActive(true);
        ui_Info.SetActive(false);
    }

    private void InfoUIOn()
    {
        ui_Sound.SetActive(false);
        ui_Channel.SetActive(false);
        ui_Info.SetActive(true);
    }

    private void TutorialOn()
    {
        UIInteractionManager.Instance.OpenPopUp(ui_Tutorial);
        UIInteractionManager.Instance.ClosePopUp(gameObject);
    }
}
