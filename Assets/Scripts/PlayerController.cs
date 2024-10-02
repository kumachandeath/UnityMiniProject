using Cinemachine;
using System.Runtime.CompilerServices;
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

    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 0.5f; // 벽 감지 거리

    [Header("Physics Materials")]
    [SerializeField] private PhysicMaterial normalMaterial; // 기본 물리 재질
    [SerializeField] private PhysicMaterial noFrictionMaterial; // 마찰력 없는 물리 재질

    [Header("Slope Settings")]
    [SerializeField] private float maxSlopeAngle = 45f;  // 최대 경사 각도
    [SerializeField] private float slopeForce = 100f;      // 경사면에서 추가 힘

    private Vector3 moveDirection;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isOnSlope;

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
        ApplySlopeForce();
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
        if (moveDirection.magnitude >= 0.1f && IsOnSlope())
        {
            Vector3 slopeDirection = Vector3.ProjectOnPlane(moveDirection, GetSlopeNormal());
            rigid.velocity = slopeDirection * moveSpeed + new Vector3(0, rigid.velocity.y, 0);
        }
        else if (moveDirection.magnitude >= 0.1f)
        {
            // 이동 벡터에 따라 속도 적용
            Vector3 movement = moveDirection * moveSpeed;
            rigid.velocity = new Vector3(movement.x, rigid.velocity.y, movement.z);  // 수평 방향만 속도 설정
        }
        else
        {
            // 수평 방향 속도 제거
            rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
        }

        if (IsWallDetected())
        {
            // 벽에 붙었을 때 마찰을 제거
            ApplyNoFriction();
        }
        else
        {
            RestoreNormalFriction();
        }
    }

    private void Jump()
    {

        if (isGrounded)
        {
            // 경사면에서 마찰이 없는 상태에서의 점프력 버그 수정용
            RestoreNormalFriction();

            // 점프할 때 Y축 속도를 0으로 초기화하여 축적된 속도 제거
            rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);

            // 그 후 점프력을 적용
            rigid.velocity += Vector3.up * jumpForce;
        }
    }

    private void CheckGroundStatus()
    {
        // 플레이어가 바닥에 있는지 확인 (원형 캐스팅을 사용하여 바닥 체크)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ApplySlopeForce()
    {
        if (IsOnSlope() && !Input.GetButton("Jump"))
        {
            rigid.AddForce(Vector3.down * slopeForce);  // 경사면에서 튀어오르는 현상 방지
        }
    }

    private bool IsOnSlope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.3f, groundLayer))
        {
            Debug.Log($"{hit.transform.rotation}");
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            return angle <= maxSlopeAngle;
        }
        return false;
    }

    private Vector3 GetSlopeNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.3f, groundLayer))
        {
            return hit.normal;
        }
        return Vector3.up;
    }

    private bool IsWallDetected()
    {
        // 플레이어가 이동하는 방향으로 Ray를 쏘아 벽 감지
        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, moveDirection, out hit, rayDistance, groundLayer))
        {
            Debug.Log("벽 감지됨: " + hit.collider.name);
            return true;
        }
        return false;
    }

    private void ApplyNoFriction()
    {
        // 벽에 붙으면 마찰력을 제거한 물리 재질로 변경
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.material = noFrictionMaterial;
        }
    }

    private void RestoreNormalFriction()
    {
        // 벽을 벗어나면 다시 기본 마찰력으로 복구
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.material = normalMaterial;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 에디터에서 Ground Check 범위를 시각화
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Raycast를 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheck.position, moveDirection * rayDistance);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            GameManager.instance.PlayerReachedGoal();
        }
    }
}
