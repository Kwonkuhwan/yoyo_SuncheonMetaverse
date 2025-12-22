using System.Collections;
using UnityEngine;

public class TreasureChestAnim : MonoBehaviour
{
    public bool playAnim; // 플레이 중 인스펙터에서 true 바꿔주면 애니메이션 재생

    [Space(10)]
    [SerializeField] private float openSpeed;
    [SerializeField] private float animTime;
    [SerializeField] private Quaternion openedUpperRot; // 상자가 다 열렸을 때 상자 윗부분 각도
    [SerializeField] private GameObject chestUpper;
    [SerializeField] private GameObject goOpenEffect;

    private bool isPlayingAnim;


    void Update()
    {
        if (playAnim == true && isPlayingAnim == false)
        {
            playAnim = false;
            Clear();
            StartCoroutine(CoOpen());
        }
    }

    private void Clear()
    {
        chestUpper.transform.localRotation = Quaternion.identity;
    }

    private IEnumerator CoOpen()
    {
        isPlayingAnim = true;
        float useTime = animTime;
        Instantiate(goOpenEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);

        while (useTime > 0f)
        {
            useTime -= Time.deltaTime;
            chestUpper.transform.localRotation = Quaternion.Lerp(chestUpper.transform.localRotation, openedUpperRot, Time.deltaTime * openSpeed);

            yield return null;
        }
        isPlayingAnim = false;
    }
}
