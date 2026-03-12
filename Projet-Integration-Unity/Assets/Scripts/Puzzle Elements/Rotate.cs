using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public GameObject rotateIcon;
    public GameObject objToRotate;
    public bool playerIn;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerIn)
        {
            objToRotate.transform.localScale = new Vector3(-objToRotate.transform.localScale.x, 1f, 1f);
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
