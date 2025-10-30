using Suncheon.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedingGameOver : MonoBehaviour
{
    [SerializeField] private GameObject feedingGameObj;
    [SerializeField] private Button btn_GameOverDone;

    [SerializeField] private TMP_Text text_Score;

    [SerializeField] private GameObject player;

    private void Awake()
    {
        if (btn_GameOverDone != null) btn_GameOverDone.onClick.AddListener(() => BtnGameOverDoneClick());
    }

    private void OnEnable()
    {
        text_Score.text = WhaleFeedingGame.score.ToString();
        player.SetActive(false);
    }

    private void BtnGameOverDoneClick()
    {
        WhaleFeedingGame.isGameOver = false;
        gameObject.SetActive(false);
        UIInteractionManager.Instance.ClosePopUp(feedingGameObj);
    }
}
