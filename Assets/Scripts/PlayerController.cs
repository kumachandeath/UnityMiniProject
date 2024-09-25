using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Rigidbody rigid;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineFreeLook freeLookCamera;  // TPS ī�޶�

    private Vector3 moveDirection;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        Move();
        RotateTowardsCameraDirection();
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");


        // ī�޶� �������� �̵� ���� ���
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        moveDirection = (forward * vertical + right * horizontal).normalized;
    }

    private void Move()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            rigid.velocity = moveDirection * moveSpeed;

        }
        else
        {
            rigid.velocity = Vector3.zero;
        }
    }

    private void RotateTowardsCameraDirection()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            // ī�޶��� forward ���⿡ �÷��̾ ȸ��
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0; // Y�� ����
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            rigid.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
    }
}
