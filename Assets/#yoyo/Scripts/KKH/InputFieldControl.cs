using TMPro;
using UnityEngine;

namespace Suncheon.UI
{
    public class InputFieldControl : MonoBehaviour
    {
        private TMP_InputField inputField;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();
        }

        private void Update()
        {
            // [24.02.16] [작성] KKH : 인풋 필드를 선택하고 있을때
            if (inputField.isFocused)
            {
                // [24.02.16] [작성] KKH : 복사 Ctrl + C
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
                {
                    UTILS.CopyToClipBoard(inputField.text);
                }

                // [24.02.16] [작성] KKH : 붙여넣기 Ctrl + V
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
                {
                    UTILS.PasteToClipBoard(inputField);
                }
            }
        }
    }
}