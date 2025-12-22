using UnityEngine;
using Photon.Pun;
using Suncheon.UI;
using UnityEngine.SceneManagement;

namespace Suncheon.Player
{
    public class PlayerMoveManager : MonoBehaviourPunCallbacks
    {
        [Header("캐릭터 관련")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerAnimManager playerAnimManager;
        [SerializeField] private GameObject go_CamPivot;
        public GameObject go_Char;
        [SerializeField] private GameObject go_CharBody;
        public float moveSpeed;                   // [2023.11.01] [작성] KKH : 캐릭터 이동 속도
        public bool use_vehicle;

        [SerializeField] private float vehicleSpeed;

        [SerializeField] private Rigidbody rb;
        [SerializeField] private PhotonView pv;

        public bool IsJoyStick = false;
        [SerializeField] private VariableJoystick joystick;
        [HideInInspector] public int joystickFingerId;

        Vector3 lookForward; 
        Vector3 lookRight;

        private void Awake()
        {
            if (!pv)
            {
                pv = GetComponent<PhotonView>();
            }

            if (!playerManager)
            {
                playerManager = GetComponent<PlayerManager>();
            }

            if (!playerAnimManager)
            {
                playerAnimManager = GetComponent<PlayerAnimManager>();
            }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            joystickFingerId = -1;
            if (!joystick)
            {
                joystick = GameObject.Find("Variable Joystick").GetComponent<VariableJoystick>();
            }
#endif            
        }

        private void Start()
        {
            if (pv.IsMine)
            {
                go_CamPivot.SetActive(true);
            }
        }

        private void FixedUpdate()
        {
#if PHOTON_UNITY_NETWORKING
            if (pv.IsMine)
            {
                if (UIInteractionManager.Instance != null)
                {
                    if (!UIInteractionManager.Instance.IsUIInteraction)
                    {
                        Vector3 moventValue = GetMovementVectorNormalized();

                        if (playerAnimManager.IsSitIdle)
                        {
                            float ypos = go_CamPivot.transform.rotation.eulerAngles.y;
                            if (ypos <= 90.0f)
                            {
                                go_Char.transform.rotation = Quaternion.Euler(new Vector3(0.0f, ypos, 0.0f));
                            }

                            if (ypos >= 270.0f)
                            {
                                go_Char.transform.rotation = Quaternion.Euler(new Vector3(0.0f, ypos, 0.0f));
                            }
                        }
                        else
                        {
                            CharMove(moventValue);
                        }
                    }
                }
            }
#endif
        }

        // [2023.11.01] [작성] KKH
        // [2023.02.21] [수정] OJY
        protected void CharMove(Vector3 movementValue)
        {
            bool isMove = movementValue.magnitude != 0;

            if (isMove)
            {
                if (use_vehicle)
                {
                    moveSpeed = 10;
                }
                else
                {
                    moveSpeed = 5;
                }

                // [23.12] [작성] KKH : 카메라가 바라보는 방향 캐릭터가 보기
                //Quaternion rot = new Quaternion(0.0f, go_CamPivot.transform.rotation.y, 0.0f, go_CamPivot.transform.rotation.w);
                //go_Char.transform.rotation = rot;

                // [24.02.20] [수정] KKH : 방향키가 바라보는 방향으로 캐릭터 보기
                //Quaternion rot = new Quaternion(0.0f, movementValue.z, 0.0f, 0.0f);

                float angle = Mathf.Atan2(movementValue.x, movementValue.z) * Mathf.Rad2Deg;
                go_CharBody.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f));

                // [23.12] [작성] KKH : 카메라가 바라보는 방향 캐릭터가 이동

                //현재 씬이름 비교
                if (SceneManager.GetActiveScene().name.Equals("04_Myroom"))
                {
                    lookForward = Vector3.forward;
                    lookRight = Vector3.right;
                }
                else
                {
                    lookForward = new Vector3(go_CamPivot.transform.forward.x, 0f, go_CamPivot.transform.forward.z).normalized;
                    lookRight = new Vector3(go_CamPivot.transform.right.x, 0f, go_CamPivot.transform.right.z).normalized;
                }

                Vector3 moveDir = lookForward * movementValue.z + lookRight * movementValue.x;

                if (!Wall_Check(body.forward))
                {
                    rb.MovePosition(transform.position + (moveDir * moveSpeed * Time.fixedDeltaTime));
                }
            }
        }

        #region 벽과 가까운지 확인
        [Header("벽과 가까운지 확인")]
        public float maxRaycastDistance = 1; // 벽까지의 거리
        public LayerMask wallLayerMask; // 벽의 layer
        [SerializeField] Transform body; // 기준이 되는 캐릭터 몸체
        [SerializeField] bool nearWall = false;

