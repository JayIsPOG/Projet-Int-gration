using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 3;

    public void TakeDamage(int dmg)
    {
        hp -= dmg;

        Debug.Log("Enemy hit");

        if (hp <= 0)
            Destroy(gameObject);
    }
}
