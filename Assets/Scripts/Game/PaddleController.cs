using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PaddleController : MonoBehaviour
{
    [SerializeField] private bool isLeft = true;
    [SerializeField] private float speed = 12f;
    [SerializeField] private Collider topWall;
    [SerializeField] private Collider bottomWall;
    [SerializeField] private bool useLocalInput = true;

    Rigidbody rb;
    float input;
    float minY, maxY;

    public void SetNetworkInput(float moveY) => input = Mathf.Clamp(moveY, -1f, 1f);

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        float topInnerY = topWall.bounds.min.y;
        float bottomInnerY = bottomWall.bounds.max.y;
        float halfPaddle = GetComponent<Collider>().bounds.extents.y;
        const float margin = 0.01f;
        maxY = topInnerY - halfPaddle - margin;
        minY = bottomInnerY + halfPaddle + margin;
    }

    void Update()
    {
        if (!useLocalInput) return;  // 👈 si es online, otro script setea el input

        input =
            (isLeft
                ? (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f)
                : (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) + (Input.GetKey(KeyCode.DownArrow) ? -1f : 0f));
        input = Mathf.Clamp(input, -1f, 1f);
    }

    void FixedUpdate()
    {
        Vector3 pos = rb.position + new Vector3(0f, input * speed * Time.fixedDeltaTime, 0f);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        rb.MovePosition(pos);
    }
    public void SetWalls(Collider top, Collider bottom)
    {
        topWall = top;
        bottomWall = bottom;
    }
}
