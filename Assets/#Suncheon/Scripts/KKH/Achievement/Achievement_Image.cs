using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon
{

    public class Achievement_Image : MonoBehaviour
    {
        [Space(10)]
        public TMP_Text text_Info;
        [SerializeField] private Color color_TextComp;

        [Space(10)]
        [SerializeField] private AchievementMenu achievementMenu;
        public AchievementMenu Type => achievementMenu;

        [Space(10)]
        [SerializeField] private Image Image_BG;
        [SerializeField] private Color color_ImageBG;

        [Space(10)]
        [SerializeField] private Image Image_comp;
        [SerializeField] private Sprite sprite_On;
        [SerializeField] private Sprite sprite_Off;

        [Space(10)]
        public bool comp;

        public void Init(string info, AchievementMenu menu, bool _comp)
        {
            text_Info.text = $"  {info}  ";
            achievementMenu = menu;
            comp = _comp;
            if (comp)
            {
                Image_BG.color = color_ImageBG;

                Image_comp.sprite = sprite_On;

                text_Info.fontStyle = TMPro.FontStyles.Strikethrough;
                text_Info.color = color_TextComp;
            }
            else
            {
                Image_comp.sprite = sprite_Off;
            }
        }
    }
}
