using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Screen_Check : MonoBehaviour
{
    [SerializeField] TMP_Text width;
    [SerializeField] TMP_Text height;
    void Update()
    {
        width.text = $"width {Screen.width}";
        height.text = $"height {Screen.height}";
    }
}
