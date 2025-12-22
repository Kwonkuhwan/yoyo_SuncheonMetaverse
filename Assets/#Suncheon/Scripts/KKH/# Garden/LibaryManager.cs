using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Suncheon
{
    public class LibaryManager : MonoBehaviour
    {
        [SerializeField] private string libName;

        [SerializeField] private GameObject player;

        [SerializeField] private GameObject object_2D;
        [SerializeField] private GameObject object_3D;

        [Range(0.0f, 300.0f)]
        [SerializeField] private float maxDistance;

        //bool isSpawn = false;

        private void Start()
        {
            try
            {
                if (object_2D == null) object_2D = transform.GetChild(0).gameObject;
                if (object_3D == null) object_3D = transform.GetChild(1).gameObject;
            }
            catch
            {
                object_2D = null;
                object_3D = null;
            }
        }


        private void Update()
        {
            if (NetworkManager.Instance == null) return;

            if (player == null)
            {
                player = NetworkManager.Instance.Go_Player;
            }

            if (player != null)
            {
                float distance = Vector3.Distance(player.transform.position, transform.position);
                if (distance > maxDistance)
                {
                    if (object_2D)
                    {
                        object_2D.SetActive(true);
                    }

                    if (object_3D)
                    {
                        object_3D.SetActive(false);
                    }
                    //Destroy(object_3D);
                    //isSpawn = false;
                }
                else
                {
                    if (object_2D)
                    {
                        object_2D.SetActive(false);
                    }

                    if (object_3D)
                    {
                        object_3D.SetActive(true);
                    }
                    //Spawn();
                }
            }
        }

        //void Spawn()
        //{
        //    if (object_3D == null && !isSpawn)
        //    {
        //        isSpawn = true;
        //        if (!ReferenceEquals(object_3D, null))
        //        {
        //            ReleaseObj();
        //        }

        //        Addressables.InstantiateAsync(libName, transform).Completed +=
        //            (AsyncOperationHandle<GameObject> obj) =>
        //            {
        //                object_3D = obj.Result;
        //            };
        //    }
        //}

        //void ReleaseObj()
        //{
        //    Addressables.ReleaseInstance(object_3D);
        //}
    }
}