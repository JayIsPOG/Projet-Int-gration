using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public GameObject rotateIcon;
    public GameObject objToRotate;
    public bool playerIn, hV; //horizontal / vertical
    public Sprite sprite1, sprite2;
    public bool canEmit;
    //public GameObject[] surface;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerIn)
        {
            GetComponent<AudioSource>().Play();
            if (hV)
            {
                if(objToRotate.GetComponent<SpriteRenderer>().sprite == sprite1){
                    objToRotate.transform.rotation = Quaternion.Euler(0, 0, 90);
                    objToRotate.GetComponent<SpriteRenderer>().sprite = sprite2;
                    StartCoroutine(UpdateTimer());
                    //foreach(GameObject e in surface)
                    //{
                    //    if(e.GetComponent<LightEmitter>().angleOffset == -90)
                    //        e.GetComponent<LightEmitter>().angleOffset = 0;
                    //    else
                    //        e.GetComponent<LightEmitter>().angleOffset = -90;
                    //}

                    //objToRotate.transform.Rotate(0f, 0.0f, -270.0f, Space.Self);
                }
                else
                {
                    //foreach(GameObject e in surface)
                    //{
                    //    if(e.GetComponent<LightEmitter>().angleOffset == -90)
                    //        e.GetComponent<LightEmitter>().angleOffset = 0;
                    //    else
                    //        e.GetComponent<LightEmitter>().angleOffset = -90;
                    //}
                    objToRotate.transform.rotation = Quaternion.Euler(0, 0, 0);
                    objToRotate.GetComponent<SpriteRenderer>().sprite = sprite1;
                    StartCoroutine(UpdateTimer());
                    //objToRotate.transform.Rotate(0f, 0.0f, 90.0f, Space.Self);
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
    IEnumerator UpdateTimer()
    {
        objToRotate.transform.GetChild(0).GetComponent<LightEmitter>().enabled = false;
        objToRotate.transform.GetChild(1).GetComponent<LightEmitter>().enabled = false;
        yield return new WaitForSeconds(0.01f);
        objToRotate.transform.GetChild(0).GetComponent<LightEmitter>().enabled = true;
        objToRotate.transform.GetChild(1).GetComponent<LightEmitter>().enabled = true;
    }
}
