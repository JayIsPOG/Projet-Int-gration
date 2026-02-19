using UnityEngine;

public class ennemy_mouvement : MonoBehaviour
{
    public float speed = 2f;
    private Rigidbody2D rb;
    private bool stopped = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!stopped)
        {
            rb.velocity = Vector2.left * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        stopped = true;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;


    }
}
