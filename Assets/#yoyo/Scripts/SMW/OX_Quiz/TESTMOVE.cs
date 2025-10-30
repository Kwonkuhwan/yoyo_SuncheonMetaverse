using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suncheon;

public class TESTMOVE : MonoBehaviour
{
    public float speed = 5f;

    Vector3 startpos;

    private void Start()
    {
        startpos = transform.position;

        QuizManager.instance.GradingEvent += ResetPos;
        QuizManager.instance.ResetQuizEvent += ResetPos;
    }

    private void OnDestroy()
    {
        QuizManager.instance.GradingEvent -= ResetPos;
        QuizManager.instance.ResetQuizEvent -= ResetPos;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(0, 0, -speed * Time.deltaTime);
        }
    }

    void ResetPos(bool answer)
    {
        transform.position = startpos;
    }

    void ResetPos()
    {
        transform.position = startpos;
    }
}
