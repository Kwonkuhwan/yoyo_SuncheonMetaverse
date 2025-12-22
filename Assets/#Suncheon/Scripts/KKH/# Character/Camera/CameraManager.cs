using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using Suncheon.Player;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UI;
using Suncheon.UI;
using UnityEngine.SceneManagement;

namespace Suncheon
{
    public class CameraManager : MonoBehaviour
    {
        public Camera m_Camera;

        [SerializeField] private float rotCamXAxisSpeed_PC = 0.5f;     // [2023.11.01] [작성] KKH : 카메라 x축 회전속도
        [SerializeField] private float rotCamYAxisSpeed_PC = 0.3f;     // [2023.11.01] [작성] KKH : 카메라 y축 회전속도

        [SerializeField] private float rotCamXAxisSpeed_Mob = 0.25f;   // [2023.11.01] [작성] KKH : 카메라 x축 회전속도
        [SerializeField] private float rotCamYAxisSpeed_Mob = 0.15f;   // [2023.11.01] [작성] KKH : 카메라 y축 회전속도

        [SerializeField] private float limitMinX = -35.0f;          // [2023.11.01] [작성] KKH : 카메라 x축 회전 범위 (최소)
        [SerializeField] private float limitMaxX = 25.0f;           // [2023.11.01] [작성] KKH : 카메라 x축 회전 범위 (최대)

        private float eulerAngleX;                                  // [2023.11.01] [작성] KKH : 마우스 좌 / 우 이동으로 카메라 y축 회전
        private float eulerAngleY;                                  // [2023.11.01] [작성] KKH : 마우스 위 / 아래 이동으로 카메라 x축 회전

        bool thirdPerson = true;

        //public string targetLayerName = "Player"; // 원하는 레이어 이름
        //int targetLayerMask;

        [SerializeField] private GameObject go_Char;

        private void Awake()
        {
            //targetLayerMask = 1 << LayerMask.NameToLayer(targetLayerName);
            SetUICamera();
        }

        void LateUpdate()
        {
            if(SceneManager.GetActiveScene().name.Equals("04_Myroom"))
            {
                return;
            }

            if (!UIInteractionManager.Instance.IsUIInteraction)
            {
#if UNITY_EDITOR || UNITY_WEBGL
                CamRotate();
#else 
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    
                    if (!IsTouchOverUI(touch.position))
                    {                    
                        RotateWithSwipe(i);
                    }
                }
#endif

            }
        }

        bool IsTouchOverUI(Vector2 touchPosition)
        {
            // 이벤트 시스템 가져오기
            EventSystem eventSystem = EventSystem.current;

            // 포인터 이벤트 생성
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = touchPosition;

            // UI 요소 검사
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            return results.Count > 0;
        }

        void RotateWithSwipe(int touchIndex)
        {
            Touch touch = Input.GetTouch(touchIndex);

            if (touch.phase == UnityEngine.TouchPhase.Moved)
            {
                eulerAngleY += touch.deltaPosition.x * rotCamYAxisSpeed_Mob;
                eulerAngleX -= touch.deltaPosition.y * rotCamXAxisSpeed_Mob;
                eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

                transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0.0f);
                go_Char.transform.rotation = Quaternion.Euler(0.0f, eulerAngleY, 0.0f);
            }
        }

        // [2023.11.01] [작성] KKH
        protected void CamRotate()
        {
            if (GetCamMouseLeftClick())
            {
                Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                eulerAngleY += mouse.x * rotCamYAxisSpeed_PC;
                eulerAngleX -= mouse.y * rotCamXAxisSpeed_PC;
                eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

                transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0.0f);
                go_Char.transform.rotation = Quaternion.Euler(0.0f, eulerAngleY, 0.0f);
            }            
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }

            if (angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }

        private bool GetCamMouseLeftClick()
        {
            return Mouse.current.leftButton.magnitude != 0.0f;
        }

        private void SetUICamera()
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("UI_Player_Camera"))
            {
                m_Camera.GetUniversalAdditionalCameraData().cameraStack.Add(obj.GetComponent<Camera>());
            }
        }

        public void LookTarget(Transform target)
        {
            transform.rotation = Quaternion.LookRotation(target.position - transform.position);
            eulerAngleX = transform.eulerAngles.x - 360;
            eulerAngleY = transform.eulerAngles.y;
        }

        /// <summary>
        /// 플레이어의 시선 변경 3인칭 <-> 1인칭
        /// </summary>
        /// <param name="value">true이면 3인칭 사용</param>
        public void SetThirdPerson(bool value)
        {
            thirdPerson = value;
            if (thirdPerson)
            {
                //transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                m_Camera.transform.localPosition = new Vector3(0.0f, 3.0f, -3.0f);
                m_Camera.transform.localRotation = Quaternion.Euler(15.0f, 0.0f, 0.0f);
                limitMinX = -35.0f;
            }
            else
            {
                //transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                m_Camera.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
                m_Camera.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                limitMinX = -5.0f;
            }
        }
    }
}