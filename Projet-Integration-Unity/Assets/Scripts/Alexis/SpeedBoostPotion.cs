using UnityEngine;

public class SpeedBoostPotion : PowerUp
{
    [Header("Speed Boost")]
    public float multiplier = 1.6f;
    public float duration = 5f;

    protected override void OnPickup()
    {
        if (player == null) return;
        var move = player.GetComponent<PlayerMovement>();
        if (move != null)
            move.ApplySpeedBoost(multiplier, duration);
    }
}
