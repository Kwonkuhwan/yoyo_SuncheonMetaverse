using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class UI_MapPopUp : MonoBehaviour
    {
        [SerializeField] private TMP_Text text_Info;
        [SerializeField] private Button btn_OK;
        [SerializeField] private Button btn_Cancel;
        [SerializeField] private string infoSub;

        [SerializeField] private GameObject go_Char;
        [SerializeField] private List<Transform> lib_Transforms;
        [SerializeField] private List<GameObject> go_lookAtposList;
        [SerializeField] private GameObject[] go_MapBtns;

        [SerializeField] private LibName libName;

        private void Awake()
        {
            btn_OK.onClick.AddListener(() => OnOKBtnClick());
            btn_Cancel.onClick.AddListener(() => OnCancelBtnClick());
        }

        public void SetInfoData(LibName _libName)
        {
            infoSub = infoSub.Replace("\\n", "\n");
            libName = _libName;
            text_Info.text = $"{libName} {infoSub}";
        }

        private void OnDisable()
        {
            MapFadeIn();
        }

        public void MapFadeIn()
        {
            foreach (GameObject btn in go_MapBtns)
            {
                ButtonControl buttonControl = btn.gameObject.GetComponent<ButtonControl>();
                if (buttonControl != null)
                {
                    buttonControl.OnBackGroundFadeIn();
                    buttonControl.OnBtnImageFadeIn();
                }
            }
        }

        public void OnOKBtnClick()
        {            
            Teleport();
            libName = LibName.None;
            MapFadeIn();
            UIInteractionManager.Instance.ClosePopUp(gameObject);
            UIInteractionManager.Instance.ClosePopUp(UIInteractionManager.Instance.ui_Map);
        }

        public void OnCancelBtnClick()
        {
            libName = LibName.None;
            MapFadeIn();
            UIInteractionManager.Instance.ClosePopUp(gameObject);
        }

        public void Teleport()
        {
            if (SceneManager.GetActiveScene().name.Equals("02_Garden_Scene"))
            {
                if (go_Char == null)
                {
                    go_Char = NetworkManager.Instance.Go_Player;
                }

                go_Char.transform.position = lib_Transforms[(int)libName].position;
                go_Char.transform.LookAt(go_lookAtposList[(int)libName].transform);
            }
            else if(SceneManager.GetActiveScene().name.Equals("03_Inside") ||
                SceneManager.GetActiveScene().name.Equals("04_Myroom") || 
                SceneManager.GetActiveScene().name.Equals("05_AVRoom"))
            {
                if(libName == LibName.오천그린광장)
                {
                    NetworkManager.Instance.SetSpawnerPos(SpawnerPos.오천그린광장);
                }
                else if(libName == LibName.삼산도서관)
                {
                    NetworkManager.Instance.SetSpawnerPos(SpawnerPos.삼산도서관);
                }
                else if(libName == LibName.그림책도서관)
                {
                    NetworkManager.Instance.SetSpawnerPos(SpawnerPos.그림책도서관);
                }
                else if(libName == LibName.연향도서관)
                {
                    NetworkManager.Instance.SetSpawnerPos(SpawnerPos.연향도서관);
                }
                else if(libName == LibName.기적의도서관)
                {
                    NetworkManager.Instance.SetSpawnerPos(SpawnerPos.기적의도서관);
                }
                else if(libName == LibName.조례호수도서관)
                {
                    NetworkManager.Instance.SetSpawnerPos(SpawnerPos.조례호수도서관);
                }
                else if(libName == LibName.신대도서관)
                {
                    NetworkManager.Instance.SetSpawnerPos(SpawnerPos.신대도서관);
                }
                else if(libName == LibName.물의광장)
                {
                    NetworkManager.Instance.SetSpawnerPos(SpawnerPos.물의광장);
                }

                NetworkManager.Instance.OnLeaveRoom();
                UTILS.LoadingSceneLoad("02_Garden_Scene");
            }
        }
    }
}