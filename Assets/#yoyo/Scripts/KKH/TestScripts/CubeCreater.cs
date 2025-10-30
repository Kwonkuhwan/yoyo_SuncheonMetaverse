using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CubeCreater : MonoBehaviour
{
    [SerializeField] private GameObject go_Cube;
    [SerializeField] private TMP_Text text_CubeCnt;

    private int cubeCnt = 0;

    void Update()
    {
        Instantiate(go_Cube, transform.position, Quaternion.identity);
        cubeCnt++;
        text_CubeCnt.text = cubeCnt.ToString();
    }
}
