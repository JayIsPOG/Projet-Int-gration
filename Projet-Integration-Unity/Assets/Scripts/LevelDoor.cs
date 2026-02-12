using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoor : MonoBehaviour
{
    public bool open, player;
    public GameObject signal;
    public GameObject icon;
    public Sprite spriteOpen, spriteClose;
    public string lvlName;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(signal.GetComponent<LightReceiver>().hitByLight)
        {
            open = true;
            GetComponent<SpriteRenderer>().sprite = spriteOpen;
        }else{
            open = false;
            GetComponent<SpriteRenderer>().sprite = spriteClose;
        }

        if(Input.GetKey(KeyCode.E) && open == true && player == true)
            SceneManager.LoadScene(lvlName);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.tag == "Player" && open == true)
        {
            player = true;
            icon.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other.transform.tag == "Player" && open == false)
        {
            player = false;
            icon.SetActive(false);
        }
    }
}
