using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public enum Side { Left, Right }

    [SerializeField] private Side side = Side.Left;
    [SerializeField] private LayerMask ballMask;
    [SerializeField] private ScoreManager score; // puede estar null en server si no lo asignaste

    void Awake()
    {
        if (score == null) score = FindFirstObjectByType<ScoreManager>(); // best-effort
    }

    private void OnTriggerEnter(Collider other)
    {
        // sólo si es la pelota
        if ((ballMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        // 1) sumar si hay score (solo lo hace el server)
        if (score != null)
        {
            if (side == Side.Left) score.AddRight();
            else score.AddLeft();
        }

        // 2) SIEMPRE resetear la pelota (pero sólo el server tiene autoridad sobre Ball)
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out BallController ball))
        {
            // si estás usando ResetAndServe con delay, llamalo acá
            ball.ResetAndServe(3f);
        }
    }
}
