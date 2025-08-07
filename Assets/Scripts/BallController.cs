using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    private Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    void Start() { Launch(); }

    void Launch()
    {
        float dirX = Random.value < 0.5f ? -1f : 1f;
        float dirY = Random.Range(-0.5f, 0.5f);
        rb.linearVelocity = new Vector3(dirX, dirY, 0f).normalized * speed;
    }

    public void ResetBall()
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        Launch();
    }

    void OnCollisionEnter(Collision _)
    {
        // mantener siempre el mismo módulo (evita que “se muera”)
        rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }
}
