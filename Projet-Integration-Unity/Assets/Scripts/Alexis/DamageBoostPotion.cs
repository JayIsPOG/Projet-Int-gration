using UnityEngine;

public class DamageBoostPotion : PowerUp
{
    [Header("Damage Boost")]
    public float multiplier = 2f;
    public float duration = 6f;

    protected override void OnPickup()
    {
        if (player == null) return;
        var attack = player.GetComponent<Player_attack>();
        if (attack != null)
            attack.ApplyDamageBoost(multiplier, duration);
    }
}
