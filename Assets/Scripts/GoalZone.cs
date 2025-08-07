using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public enum Side { Left, Right }

    [SerializeField] private Side side = Side.Left;
    [SerializeField] private LayerMask ballMask;     // ← arrastrá aquí la layer "Ball" desde el inspector
    [SerializeField] private ScoreManager score;     // ← referencia directa (evita FindObjectOfType)

    private void OnTriggerEnter(Collider other)
    {
        // Rechazar si NO es la pelota (por layer)
        if ((ballMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        if (score == null) return;

        if (side == Side.Left) score.AddRight();
        else score.AddLeft();

        // Reset de pelota (si está presente el controlador)
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out BallController ball))
            ball.ResetBall();
    }
}
