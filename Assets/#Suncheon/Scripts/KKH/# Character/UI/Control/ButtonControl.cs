using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class ButtonControl : MonoBehaviour, IPointerExitHandler, IPointerMoveHandler, IPointerEnterHandler, IPointerClickHandler
    {
        [Header("이미지 관련")]
        [SerializeField] protected bool isImage;
        [SerializeField] protected Image image;
        [SerializeField] protected Sprite sprite_On;
        [SerializeField] protected Sprite sprite_Off;

        [SerializeField] protected bool isImageSetNativeSize;
        [SerializeField] protected bool isImageMouse;
        [SerializeField] protected float outSize = 1.0f;
        [SerializeField] protected float inSize = 1.0f;

        public List<Image> image_BtnList = new List<Image>();
        [SerializeField] protected bool isImageFadeOut_Color;
        [SerializeField] protected Color color_BtnImageFadeOut;
        [SerializeField] protected Color color_BtnImageNormal = Color.white;
                
        [Space(10)]

        [Header("텍스트 관련")]
        [SerializeField] protected bool isTextInfo;
        [SerializeField] public TMP_Text text;
        [SerializeField] protected Color text_NormalColor;
        [SerializeField] protected Color text_HighlightedColor;
        [SerializeField] protected TMP_FontAsset text_NormalFont;
        [SerializeField] protected TMP_FontAsset text_HighlightedFont;
        [SerializeField] protected bool isClickText_Color;
        [SerializeField] protected Color color_Click = Color.white;
        [SerializeField] protected Color color_NonClick = Color.black;

        [Space(10)]

        [Header("백그라운드 관련")]
        [SerializeField] protected bool isBackFroundFadeOut;
        [SerializeField] protected Image image_BackGround;
        [SerializeField] protected Color color_BackGroundNormal;
        [SerializeField] protected Color color_BackGroundFadeOut;

        [Space(10)]
        [Header("사운드")]
        [SerializeField] protected bool isAaudio;
        [SerializeField] protected AudioSource audioSource;

        virtual protected void Awake()
        {
            if (isImage)
            {
                if (image == null) image = GetComponent<Image>();  // 이미지 컴포넌트를 가져옵니다.
            }

            if (isTextInfo || isClickText_Color)
            {
                if (text == null) text = GetComponentInChildren<TMP_Text>();  // TMP_Text 컴포넌트를 자식 오브젝트에서 가져옵니다.
            }

            if (isImageMouse)
            {
                transform.localScale = new Vector3(outSize, outSize, outSize);  // 이미지의 크기를 설정합니다.
            }

            if (isAaudio)
            {
                audioSource = GetComponent<AudioSource>();  // AudioSource 컴포넌트를 가져옵니다.
                GetComponent<Button>().onClick.AddListener(() =>
                {
                    audioSource.volume = GameManager.Instance.EffectVolume * GameManager.Instance.AllVolume;  // 버튼이 클릭되었을 때, AudioSource의 볼륨을 설정합니다.
                    audioSource.Play();  // AudioSource를 재생합니다.
                });
            }
        }

        private void OnDisable()
        {   
            if (image != null && text != null)
            {
                Btnoff();
            }
        }

        virtual public void OnChangeSprite(bool isOn)
        {
            if (isOn)
            {
                image.sprite = sprite_On;  // 이미지의 스프라이트를 sprite_On으로 변경합니다.
            }
            else
            {
                image.sprite = sprite_Off;  // 이미지의 스프라이트를 sprite_Off로 변경합니다.
            }

            if (isImageSetNativeSize)
            {
                image.SetNativeSize();  // 이미지의 네이티브 사이즈로 설정합니다.
            }
        }

        virtual public void OnPointerMove(PointerEventData eventData)
        {
            if (isTextInfo)
            {
                TextColorChange(true);  // 텍스트 색상을 변경합니다.
            }

            if (isImageMouse)
            {
                gameObject.transform.localScale = new Vector3(inSize, inSize, inSize);  // 이미지의 크기를 설정합니다.
                image.sprite = sprite_On;  // 이미지의 스프라이트를 sprite_On으로 변경합니다.
            }
        }

        virtual public void TextColorChange(bool isHilight)
        {
            if (text == null)
            {
                return;
            }

            if (!isHilight)
            {
                text.font = text_NormalFont;  // 텍스트의 폰트를 text_NormalFont로 변경합니다.
                text.color = text_NormalColor;  // 텍스트의 색상을 text_NormalColor로 변경합니다.
            }
            else
            {
                text.font = text_HighlightedFont;  // 텍스트의 폰트를 text_HighlightedFont로 변경합니다.
                text.color = text_HighlightedColor;  // 텍스트의 색상을 text_HighlightedColor로 변경합니다.
            }
        }

        virtual public void OnBackGroundFadeIn()
        {
            if (isBackFroundFadeOut)
            {
                image_BackGround.color = color_BackGroundNormal;  // 배경 이미지의 색상을 color_BackGroundNormal로 변경합니다.
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GameManager.Instance.isUIMouseOver = true;  // GameManager의 isUIMouseOver 변수를 true로 설정합니다.
        }

        virtual public void OnPointerExit(PointerEventData eventData)
        {
            if (isTextInfo)
            {
                TextColorChange(false);  // 텍스트 색상을 변경합니다.
            }

            if (isImageMouse)
            {
                gameObject.transform.localScale = new Vector3(outSize, outSize, outSize);  // 이미지의 크기를 설정합니다.
                image.sprite = sprite_Off;  // 이미지의 스프라이트를 sprite_Off로 변경합니다.
            }

            GameManager.Instance.isUIMouseOver = false;  // GameManager의 isUIMouseOver 변수를 false로 설정합니다.
        }

        virtual public void OnBtnImageFadeOut()
        {
            if (isImageFadeOut_Color)
            {
                image.color = color_BtnImageNormal;  // 이미지의 색상을 color_BtnImageNormal로 변경합니다.
                foreach (Image img in image_BtnList)
                {
                    img.color = color_BtnImageFadeOut;  // 이미지 리스트의 색상을 color_BtnImageFadeOut으로 변경합니다.
                }
            }
        }

        virtual public void OnBtnImageFadeIn()
        {
            if (isImageFadeOut_Color)
            {
                foreach (Image img in image_BtnList)
                {
                    img.color = color_BtnImageNormal;  // 이미지 리스트의 색상을 color_BtnImageNormal로 변경합니다.
                }
            }
        }

        virtual public void OnBackGroundFadeOut()
        {
            if (isImageFadeOut_Color)
            {
                if (isBackFroundFadeOut)
                {
                    image_BackGround.color = color_BackGroundFadeOut;  // 배경 이미지의 색상을 color_BackGroundFadeOut으로 변경합니다.
                }
            }
        }

        public void Btnoff()
        {
            image.sprite = sprite_Off;  // 이미지의 스프라이트를 sprite_Off로 변경합니다.
            TextColorChange(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isClickText_Color)
            {
                text.color = color_Click;
                foreach (Image img in image_BtnList)
                {
                    img.GetComponent<ButtonControl>().text.color = color_NonClick;
                }
            }
        }
    }
}