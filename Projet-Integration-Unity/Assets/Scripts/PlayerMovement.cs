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
        rb.velocity = new Vector2(speedX, speedY);

    }
}