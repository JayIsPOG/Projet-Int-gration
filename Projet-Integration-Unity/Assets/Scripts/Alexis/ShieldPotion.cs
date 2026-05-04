using UnityEngine;

public class ShieldPotion : PowerUp
{
    [Header("Shield")]
    public float duration = 5f;

    protected override void OnPickup()
    {
        if (playerHealth != null)
            playerHealth.ApplyInvulnerability(duration);
    }
}
