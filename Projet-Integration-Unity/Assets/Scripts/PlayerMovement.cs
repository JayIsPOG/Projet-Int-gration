using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 10f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRen;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        spriteRen = GetComponent<SpriteRenderer>();
    }
    void Update () {
        float speedX;
        float speedY;

        if (Input.GetKey ("w")) {
            speedY = movementSpeed;
        }   else if (Input.GetKey ("s")) {
            speedY = -movementSpeed;
        }else{
            speedY = 0;
        }
 
        if (Input.GetKey ("a") && !Input.GetKey ("d")) {
            speedX = -movementSpeed;
            spriteRen.flipX = true;
        } else if (Input.GetKey ("d") && !Input.GetKey ("a")) {
            speedX = movementSpeed;
            spriteRen.flipX = false;
        }else{
            speedX = 0;
        }
    
        rb.velocity = new Vector2(speedX, speedY);

    }
}