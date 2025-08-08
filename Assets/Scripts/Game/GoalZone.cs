using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public enum Side { Left, Right }

    [SerializeField] private Side side = Side.Left;
    [SerializeField] private LayerMask ballMask;
    [SerializeField] private ScoreManager score; 

    void Awake()
    {
        if (score == null) score = FindFirstObjectByType<ScoreManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((ballMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        if (score != null)
        {
            if (side == Side.Left) score.AddRight();
            else score.AddLeft();
        }

        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out BallController ball))
        {
            ball.ResetAndServe(3f);
        }
    }
}
