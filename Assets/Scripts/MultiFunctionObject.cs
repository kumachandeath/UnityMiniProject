using UnityEngine;

public class MultiFunctionObject : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool canRotate;  // ȸ�� ����
    [SerializeField] private bool canMoveUpDown;  // ���� �̵� ����
    [SerializeField] private bool canOrbit;  // ���� ����
    [SerializeField] private bool isTrampoline;  // Ʈ���޸� ����

    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;  // ȸ�� ��
    [SerializeField] private float rotationSpeed = 50f;  // ȸ�� �ӵ�

    [Header("Up-Down Movement Settings")]
    [SerializeField] private float moveDistance = 3f;  // ���� �̵� �Ÿ�
    [SerializeField] private float moveSpeed = 2f;  // ���� �̵� �ӵ�
    private Vector3 initialPosition;

    [Header("Orbit Settings")]
    [SerializeField] private Transform orbitCenter;  // ���� �߽�
    [SerializeField] private float orbitRadius = 5f;  // ���� �ݰ�
    [SerializeField] private float orbitSpeed = 30f;  // ���� �ӵ�

    [Header("Trampoline Settings")]
    [SerializeField] private float bounceForce = 10f;  // Ʈ���޸� ��·�

    private void Start()
    {
        // ���� �̵��� ���� �ʱ� ��ġ ����
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (canRotate)
        {
            RotateObject();
        }

        if (canMoveUpDown)
        {
            MoveUpDown();
        }

        if (canOrbit && orbitCenter != null)
        {
            OrbitAround();
        }
    }

    private void RotateObject()
    {
        // ������ ���� �������� ȸ��
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    private void MoveUpDown()
    {
        // ���Ϸ� �ݺ� �̵�
        float newY = initialPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OrbitAround()
    {
        // ���� �߽��� �������� ����
        transform.RotateAround(orbitCenter.position, Vector3.up, orbitSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTrampoline && collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // Ʈ���޸� ȿ���� �÷��̾�� �������� ��·� ����
                playerRigidbody.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
