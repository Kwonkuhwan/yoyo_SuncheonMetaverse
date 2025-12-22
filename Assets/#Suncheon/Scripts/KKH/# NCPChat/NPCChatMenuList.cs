using Suncheon;
using Suncheon.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCChatMenuList : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject menuBtn;



    public void Init(List<string> data)
    {
        List<string> categroys = data.Distinct().ToList();
        if (categroys == null) return;

        ScrollClear();

        foreach(string categroy in categroys)
        {
            GameObject obj = Instantiate(menuBtn, content);
            obj.GetComponent<NPCChatMenuBtn>().Init(categroy);
        }


    }

    private void ScrollClear()
    {
        foreach (var obj in content.GetComponentsInChildren<ButtonControl>())
        {
            Destroy(obj.gameObject);
        }
    }
}
