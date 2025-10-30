using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class ProgressBar : MonoBehaviour
    {

        public bool b = true;
        public Image image;
        public float speed;
        float time = 0f;
        float maxValue = 1.0f;

        float height = 390.0f;

        public TMP_Text text_Cnt;
        public TMP_Text text_Time;

        public void Start()
        {
            image = GetComponent<Image>();
        }

        void Update()
        {
            if (maxValue > image.fillAmount)
            {
                if (b)
                {
                    time += Time.deltaTime * speed;
                    image.fillAmount = time;

                    float text_height = -195.0f + (height * image.fillAmount);

                    text_Cnt.transform.localPosition = new Vector3(text_Cnt.transform.localPosition.x, text_height, text_Cnt.transform.localPosition.y);

                    if (time > 1)
                    {

                        time = 0;
                    }
                }
            }
        }

        public void SetProgressBar(float maxCnt, float _maxValue, string date)
        {
            maxValue = _maxValue;
            text_Cnt.text = maxCnt.ToString();
            text_Time.text = date;
        }

    }
}