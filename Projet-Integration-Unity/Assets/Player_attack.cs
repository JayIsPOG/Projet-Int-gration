using UnityEngine;

public class Player_attack : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    public GameObject attackHitbox;

    private bool facingRight = true;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        if (move > 0) facingRight = true;
        if (move < 0) facingRight = false;

        if (Input.GetMouseButtonDown(0) && rb.velocity.magnitude < 0.1f)
        {
            animator.SetTrigger("Attack");

            if (facingRight)
                attackHitbox.transform.localPosition = new Vector2(1, 0);
            else
                attackHitbox.transform.localPosition = new Vector2(-1, 0);
        }
    }

    // appelée par animation
    public void ActivateHitbox()
    {
        Debug.Log("attack");
        attackHitbox.SetActive(true);
    }

    // appelée par animation
    public void DeactivateHitbox()
    {
        attackHitbox.SetActive(false);
    }
}
