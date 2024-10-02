using UnityEngine;

public class MultiFunctionObject : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool canRotate;  // 회전 여부
    [SerializeField] private bool canMoveUpDown;  // 상하 이동 여부
    [SerializeField] private bool canOrbit;  // 공전 여부
    [SerializeField] private bool isTrampoline;  // 트램펄린 여부

    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;  // 회전 축
    [SerializeField] private float rotationSpeed = 50f;  // 회전 속도

    [Header("Up-Down Movement Settings")]
    [SerializeField] private float moveDistance = 3f;  // 상하 이동 거리
    [SerializeField] private float moveSpeed = 2f;  // 상하 이동 속도
    private Vector3 initialPosition;

    [Header("Orbit Settings")]
    [SerializeField] private Transform orbitCenter;  // 공전 중심
    [SerializeField] private float orbitRadius = 5f;  // 공전 반경
    [SerializeField] private float orbitSpeed = 30f;  // 공전 속도

    [Header("Trampoline Settings")]
    [SerializeField] private float bounceForce = 10f;  // 트램펄린 상승력

    private void Start()
    {
        // 상하 이동을 위한 초기 위치 설정
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
        // 지정된 축을 기준으로 회전
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    private void MoveUpDown()
    {
        // 상하로 반복 이동
        float newY = initialPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OrbitAround()
    {
        // 공전 중심을 기준으로 공전
        transform.RotateAround(orbitCenter.position, Vector3.up, orbitSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTrampoline && collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // 트램펄린 효과로 플레이어에게 순간적인 상승력 적용
                playerRigidbody.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
