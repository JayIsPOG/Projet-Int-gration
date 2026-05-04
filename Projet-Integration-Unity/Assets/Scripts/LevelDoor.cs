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
    public AudioSource audioSource;
    public AudioClip openClip, closeClip;
    public bool isPuzzleLvl = true;
    private bool saving;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(signal.GetComponent<DoorSignal>().open)
        {
            if (!open)
            {
                open = true;
                GetComponent<SpriteRenderer>().sprite = spriteOpen;
                audioSource.clip = openClip;
                audioSource.Play();
            }
            
        }else{
            if (open)
            {
                open = false;
                GetComponent<SpriteRenderer>().sprite = spriteClose;
                audioSource.clip = closeClip;
                audioSource.Play();
            }
        }

        if(Input.GetKey(KeyCode.E) && open == true && player == true){
            if(!saving)
            {
                saving = true;
                if(isPuzzleLvl)
                    FindObjectsByType<GlobalData>(FindObjectsSortMode.None)[0].puzzlesCompleted ++;
                FindObjectsByType<SceneManagerPuzzle>(FindObjectsSortMode.None)[0].CloseScene(lvlName);
            }
        }
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
