using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WhaleFeedingGame : MonoBehaviour
{
    public float gravity;
    float acceleration = 1;
    public Button btn;

    bool up = false;
    float uptimer = 0;

    float playTime = 0;
    int scoreTime = 1;
    public static int score = 0;

    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI scoreRank;

    //
    WhaleFeedingRespawn respawn;

    // Game Event
    public static Action GameStart;
    public static Action GameEnd;
    public static Action<int> EatScore;

    public static bool isGameOver = false;

    void Awake()
    {
        btn.onClick.AddListener(PlayerUp);
        respawn = FindAnyObjectByType<WhaleFeedingRespawn>();        

        GameStart += Reset;
        EatScore += ScoreAdd;
    }

    private void OnDestroy()
    {
        GameStart -= Reset;
        EatScore -= ScoreAdd;
    }

    private void Reset()
    {
        Init();
    }

    void Init()
    {
        acceleration = 1;
        up = false;
        uptimer = 0;
        playTime = 0;
        scoreTime = 1;
        score = 0;
        scoreUI.text = score.ToString();
    }

    void Update()
    {
        playTime += Time.deltaTime;
        if(playTime > scoreTime)
        {
            // 1초마다
            scoreTime += 1;

            if (scoreTime < 60)
            {
                ScoreAdd(10);
                acceleration = 1.1f;
                respawn._acceleration = 1.1f;
            }
            else if (scoreTime < 120)
            {
                ScoreAdd(20);
                acceleration = 1.3f;
                respawn._acceleration = 1.3f;
            }
            else if (scoreTime < 180)
            {
                ScoreAdd(30);
                acceleration = 1.5f;
                respawn._acceleration = 1.5f;
            }
            else
            {
                ScoreAdd(50);
                acceleration = 2f;
                respawn._acceleration = 2f;
            }
        }

        if (up)
        {
            uptimer += Time.deltaTime;
            transform.Translate(0, gravity * 5 * Time.deltaTime,0);
            if (uptimer > 0.1f)
            {
                uptimer = 0;
                up = false;
            }
            return;
        }

        transform.Translate(0, -gravity * acceleration * Time.deltaTime, 0);
    }

    void PlayerUp()
    {
        up = true;
    }

    void ScoreAdd(int num)
    {
        score += num;
        scoreUI.text = score.ToString();// ====> 2023.11.07 ryu 점수를 얻을때 마다 텍스트 변경
    }
}