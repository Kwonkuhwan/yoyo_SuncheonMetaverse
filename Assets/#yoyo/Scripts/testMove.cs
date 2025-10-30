using UnityEngine;
using Suncheon;

public class testMove : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]Transform oxGamePos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        QuizManager.instance.SetPlayerQuiz(oxGamePos);

    }

    private void FixedUpdate()
    {

        Vector3 moventValue = GetMovementVectorNormalized();

        CharMove(moventValue);

    }

    // [2023.11.01] [작성] KKH
    protected void CharMove(Vector3 movementValue)
    {
        bool isMove = movementValue.magnitude != 0;

        if (isMove)
        {

            rb.MovePosition(transform.position + (movementValue * 1.0f * Time.fixedDeltaTime));
        }
    }

    // [2023.11.01] [작성] KKH
    public Vector3 GetMovementVectorNormalized()
    {
        Vector3 movementValue = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            movementValue += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movementValue += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movementValue += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movementValue += Vector3.right;
        }
        return movementValue;
    }
}
