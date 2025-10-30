using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class UI_Setting_Sound : MonoBehaviour
    {
        [SerializeField] private Button btn_MadePeople;
        [SerializeField] private Button btn_LogOut;
        [SerializeField] private Slider slider_AllSound;
        [SerializeField] private Slider slider_BGMSound;
        [SerializeField] private Slider slider_EffectSount;

        private void Awake()
        {
            btn_MadePeople.onClick.AddListener(() => BtnMadePeopleClick());
            btn_LogOut.onClick.AddListener(() => BtnLogOutClick());
            slider_AllSound.onValueChanged.AddListener((value) => SliderAllSoundValueChange(value));
            slider_BGMSound.onValueChanged.AddListener((value) => SliderBGMSoundValueChange(value));
            slider_EffectSount.onValueChanged.AddListener((value) => SliderEffectSountValueChange(value));

            slider_AllSound.value = GameManager.Instance.AllVolume;
            slider_BGMSound.value = GameManager.Instance.BGMVolume;
            slider_EffectSount.value = GameManager.Instance.EffectVolume;
        }

        private void BtnMadePeopleClick()
        {
            UTILS.Log("BtnMadePeopleClick");
        }

        private void BtnLogOutClick()
        {
            UTILS.Log("BtnLogOutClick");
            StartCoroutine(LogOut());
        }

        IEnumerator LogOut()
        {
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.logoutUrl}", (jsonData) =>
            {
            }));

            NetworkManager.Instance.OnLeaveRoom();
            yield return new WaitForSeconds(1.0f);
            UTILS.LoadingSceneLoad("01_Title_Scene");
        }

        private void SliderAllSoundValueChange(float value)
        {
            UTILS.Log($"SliderAllSoundValueChange : {value}");
            GameManager.Instance.AllVolume = value;
        }

        private void SliderBGMSoundValueChange(float value)
        {
            UTILS.Log($"SliderBGMSoundValueChange : {value}");
            GameManager.Instance.BGMVolume = value;
        }

        private void SliderEffectSountValueChange(float value)
        {
            UTILS.Log($"SliderEffectSountValueChange : {value}");
            GameManager.Instance.EffectVolume = value;
        }
    }
}