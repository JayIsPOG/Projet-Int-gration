using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_attack : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    private PlayerMovement pm;
    private Rigidbody2D rb;
    private bool leftReleased = true;
    private bool rightReleased = true;
    [SerializeField] private GameObject blockObject;
    public bool IsBlocking { get; private set; }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButton(0)) leftReleased = true;
        if (!Input.GetMouseButton(1)) rightReleased = true;

        if (Input.GetMouseButton(0) && leftReleased && rb.velocity.magnitude < 0.1f)
        {
            animator.SetTrigger("Attack");
            leftReleased = false;
        }

        if (Input.GetMouseButton(1) && rightReleased)
        {
            IsBlocking = true;
            animator.SetBool("IsBlocking", true);
            blockObject?.SetActive(true);
            rb.velocity = Vector2.zero;
            pm.enabled = false;
            rightReleased = false;
        }
        else if (!Input.GetMouseButton(1))
        {
            IsBlocking = false;
            animator.SetBool("IsBlocking", false);
            blockObject?.SetActive(false);
            pm.enabled = true;
        }




    }


}
