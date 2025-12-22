using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleFeedingObject : MonoBehaviour
{
    public bool moving = true;
    //
    public float _speed = 5f;
    private float _acceleration = 1;

    public bool isFeed = true;      // 먹이면 true, 오브젝트면 false

    private void Start()
    {
        _acceleration = 1;
    }

    private void Update()
    {
        if(moving)
            transform.Translate(-_speed * _acceleration * Time.deltaTime, 0, 0);

        if (transform.position.x < -25f)
            Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.GetComponent<WhaleFeedingObject>().isFeed)
        {
            WhaleFeedingGame.EatScore(50);  // 점수
            Destroy(gameObject);
        }
        else
        {
            WhaleFeedingGame.GameEnd();
        }
    }

    public void InCreaseAcceleration(float increase)
    {
        _acceleration = increase;
    }
}
