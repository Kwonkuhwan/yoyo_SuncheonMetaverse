using Newtonsoft.Json.Linq;
using Suncheon;
using Suncheon.WebData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_EventList : MonoBehaviour
{
    [SerializeField] private GameObject eventObject;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject ui_UrlSavePopUp;

    private void OnEnable()
    {
        foreach (var go in content.GetComponentsInChildren<EventObject>())
        {
            Destroy(go.gameObject);
        }

        StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrEventList}", (jsonData) =>
        {
            Response_EventList response_EventList = null;
            try
            {
                response_EventList = new Response_EventList(jsonData);
            }
            catch
            {
                response_EventList = null;
            }

            if (response_EventList == null) return;

            SetEventObject(response_EventList);
        }));
    }

    private void SetEventObject(Response_EventList result)
    {
        foreach(var item in result.response_EventListDatas)
        {
            GameObject obj = Instantiate(eventObject, content);
            UTILS.Log($"{item.eventName}, {item.eventStartTime}, {item.eventLocation}, {item.eventSeq}");
            obj.GetComponent<EventObject>().Init(item, ui_UrlSavePopUp);
        }

    }
}
