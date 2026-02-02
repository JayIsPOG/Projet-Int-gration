using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    public GameObject grabIcon;
    private bool grabbed;
    public Sprite[] grabIconSprites;
    public LayerMask wallLayer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay2D(Collision2D other) {
        if(other.transform.tag == "Player")
        {
            grabIcon.SetActive(true);

            if(Input.GetMouseButton(2))
            {
                grabIcon.GetComponent<SpriteRenderer>().sprite = grabIconSprites[1];
                if(Input.GetKey ("w") && Mathf.Floor(other.transform.position.y) < Mathf.Round(transform.position.y) ){
                    Drag(Vector3.up);
                }
                if(Input.GetKey ("s") && Mathf.Round(other.transform.position.y) > Mathf.Round(transform.position.y) ){
                    Drag(-Vector3.up);
                }
                if(Input.GetKey ("d") && Mathf.Floor(other.transform.position.x) < Mathf.Floor(transform.position.x) ){
                    Drag(Vector3.right);
                }
                if(Input.GetKey ("a") && Mathf.Round(other.transform.position.x) > Mathf.Floor(transform.position.x) ){
                    Drag(-Vector3.right);
                }
            }else{
                grabIcon.GetComponent<SpriteRenderer>().sprite = grabIconSprites[0];
            }
        }
    }

    void Drag(Vector3 direction){
        if(Physics2D.Raycast(transform.position + direction * 1.5f, (Vector2)direction, 0.5f, wallLayer) == false){
            transform.position += direction;
        }
    }

    void OnCollisionExit2D(Collision2D other){
        if(other.transform.tag == "Player")
        {
            grabIcon.SetActive(false);
        }
    }
}
