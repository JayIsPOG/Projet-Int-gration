using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReceiver : MonoBehaviour
{
    public bool hitByLight;
    public float waveLenghtReceived;
    public float waveLenghtToOpen = 550;
    public bool open;
    public Sprite spriteLit, spriteUnlit;
    public bool lightPassesThrough;
    public GameObject symbols;

    void Update()
    {
        if(hitByLight)
        {
            if(waveLenghtToOpen <= waveLenghtReceived + 10 && waveLenghtToOpen >= waveLenghtReceived - 10)
                open = true;
            else
                open = false;
        }
        else
            open = false;
        if (open)
        {
            if(GetComponent<SpriteRenderer>().sprite != spriteLit)
            {
                GetComponent<SpriteRenderer>().sprite = spriteLit;
                symbols.SetActive(true);
                GetComponent<AudioSource>().Play();
            }
        }
        else{
            GetComponent<SpriteRenderer>().sprite = spriteUnlit;
            symbols.SetActive(false);
        }
    }
}
