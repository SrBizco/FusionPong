using Fusion;                // 👈 nuevo
using UnityEngine;

public class BallController : NetworkBehaviour   // 👈 antes era MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    private Rigidbody rb;

    public override void Spawned()               // 👈 en red, reemplaza Start()
    {
        rb = GetComponent<Rigidbody>();
        if (Object.HasStateAuthority)            // 👈 solo el server lanza la bola
            Launch();
    }

    void Launch()
    {
        float dirX = Random.value < 0.5f ? -1f : 1f;
        float dirY = Random.Range(-0.5f, 0.5f);
        rb.linearVelocity = new Vector3(dirX, dirY, 0f).normalized * speed;
    }

    public void ResetBall()
    {
        if (!Object.HasStateAuthority) return;   // 👈 solo server resetea
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        Launch();
    }

    void OnCollisionEnter(Collision _)
    {
        if (!Object.HasStateAuthority) return;   // 👈 solo server ajusta velocidad
        rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }
}
