using System.Collections.Generic;
using UnityEngine;

public class WhaleFeedingPlayer : MonoBehaviour
{
    [SerializeField] private int miniGameTypeId;
    [SerializeField] private WhaleFeedingGameStart whaleFeedingGameStart;

    public List<Sprite> playerMoveSpritelist;
    public Sprite playerEat;
    public Sprite playerHit;
    public Sprite playerUp;
    public float TimeDelay = 0.3f;

    SpriteRenderer player;
    private float timer = 0;
    private bool eat = false;

    void Awake()
    {
        player = GetComponent<SpriteRenderer>();
        player.sprite = playerMoveSpritelist[0];

        WhaleFeedingGame.GameStart += Init;
        WhaleFeedingGame.EatScore += PlayerEat;
        WhaleFeedingGame.GameEnd += PlayerHit;
    }

    private void OnDestroy()
    {
        WhaleFeedingGame.GameStart -= Init;
        WhaleFeedingGame.EatScore -= PlayerEat;
        WhaleFeedingGame.GameEnd -= PlayerHit;
    }

    void Update()
    {
        if (timer > 0)
        {
            if (eat)
            {
                player.sprite = playerMoveSpritelist[2];
                eat = false;
            }
        }

        if (timer < TimeDelay)
            timer += Time.deltaTime;
        else
        {
            timer = 0;
            Playerbase();
        }
    }

    void Init()
    {
        gameObject.SetActive(true);
        timer = 0;
        eat = false;
        player.sprite = playerMoveSpritelist[0];
        gameObject.transform.localPosition = new Vector3(-7, 0, 0);
    }

    void Playerbase()
    {
        if (player.sprite == playerMoveSpritelist[0])
            player.sprite = playerMoveSpritelist[1];
         else if (player.sprite == playerMoveSpritelist[1])
            player.sprite = playerMoveSpritelist[2];
        else if (player.sprite == playerMoveSpritelist[2])
            player.sprite = playerMoveSpritelist[3];
         else if (player.sprite == playerMoveSpritelist[3])
            player.sprite = playerMoveSpritelist[0];

    }

    /// <summary>
    /// 플레이어가 먹이를 먹었을 경우
    /// </summary>
    void PlayerEat(int score)
    {
        timer = -0.5f;
        player.sprite = playerEat;
        eat = true;
    }

    void PlayerHit()
    {
        player.sprite = playerHit;
        WhaleFeedingGame.isGameOver = true;
    }

}
