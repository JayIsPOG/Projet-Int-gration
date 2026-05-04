using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 10f;
    public int facingDirection = 1;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRen;
    private Animator animator;

    Coroutine speedRoutine;

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (speedRoutine != null) StopCoroutine(speedRoutine);
        speedRoutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        float original = movementSpeed;
        movementSpeed *= multiplier;
        yield return new WaitForSeconds(duration);
        movementSpeed = original;
        speedRoutine = null;
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        spriteRen = GetComponent<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }
    void Update () {
        float speedX = 0;
        float speedY = 0;

        if(Input.GetKey ("w")) speedY += movementSpeed;
        if(Input.GetKey ("s")) speedY -= movementSpeed;
        if(Input.GetKey ("d")) speedX += movementSpeed;
        if(Input.GetKey ("a")) speedX -= movementSpeed;


        if(speedY != 0 && speedX != 0){
            speedY *= 0.7071f;
            speedX *= 0.7071f;
        }
        if(-speedX > 0 && transform.localScale.x < 0 ||
            -speedX < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
        animator.SetFloat("Speed", rb.velocity.magnitude);
        rb.velocity = new Vector2(speedX, speedY);
        
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1,transform.localScale.y, transform.localScale.z);
    }
}