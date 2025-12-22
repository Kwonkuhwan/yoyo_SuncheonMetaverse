using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class ImageSamplePopUp : MonoBehaviour
    {
        public Image image_Sample;

        public void SetImage(Sprite sprite)
        {
            image_Sample.sprite = sprite;
        }
    }
}
