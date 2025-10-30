using Suncheon;
using UnityEngine;
using UnityEngine.UI;

public class VoteSelectBtn : MonoBehaviour
{
    Toggle toggle;
    [SerializeField] private VoteSelectBtn[] otherSelects;
    public bool isSelect = false;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { Toggle_On(); } );
    }

    private void Toggle_On()
    {
        isSelect = toggle.isOn;
        UTILS.Log($"toggle.isOn = {isSelect}");
        if (isSelect)
        {
            foreach (VoteSelectBtn item in otherSelects)
            {
                item.Toggle_Off();
            }
        }
    }

    public void Toggle_Off()
    {
        toggle.isOn = false;
        isSelect = toggle.isOn;
    }
}
