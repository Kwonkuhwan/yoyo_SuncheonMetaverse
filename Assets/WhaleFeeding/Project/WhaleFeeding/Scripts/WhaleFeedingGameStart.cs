using NUnit.Framework;
using Suncheon.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WhaleFeedingGameStart : MonoBehaviour
{
    [SerializeField] private Camera camera_Feeding;
    [SerializeField] private GameObject gameOverFadeOut;

    void Awake()
    {
        WhaleFeedingGame.GameStart += GameStart;
        WhaleFeedingGame.GameEnd += GameEnd;
    }

    private void Start()
    {
        // Event를 바인드하기위한 장치
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        WhaleFeedingGame.GameStart -= GameStart;
        WhaleFeedingGame.GameEnd -= GameEnd;
    }

    private void OnEnable()
    {
        if (Camera.main)
        {
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(camera_Feeding);
        }
    }

    private void OnDisable()
    {
        if (Camera.main)
        {
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Remove(camera_Feeding);
        }
    }

    void GameStart()
    {
        UIInteractionManager.Instance.OpenPopUp(gameObject);
    }

    void GameEnd()
    {
        gameOverFadeOut.SetActive(true);
    }
}
