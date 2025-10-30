using Suncheon.Admin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuseumBtn : MonoBehaviour
{
    [SerializeField] private GameObject image_SelectPopUp;

    public int nMuseumFrameCnt = 0;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => MuseumBtnClick());
    }

    public void MuseumBtnClick()
    {
        image_SelectPopUp.SetActive(true);
        image_SelectPopUp.GetComponent<Image_SelectPopUp>().Init(nMuseumFrameCnt);
    }
}
