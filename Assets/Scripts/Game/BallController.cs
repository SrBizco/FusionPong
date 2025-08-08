using Fusion;
using UnityEngine;
using System.Collections;

public class BallController : NetworkBehaviour
{
    [SerializeField] private float speed = 8f;
    [Networked] public NetworkBool IsFrozen { get; set; }
    public NetworkPaddle LastPaddleHit { get; private set; }

    Rigidbody rb;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        if (Object.HasStateAuthority) Freeze(true);
    }

    void Launch()
    {
        float dirX = Random.value < 0.5f ? -1f : 1f;
        float dirY = Random.Range(-0.5f, 0.5f);
        rb.linearVelocity = new Vector3(dirX, dirY, 0f).normalized * speed;
    }

    public void Freeze(bool freeze)
    {
        if (!Object.HasStateAuthority) return;
        IsFrozen = freeze;
        if (freeze) rb.linearVelocity = Vector3.zero;
    }

    public void ResetAndServe(float delaySeconds)
    {
        if (!Object.HasStateAuthority) return;
        StopAllCoroutines();
        StartCoroutine(ServeAfter(delaySeconds));
    }

    IEnumerator ServeAfter(float delay)
    {
        Freeze(true);
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(delay);
        Freeze(false);
        Launch();
    }

    void OnCollisionEnter(Collision c)
    {
        if (!Object.HasStateAuthority || IsFrozen) return;

        if (c.gameObject.TryGetComponent<NetworkPaddle>(out var paddle))
            LastPaddleHit = paddle;

        rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

}
