using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Rigidbody rigid;

    [Header("Camera Settings")]
    [SerializeField] private CinemachineFreeLook freeLookCamera;  // TPS ī�޶�

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck; // �߹� Ȯ�ο� Ʈ������
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer; // �ٴ��� ��Ÿ���� ���̾�

    private Vector3 moveDirection;

    [SerializeField] private bool isGrounded;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovementInput();
        CheckGroundStatus();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
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
            // �̵� ���Ϳ� ���� �ӵ� ����
            Vector3 movement = moveDirection * moveSpeed;
            rigid.velocity = new Vector3(movement.x, rigid.velocity.y, movement.z);  // ���� ���⸸ �ӵ� ����
        }
        else
        {
            // ���� ���� �ӵ� ����
            rigid.velocity = new Vector3(0,rigid.velocity.y,0);
        }
    }

    private void Jump()
    {
        // ������ ����
        rigid.velocity = new Vector3(rigid.velocity.x, jumpForce, rigid.velocity.z);
    }

    private void CheckGroundStatus()
    {
        // �÷��̾ �ٴڿ� �ִ��� Ȯ�� (���� ĳ������ ����Ͽ� �ٴ� üũ)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // �����Ϳ��� Ground Check ������ �ð�ȭ
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
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
