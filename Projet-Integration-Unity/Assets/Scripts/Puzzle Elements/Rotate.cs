using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public GameObject rotateIcon;
    public GameObject objToRotate;
    public bool playerIn, hV; //horizontal / vertical
    public Sprite sprite1, sprite2;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerIn)
        {
            if (hV)
            {
                if(objToRotate.GetComponent<SpriteRenderer>().sprite == sprite1){
                    objToRotate.GetComponent<SpriteRenderer>().sprite = sprite2;
                    objToRotate.transform.Rotate(0f, 0.0f, 270.0f, Space.Self);
                }
                else
                {
                    objToRotate.GetComponent<SpriteRenderer>().sprite = sprite1;
                    objToRotate.transform.Rotate(0f, 0.0f, 90.0f, Space.Self);
                }
                    
            }else{
                objToRotate.transform.localScale = new Vector3(-objToRotate.transform.localScale.x, 1f, 1f);
            }
            if(objToRotate.GetComponent<SpriteSorting>().offset == 130)
                objToRotate.GetComponent<SpriteSorting>().offset = 127;
            
            else if(objToRotate.GetComponent<SpriteSorting>().offset == 127)
                objToRotate.GetComponent<SpriteSorting>().offset = 130;
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.tag == "Player")
        {
            rotateIcon.SetActive(true);
            playerIn = true;
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if(other.transform.tag == "Player")
        {
            rotateIcon.SetActive(false);
            playerIn = false;
        }
    }
}