        /// <summary>
        /// 벽과 가까운지 반환
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>가까우면 true, 아니면 false</returns>
        bool Wall_Check(Vector3 dir)
        {
            // 캐릭터의 현재 위치와 전방 방향을 가져옴
            Vector3 characterPosition = body.position + body.up;

            Ray ray = new Ray(characterPosition, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRaycastDistance, wallLayerMask))
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxRaycastDistance, Color.green);

                // 벽에 가까우면 true, 아니면 false
                return true;
            }
            else
            {
                // 벽에 가까우지 않으면 false
                return false;
            }
        }
        #endregion

        // [2023.11.01] [작성] KKH
        // [2023.02.13] [수정] OJY
        // [2023.02.15] [수정] KKH
        // [2023.02.21] [수정] OJY
        // [2023.03.12] [수정] OJY
        public Vector3 GetMovementVectorNormalized()
        {
            Vector3 movementValue = new Vector3(0, 0, 0);
            if (playerAnimManager.IsSitIdle) return movementValue;            

            //조이스틱 일 때
            if (joystick && IsJoyStick)
            {
                movementValue = HandleMovementInput(movementValue);
            }
            else //키보드 일 때
            {
                movementValue = Insert_MoveKey(movementValue);
            }

            if (playerAnimManager.IsVehicle)
            {
                movementValue *= 1.5f;
            }

            PlayerAnimUpdata((movementValue.x != 0.0f || movementValue.z != 0.0f));
            return movementValue;
        }

        /// <summary>
        /// 개인서재 내에서 조이스틱 이동
        /// </summary>
        Vector3 HandleMovementInput(Vector3 movement)
        {
            Vector3 direction = joystick.Direction;

            // 보정된 각도로 이동 벡터 생성. 옆으로 45도 회전
            float adjustedX = direction.x * Mathf.Sqrt(0.5f) + direction.y * Mathf.Sqrt(0.5f);
            float adjustedY = direction.y * Mathf.Sqrt(0.5f) - direction.x * Mathf.Sqrt(0.5f);

            movement = new Vector3(adjustedX, 0.0f, adjustedY);
            return movement;
        }

        /// <summary>
        /// 이동 입력
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        Vector3 Insert_MoveKey(Vector3 movement)
        {
            if (SceneManager.GetActiveScene().name.Equals("04_Myroom"))
            {
                if ((Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.UpArrow))))
                {
                    movement += Vector3.forward;
                    movement += Vector3.right;
                }

                if ((Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.DownArrow))))
                {
                    movement += Vector3.back;
                    movement += Vector3.left;
                }

                if ((Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.LeftArrow))))
                {
                    movement += Vector3.left;
                    movement += Vector3.forward;
                }

                if ((Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.RightArrow))))
                {
                    movement += Vector3.right;
                    movement += Vector3.back;
                }
            }
            else
            {
                if ((Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.UpArrow))))
                {
                    movement += Vector3.forward;
                }

                if ((Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.DownArrow))))
                {
                    movement += Vector3.back;
                }

                if ((Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.LeftArrow))))
                {
                    movement += Vector3.left;
                }

                if ((Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.RightArrow))))
                {
                    movement += Vector3.right;
                }
            }

            return movement;
        }

        private void PlayerAnimUpdata(bool isWalk)
        {
            if (pv.IsMine)
            {
                if (isWalk)
                {
                    if (playerAnimManager.IsVehicle || playerAnimManager.IsSitIdle)
                    {
                    }
                    else
                    {
                        playerAnimManager.SetWalk();
                    }
                }
                else
                {
                    if (playerAnimManager.IsVehicle || playerAnimManager.IsSitIdle || playerAnimManager.IsDance || playerAnimManager.IsHello || playerAnimManager.IsSmlie || playerAnimManager.IsSad)
                    {
                    }
                    else
                    {
                        playerAnimManager.SetIdle();
                    }
                }
            }
        }

        /// <summary>
        /// 카메라off, 이동x, UI x
        /// </summary>
        public void Cam_Off()
        {
            go_CamPivot.SetActive(false);
            UIInteractionManager.Instance.IsUIInteraction = true;
            UIInteractionManager.Instance.ui_Canvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// 카메라on, 이동 o, UI o
        /// </summary>
        public void Cam_On()
        {
            go_CamPivot.SetActive(true);
            UIInteractionManager.Instance.IsUIInteraction = false;
            UIInteractionManager.Instance.ui_Canvas.gameObject.SetActive(true);
        }
    }
}
