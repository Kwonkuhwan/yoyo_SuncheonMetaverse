using Suncheon.UI;
using UnityEngine;
using Suncheon.WebData;
using Suncheon.Player;
using System.Collections;

namespace Suncheon
{
    public class TreasureChest : MonoBehaviour, Clickable
    {
        enum TreasureChestIndex
        {
            None = 0,
            오천그린광장,
            물의광장,
            삼산도서관,
            그림책도서관,
            연향도서관,
            기적의도서관,
            조례호수도서관,
            신대도서관
        }

        [SerializeField] private bool isSpawn;
        [SerializeField] private bool isOpen;

        TreasureChestAnim treasureChestAnim;
        [SerializeField] private TreasureChestIndex treasureChestIndex = TreasureChestIndex.None;

        private void Awake()
        {
            treasureChestAnim = GetComponent<TreasureChestAnim>();
        }

        private void Update()
        {
            if (!isSpawn)
            {
                if (!NetworkManager.Instance.Go_Player) return;
                if (!NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().IsTreasureLoad) return;
                Response_Treasure treasureInfo = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().treasureInfo;
                if (treasureInfo == null) return;

                if (treasureInfo.location1 == 1 && treasureChestIndex.ToString() == "오천그린광장")
                {
                    Destroy(gameObject);
                }
                else if (treasureInfo.location2 == 1 && treasureChestIndex.ToString() == "물의광장")
                {
                    Destroy(gameObject);
                }
                else if (treasureInfo.location3 == 1 && treasureChestIndex.ToString() == "삼산도서관")
                {
                    Destroy(gameObject);

                }
                else if (treasureInfo.location4 == 1 && treasureChestIndex.ToString() == "그림책도서관")
                {
                    Destroy(gameObject);

                }
                else if (treasureInfo.location5 == 1 && treasureChestIndex.ToString() == "연향도서관")
                {
                    Destroy(gameObject);

                }
                else if (treasureInfo.location6 == 1 && treasureChestIndex.ToString() == "기적의도서관")
                {
                    Destroy(gameObject);

                }
                else if (treasureInfo.location7 == 1 && treasureChestIndex.ToString() == "조례호수도서관")
                {
                    Destroy(gameObject);

                }
                else if (treasureInfo.location8 == 1 && treasureChestIndex.ToString() == "신대도서관")
                {
                    Destroy(gameObject);

                }

                isSpawn = true;
            }
        }

        public void OnClick()
        {
            if (isOpen) return;

            treasureChestAnim.playAnim = true;
            isOpen = true;
            UTILS.Log("보물 상자 클릭");

            Response_Treasure treasureInfo = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().treasureInfo;

            Request_UpsertTreasure request_UpsertTreasure = null;
            if(treasureChestIndex.ToString() == "오천그린광장")
            {
                treasureInfo.location1 = 1;
            }
            else if(treasureChestIndex.ToString() == "물의광장")
            {
                treasureInfo.location2 = 1;
            }
            else if (treasureChestIndex.ToString() == "삼산도서관")
            {
                treasureInfo.location3 = 1;
            }
            else if (treasureChestIndex.ToString() == "그림책도서관")
            {
                treasureInfo.location4 = 1;
            }
            else if (treasureChestIndex.ToString() == "연향도서관")                           
            {                                                                               
                treasureInfo.location5 = 1;
            }
            else if (treasureChestIndex.ToString() == "기적의도서관")                          
            {                                                                               
                treasureInfo.location6 = 1;
            }
            else if (treasureChestIndex.ToString() == "조례호수도서관")                        
            {                                                                               
                treasureInfo.location7 = 1;
            }
            else if (treasureChestIndex.ToString() == "신대도서관")                           
            {                                                                               
                treasureInfo.location8 = 1;
            }

            request_UpsertTreasure = new Request_UpsertTreasure(treasureInfo.location1, treasureInfo.location2, treasureInfo.location3, treasureInfo.location4, treasureInfo.location5, treasureInfo.location6, treasureInfo.location7, treasureInfo.location8);

            if (request_UpsertTreasure == null) return;

            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.upsertUserTreasure}", request_UpsertTreasure, (jsonData) => 
            {
                UTILS.Log(jsonData);

                Response_ReturnMsg response_UpsertTreasure = null;
                try
                {
                    response_UpsertTreasure = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    response_UpsertTreasure = null;
                }

                if(response_UpsertTreasure == null) return;

                if(response_UpsertTreasure.rtnCode == "000")
                {
                    UTILS.Log(response_UpsertTreasure.rtnMsg);
                    UIInteractionManager.Instance.ShowSystemPopUp($"{treasureChestIndex}의 보물상자를 찾았습니다.");
                    StartCoroutine(WiatObjectDestroy());
                }
                else
                {
                    UTILS.Log(response_UpsertTreasure.rtnMsg);
                }
            }));

        }

        [SerializeField] string obj_Name;
        public string Return_ObjName()
        {
            return obj_Name;
        }

        IEnumerator WiatObjectDestroy()
        {
            yield return new WaitForSeconds(3.0f);
            Destroy(gameObject);
        }

    }
}