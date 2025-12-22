using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using Suncheon.Player;
using Suncheon.WebData;

namespace Suncheon
{
    public class CustomizationMediator : MonoBehaviour
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private CustomMaizingManager maizingManager;

        [SerializeField]
        private GameObject kekosCharacter;

        [SerializeField]
        private Button allRandomizationButton;

        [SerializeField]
        private bool loadSavedData;

        [Header("BODY MENU")]
        [SerializeField]
        private GameObject[] bodyMenus;

        [SerializeField]
        private PartCustomization torsoCustomization;

        [SerializeField]
        private PartCustomization legsCustomization;

        [SerializeField]
        private PartCustomization feetCustomization;

        [SerializeField]
        private PartCustomization handsCustomization;

        [SerializeField]
        private PartCustomization fullTorsoCustomization;

        [SerializeField]
        private PartCustomization torsoPropCustomization;

        [Header("HEAD AND SKIN MENU")]
        [SerializeField]
        private GameObject[] headMenus;

        [SerializeField]
        private PartCustomization hairCustomization;

        [SerializeField]
        private PartCustomization capCustomization;

        [SerializeField]
        private PartCustomization glassesCustomization;

        [SerializeField]
        private EyesCustomization eyesCustomization;

        [SerializeField]
        private SkinCustomization skinCustomization;

        [SerializeField]
        private PartCustomization eyebrowsCustomization;

        [SerializeField]
        private FaceCaracteristicsCustomization faceCaracteristicsCustomization;

        private void Awake()
        {
            playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();

            kekosCharacter.SetActive(true);

            ChangeToBodyMenuIndex(0);

            // BODY
            fullTorsoCustomization.Init(kekosCharacter);
            torsoCustomization.Init(kekosCharacter);
            legsCustomization.Init(kekosCharacter);
            feetCustomization.Init(kekosCharacter);
            handsCustomization.Init(kekosCharacter);
            torsoPropCustomization.Init(kekosCharacter);

            // HEAD
            hairCustomization.Init(kekosCharacter);
            capCustomization.Init(kekosCharacter);
            glassesCustomization.Init(kekosCharacter);
            eyesCustomization.Init(kekosCharacter);
            skinCustomization.Init(kekosCharacter);
            eyebrowsCustomization.Init(kekosCharacter);
            faceCaracteristicsCustomization.Init(kekosCharacter);

            allRandomizationButton.onClick.AddListener(RandomizeAll);

            if (!playerManager.IsCharacterData)
            {
                eyesCustomization.Randomize();
                skinCustomization.Randomize();
            }
        }

        private void OnEnable()
        {
            if (playerManager.IsCharacterData)
            {
                LoadSettings();
            }
        }

        public void LoadSettings()
        {
            string savedCharacterJson = playerManager.AvatarInfo;
            if (string.IsNullOrEmpty(savedCharacterJson)) return;

            SaveCustomizationModel saveCustomizationModel = JsonUtility.FromJson<SaveCustomizationModel>(savedCharacterJson);
            torsoCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.torsoSelected);
            legsCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.legsSelected);
            feetCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.feetSelected);
            handsCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.handsSelected);
            fullTorsoCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.fullTorsoSelected);
            torsoPropCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.torsoPropSelected);
            hairCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.hairSelected);
            capCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.capSelected);
            glassesCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.glassesSelected);
            eyesCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.eyesSelected);
            skinCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.skinBaseSelected,
                saveCustomizationModel.skinDetailSelected);
            eyebrowsCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.eyebrowsSelected);
            faceCaracteristicsCustomization.ChangeWithSaveItemInfo(saveCustomizationModel.faceCharacteristicsSelected);
        }

        public void SaveSettings()
        {
            SaveCustomizationModel saveCustomizationModel = new SaveCustomizationModel
            {
                torsoSelected = torsoCustomization.GetSaveItemInfo(),
                legsSelected = legsCustomization.GetSaveItemInfo(),
                feetSelected = feetCustomization.GetSaveItemInfo(),
                handsSelected = handsCustomization.GetSaveItemInfo(),
                fullTorsoSelected = fullTorsoCustomization.GetSaveItemInfo(),
                torsoPropSelected = torsoPropCustomization.GetSaveItemInfo(),
                hairSelected = hairCustomization.GetSaveItemInfo(),
                capSelected = capCustomization.GetSaveItemInfo(),
                glassesSelected = glassesCustomization.GetSaveItemInfo(),
                eyesSelected = eyesCustomization.GetSaveItemInfo(),
                eyebrowsSelected = eyebrowsCustomization.GetSaveItemInfo(),
                faceCharacteristicsSelected = faceCaracteristicsCustomization.GetSaveItemInfo(),
            };

            (saveCustomizationModel.skinBaseSelected, saveCustomizationModel.skinDetailSelected) =
                skinCustomization.GetSaveItemInfo();

            string savedCharacterJson = JsonUtility.ToJson(saveCustomizationModel);
            Request_AvatarSave request_AvatarSave = new Request_AvatarSave(savedCharacterJson);
            UTILS.Log(request_AvatarSave.avatar);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.avatarSaveUrl}", request_AvatarSave, (jsonData) =>
            {
                Response_AvatarSave response_AvatarSave = JsonUtility.FromJson<Response_AvatarSave>(jsonData);
                if (response_AvatarSave == null) return;
                SaveAvata(response_AvatarSave, savedCharacterJson);
            }));
            //PlayerPrefs.SetString(_SaveString, savedCharacterJson);
        }

        private void SaveAvata(Response_AvatarSave resultData, string savedCharacterJson)
        {
            if(resultData.rtnCode == "000")
            {
                playerManager.AvatarInfo = savedCharacterJson;
                maizingManager.MetaverseStart();
            }
            else
            {
                UTILS.Log($"{resultData.rtnMsg}");
            }
        }

        private void RandomizeAll()
        {
            // BODY
            if (Random.Range(0, 100) < 50)
            {
                torsoCustomization.Randomize();
                legsCustomization.Randomize();
            }
            else
            {
                fullTorsoCustomization.Randomize();
            }
            feetCustomization.Randomize();
            handsCustomization.Randomize();
            torsoPropCustomization.Randomize();

            // HEAD
            hairCustomization.Randomize();
            capCustomization.Randomize();
            glassesCustomization.Randomize();
            eyesCustomization.Randomize();
            skinCustomization.Randomize();
            eyebrowsCustomization.Randomize();
            faceCaracteristicsCustomization.Randomize();
        }

        public void ChangeToBodyMenuIndex(int index)
        {
            foreach (GameObject bodyMenu in bodyMenus)
                bodyMenu.SetActive(false);

            bodyMenus[index].SetActive(true);
        }

        public void ChangeToHeadMenuIndex(int index)
        {
            foreach (GameObject headMenu in headMenus)
                headMenu.SetActive(false);

            headMenus[index].SetActive(true);
        }

        private void OnDestroy()
        {
            skinCustomization.Clean();
            eyesCustomization.Clean();
        }
    }
}
