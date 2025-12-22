using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class RenderingManager : MonoBehaviour
{
    [SerializeField] GameObject[] renderObjs;
    [Range(0.0f, 1.0f)]
    [SerializeField] float renderTime = 0.5f;

    private void Awake()
    {
        StartCoroutine(RenderObject());
    }

    IEnumerator RenderObject()
    {
        foreach (var obj in renderObjs)
        {
            yield return new WaitForSeconds(renderTime);
            obj.SetActive(true);
        }
    }
}
