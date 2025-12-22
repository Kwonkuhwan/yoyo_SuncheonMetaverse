using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

public class AdminButtonControl : MonoBehaviour
{
    [SerializeField] private ButtonControl buttonControl;

    private void Awake()
    {
        buttonControl = GetComponent<ButtonControl>();
        foreach(Image image in transform.parent.GetComponentsInChildren<Image>())
        {
            if (image.gameObject == gameObject) continue;
            buttonControl.image_BtnList.Add(image);
        }
    }
}
