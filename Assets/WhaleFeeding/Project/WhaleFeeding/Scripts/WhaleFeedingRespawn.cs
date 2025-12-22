using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleFeedingRespawn : MonoBehaviour
{
    public GameObject[] objs; // 0닻, 1드럼, 2바위
    public GameObject[] tcbpos; // 0탑, 1센터, 2바위

    public GameObject[] feed;
    bool fishrespawn = true;

    int num = 0;

    float timer = 0;
    float timerMax;

    public float _acceleration = 1f;

    private List<GameObject> instances;

    void Awake()
    {
        // 첫 시작
        timerMax = Random.Range(1.0f, 2.0f);

        WhaleFeedingGame.GameStart += Init;
        WhaleFeedingGame.GameEnd += ObjectClear;
    }

    private void OnDestroy()
    {
        WhaleFeedingGame.GameEnd -= ObjectClear;
        WhaleFeedingGame.GameStart -= Init;
    }

    void Update()
    {
        if (!WhaleFeedingGame.isGameOver)
        {
            timer += Time.deltaTime;

            if (timer > timerMax / 2 && fishrespawn)
            {
                FishInstantiate();
                fishrespawn = false;
            }

            if (timer > timerMax)
            {
                fishrespawn = true;
                ObjectInstantiate();
                timer = 0;
                timerMax = Random.Range(1.0f, 2.0f);
            }
        }
    }

    private void ObjectClear()
    {
        if (instances != null)
        {
            foreach (GameObject obj in instances)
            {
                Destroy(obj);
            }

            instances.Clear();
        }
    }

    public void Init()
    {
        if (instances != null)
        {
            foreach (GameObject obj in instances)
            {
                DestroyImmediate(obj);
            }

            instances.Clear();
        }

        instances = new List<GameObject>();

        num = 0;

        timer = 0;
        timerMax = Random.Range(1.0f, 2.0f);
    }

    void FishInstantiate()
    {
        GameObject obj = Instantiate(feed[Random.Range(0, 2)], tcbpos[1].transform.position, Quaternion.identity, tcbpos[1].transform);
        float randtemp = Random.Range(-3.0f, 3.0f);
        obj.transform.position += new Vector3(0, randtemp, 0);
        obj.GetComponent<WhaleFeedingObject>().InCreaseAcceleration(_acceleration);
        instances.Add(obj);
    }

    void ObjectInstantiate()
    {
        //중복방지
        int temp;
        do
            temp = Random.Range(0, 4);
        while (temp == num);
        num = temp;

        ////생성할 오브젝트, 생성할 위치, 생성시 회전
        ////1번Center는 위치만 변경 / 0,2번은 스케일만 변경
        //if(num == 1)
        //{
        GameObject obj = Instantiate(objs[num], tcbpos[num].transform.position, Quaternion.identity, tcbpos[num].transform);
        float randtemp = Random.Range(-3.0f, 3.0f);
        obj.transform.position += new Vector3(0, randtemp, 0);
        obj.GetComponent<WhaleFeedingObject>().InCreaseAcceleration(_acceleration);
        //obj.transform.rotation = Quaternion.Euler(0,0,Random.Range(0,360));//진행방향 바뀜

        instances.Add(obj);
        //}
        //else
        //{
        //    GameObject obj = Instantiate(objs[num], tcbpos[num].transform.position, Quaternion.identity, tcbpos[num].transform);
        //    float randtemp = Random.Range(0.7f, 1.1f);
        //    obj.transform.localScale = new Vector3(randtemp, randtemp, 0);
        //    obj.GetComponent<WhaleFeedingObject>().InCreaseAcceleration(_acceleration);

        //    instances.Add(obj);
        //}

    }
}
