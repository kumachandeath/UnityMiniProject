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
    [SerializeField] private CinemachineFreeLook freeLookCamera;  // TPS 카메라

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck; // 발밑 확인용 트랜스폼
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer; // 바닥을 나타내는 레이어

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


        // 카메라 기준으로 이동 벡터 계산
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
            // 이동 벡터에 따라 속도 적용
            Vector3 movement = moveDirection * moveSpeed;
            rigid.velocity = new Vector3(movement.x, rigid.velocity.y, movement.z);  // 수평 방향만 속도 설정
        }
        else
        {
            // 수평 방향 속도 제거
            rigid.velocity = new Vector3(0,rigid.velocity.y,0);
        }
    }

    private void Jump()
    {
        // 점프력 적용
        rigid.velocity = new Vector3(rigid.velocity.x, jumpForce, rigid.velocity.z);
    }

    private void CheckGroundStatus()
    {
        // 플레이어가 바닥에 있는지 확인 (원형 캐스팅을 사용하여 바닥 체크)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // 에디터에서 Ground Check 범위를 시각화
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
            // 카메라의 forward 방향에 플레이어를 회전
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0; // Y축 고정
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            rigid.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
    }
}
