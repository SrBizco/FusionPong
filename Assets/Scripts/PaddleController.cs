using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PaddleController : MonoBehaviour
{
    [SerializeField] private bool isLeft = true;
    [SerializeField] private float speed = 12f;
    [SerializeField] private Collider topWall;     // arrastrá el BoxCollider de la pared de arriba
    [SerializeField] private Collider bottomWall;  // arrastrá el BoxCollider de la pared de abajo

    Rigidbody rb;
    float input;
    float minY, maxY;

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
        // límites internos del campo medidos por colliders
        float topInnerY = topWall.bounds.min.y;
        float bottomInnerY = bottomWall.bounds.max.y;
        float halfPaddle = GetComponent<Collider>().bounds.extents.y;
        const float margin = 0.01f;

        maxY = topInnerY - halfPaddle - margin;
        minY = bottomInnerY + halfPaddle + margin;
    }

    void Update()
    {
        input =
            (isLeft
                ? (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f)
                : (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) + (Input.GetKey(KeyCode.DownArrow) ? -1f : 0f));
        input = Mathf.Clamp(input, -1f, 1f);
    }

    void FixedUpdate()
    {
        Vector3 pos = rb.position + new Vector3(0f, input * speed * Time.fixedDeltaTime, 0f);
        pos.y = Mathf.Clamp(pos.y, minY, maxY); // tope real contra paredes
        rb.MovePosition(pos);
    }
}
