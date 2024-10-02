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
    [SerializeField] private CinemachineFreeLook freeLookCamera;  // TPS ī�޶�

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck; // �߹� Ȯ�ο� Ʈ������
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer; // �ٴ��� ��Ÿ���� ���̾�

    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 0.5f; // �� ���� �Ÿ�

    [Header("Physics Materials")]
    [SerializeField] private PhysicMaterial normalMaterial; // �⺻ ���� ����
    [SerializeField] private PhysicMaterial noFrictionMaterial; // ������ ���� ���� ����

    [Header("Slope Settings")]
    [SerializeField] private float maxSlopeAngle = 45f;  // �ִ� ��� ����
    [SerializeField] private float slopeForce = 100f;      // ���鿡�� �߰� ��

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
        if (moveDirection.magnitude >= 0.1f && IsOnSlope())
        {
            Vector3 slopeDirection = Vector3.ProjectOnPlane(moveDirection, GetSlopeNormal());
            rigid.velocity = slopeDirection * moveSpeed + new Vector3(0, rigid.velocity.y, 0);
        }
        else if (moveDirection.magnitude >= 0.1f)
        {
            // �̵� ���Ϳ� ���� �ӵ� ����
            Vector3 movement = moveDirection * moveSpeed;
            rigid.velocity = new Vector3(movement.x, rigid.velocity.y, movement.z);  // ���� ���⸸ �ӵ� ����
        }
        else
        {
            // ���� ���� �ӵ� ����
            rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
        }

        if (IsWallDetected())
        {
            // ���� �پ��� �� ������ ����
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
            // ���鿡�� ������ ���� ���¿����� ������ ���� ������
            RestoreNormalFriction();

            // ������ �� Y�� �ӵ��� 0���� �ʱ�ȭ�Ͽ� ������ �ӵ� ����
            rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);

            // �� �� �������� ����
            rigid.velocity += Vector3.up * jumpForce;
        }
    }

    private void CheckGroundStatus()
    {
        // �÷��̾ �ٴڿ� �ִ��� Ȯ�� (���� ĳ������ ����Ͽ� �ٴ� üũ)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ApplySlopeForce()
    {
        if (IsOnSlope() && !Input.GetButton("Jump"))
        {
            rigid.AddForce(Vector3.down * slopeForce);  // ���鿡�� Ƣ������� ���� ����
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
        // �÷��̾ �̵��ϴ� �������� Ray�� ��� �� ����
        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, moveDirection, out hit, rayDistance, groundLayer))
        {
            Debug.Log("�� ������: " + hit.collider.name);
            return true;
        }
        return false;
    }

    private void ApplyNoFriction()
    {
        // ���� ������ �������� ������ ���� ������ ����
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.material = noFrictionMaterial;
        }
    }

    private void RestoreNormalFriction()
    {
        // ���� ����� �ٽ� �⺻ ���������� ����
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.material = normalMaterial;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // �����Ϳ��� Ground Check ������ �ð�ȭ
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Raycast�� �ð�ȭ
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheck.position, moveDirection * rayDistance);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            GameManager.instance.PlayerReachedGoal();
        }
    }
}
